using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PrefabChangedHandler
{
    private static readonly PrefabUtility.PrefabInstanceUpdated _oldUpdated;

    [MenuItem("CONTEXT/TextMeshProUGUI/Create Material Instance")]
    public static void CreateMaterialCopy(MenuCommand command)
    {
	    var text = (command.context as TextMeshProUGUI);
	    text.fontSharedMaterial = text.fontMaterial;
    }
    
    #region [Constructor] PrefabChangedHandler()
    static PrefabChangedHandler()
    {
        if( !Application.isPlaying )
        {
            _oldUpdated = PrefabUtility.prefabInstanceUpdated;
            PrefabUtility.prefabInstanceUpdated = PrefabInstanceUpdated;
        }
    }
    #endregion PrefabChangedHandler()

    #region PrefabInstanceUpdated()
    private static void PrefabInstanceUpdated( GameObject instance )
    {
        // First invoke old update prefab function
        _oldUpdated?.Invoke( instance );
        //Debug.Log( $"Updating prefab {instance.name}" );


        var parentObject = PrefabUtility.GetCorrespondingObjectFromSource(instance);
        string prefabPath = AssetDatabase.GetAssetPath(parentObject);
        //Debug.Log( $"Prefab path: {prefabPath}" );

        var tmps = instance.GetComponentsInChildren<TextMeshProUGUI>( true );
        var problematicTmps = new List<TextMeshProUGUI>();
        var materials = new List<Material>();
        foreach( var tmp in tmps )
        {
            // TextMeshPro component - check which materials it uses
            var mat = tmp.materialForRendering;
            if( mat == null ) continue;

            // Check if we use actual material, not instance of it
            // If we're already using proper material, then leave it be
            if( !mat.name.EndsWith( " (Instance)" ) ) continue;

            problematicTmps.Add( tmp );
        }

        if( problematicTmps.Count == 0 ) return;

        if( !EditorUtility.DisplayDialog( "Prefab creation warning", $"This prefab contains {problematicTmps.Count} instance(s) of TextMeshPro text with their own unique instances of material.\nJust packing them to prefab will lose all variance in their Face / Outline / Underlay / Lightning / Glow.\n\nDo you want to create individual material for each text inside prefab to save those variances?\n\n", "Yes - create material assets", "No - leave it be" ) ) return;


        foreach( var tmp in problematicTmps )
        {
            var mat = tmp.materialForRendering;

            // Check if material with that same properties exist
            Material newMat = GetIdenticalMaterialFromList( materials, mat );
            if( newMat != null ) {
                //Debug.Log( $"Found duplicate material! {mat} {newMat}" );
                tmp.fontSharedMaterial = newMat;
                continue;
            }

            // Create new material and add it to assets
            newMat = CreateNewTMPMaterial( mat );
            tmp.fontSharedMaterial = newMat;
            materials.Add( newMat );
        }

        // Apply all our changes to original Prefab
        // NOTE: Calling this function will update prefab, which means this exact function
        // will be called again with same GameObject. So just make sure to stop endless recursion.
        PrefabUtility.ApplyPrefabInstance( instance, InteractionMode.AutomatedAction );
    }
    #endregion PrefabInstanceUpdated()

    #region CreateNewTMPMaterial()
    private static Material CreateNewTMPMaterial( Material fromMaterial )
    {
        // Clone original material
        var mat = new Material( fromMaterial );

        // Clean up material name
        mat.name = fromMaterial.name.Replace( " SDF Material", "" ).Replace( "(Instance)", "" ).Replace( "(Clone)", "" ).Trim();
        //Debug.Log( $"Created new {mat.name} from {fromMaterial.name}" );


        // Save material to Assets. Unity supports only 1 Material per asset file,
        // so no chance of grouping more of them in single file
        var assetPath = AssetDatabase.GenerateUniqueAssetPath( $"Assets/{mat.name}-V00.mat" );
        AssetDatabase.CreateAsset( mat, assetPath );
        AssetDatabase.ImportAsset( assetPath );

        // Destroy old material
        GameObject.DestroyImmediate( fromMaterial );

        return mat;
    }
    #endregion CreateNewTMPMaterial()

    #region GetIdenticalMaterialFromList()
    private static Material GetIdenticalMaterialFromList( List<Material> materials, Material mat )
    {
        int ourPropCount = ShaderUtil.GetPropertyCount( mat.shader );
        var ourPropNames = Enumerable.Range( 0, ourPropCount ).Select( idx => ShaderUtil.GetPropertyName(mat.shader, idx) ).ToArray();
        var ourPropTypes = Enumerable.Range( 0, ourPropCount ).Select( idx => ShaderUtil.GetPropertyType(mat.shader, idx) ).ToArray();

        foreach( var m in materials )
        {
            // Compare materials
            var shader = m.shader;
            if( shader != mat.shader ) continue;
            int theirPropCount = ShaderUtil.GetPropertyCount( shader );
            if( ourPropCount != theirPropCount ) continue;
            
            // Check if all shader properties have the same name/type/value
            bool defNotTheSame = false;
            for( int i = 0; i < theirPropCount && !defNotTheSame; i++ )
            {
                // We don't compare ScaleRatio params, as those are constantly recalculated
                // http://digitalnativestudios.com/forum/index.php?topic=1461.0
                if( ourPropNames[i].StartsWith( "_ScaleRatio" ) ) continue;

                if( ShaderUtil.GetPropertyName( shader, i ) != ourPropNames[i]
                    || ShaderUtil.GetPropertyType( shader, i ) != ourPropTypes[i] )
                {
                    defNotTheSame = true;
                    break;
                }

                // Compare shader value
                var vn = ourPropNames[i];
                switch( ourPropTypes[i] )
                {
                    case ShaderUtil.ShaderPropertyType.Color: if( m.GetColor( vn ) != mat.GetColor( vn ) ) defNotTheSame = true; break;
                    case ShaderUtil.ShaderPropertyType.Vector: if( m.GetVector( vn ) != mat.GetVector( vn ) ) defNotTheSame = true; break;
                    case ShaderUtil.ShaderPropertyType.Float: if( m.GetFloat( vn ) != mat.GetFloat( vn ) ) defNotTheSame = true; break;
                    case ShaderUtil.ShaderPropertyType.Range: if( m.GetFloat( vn ) != mat.GetFloat( vn ) ) defNotTheSame = true; break;
                    case ShaderUtil.ShaderPropertyType.TexEnv: if( m.GetTexture( vn ) != mat.GetTexture( vn ) ) defNotTheSame = true; break;
                }
            }

            if( defNotTheSame ) continue;   // Definitely not the same shader, continue

            // Found the same material! Return it to be used instead of old material.
            return m;
        }
        return null;
    }
    #endregion GetIdenticalMaterialFromList()
}

