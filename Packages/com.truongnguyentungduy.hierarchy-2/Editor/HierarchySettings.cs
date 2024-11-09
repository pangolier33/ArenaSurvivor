using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Hierarchy2
{
    [Serializable]
    internal partial class HierarchySettings : ScriptableObject
    {
        public bool IsPackageAsset = true;

        [Serializable]
        public struct ThemeData
        {
            public Color colorRowEven;
            public Color colorRowOdd;
            public Color colorGrid;
            public Color colorTreeView;
            public Color colorLockIcon;
            public Color tagColor;
            public Color layerColor;
            public Color comSelBGColor;
            public Color selectionColor;
            public Color colorHeaderTitle;
            public Color colorHeaderBackground;

            public ThemeData(ThemeData themeData)
            {
                colorRowEven = themeData.colorRowEven;
                colorRowOdd = themeData.colorRowOdd;
                colorGrid = themeData.colorGrid;
                colorTreeView = themeData.colorTreeView;
                colorLockIcon = themeData.colorLockIcon;
                tagColor = themeData.tagColor;
                layerColor = themeData.layerColor;
                comSelBGColor = themeData.comSelBGColor;
                selectionColor = themeData.selectionColor;
                colorHeaderTitle = themeData.colorHeaderTitle;
                colorHeaderBackground = themeData.colorHeaderBackground;
            }

            public void BlendMultiply(Color blend)
            {
                colorRowEven = colorRowEven * blend;
                colorRowOdd = colorRowOdd * blend;
                colorGrid = colorGrid * blend;
                colorTreeView = colorTreeView * blend;
                colorLockIcon = colorLockIcon * blend;
                tagColor = tagColor * blend;
                layerColor = layerColor * blend;
                comSelBGColor = comSelBGColor * blend;
                selectionColor = selectionColor * blend;
                colorHeaderTitle = colorHeaderTitle * blend;
                colorHeaderBackground = colorHeaderBackground * blend;
            }
        }

        ///<summary>Define background color using prefix.</summary>
        [Serializable]
        public struct InstantBackgroundColor
        {
            public bool active;
            public bool useStartWith, useTag, useLayer;
            public string startWith;
            public string tag;
            public LayerMask layer;
            public Color color;
        }

        public enum ComponentSize
        {
            Small,
            Normal,
            Large
        }

        public enum ElementAlignment
        {
            AfterName,
            Right
        }

        [Flags]
        public enum ContentDisplay
        {
            Component = (1 << 0),
            Tag = (1 << 1),
            Layer = (1 << 2)
        }

        private static HierarchySettings instance;

        public ThemeData personalTheme;
        public ThemeData professionalTheme;
        public ThemeData playmodeTheme;
        private bool useThemePlaymode = false;

        public ThemeData usedTheme
        {
            get
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (useThemePlaymode == false)
                    {
                        playmodeTheme = new ThemeData(EditorGUIUtility.isProSkin ? professionalTheme : personalTheme);
                        playmodeTheme.BlendMultiply(GUI.color);
                        useThemePlaymode = true;
                    }

                    return playmodeTheme;
                }
                else
                {
                    useThemePlaymode = false;
                    return EditorGUIUtility.isProSkin ? professionalTheme : personalTheme;
                }
            }
        }

        [HideInInspector] public bool activeHierarchy = true;
        public bool displayCustomObjectIcon = true;
        public bool displayTreeView = true;
        public bool displayRowBackground = true;
        public bool displayGrid = false;
        [HideInInspector] public bool displayStaticButton = true;
        public int offSetIconAfterName = 8;
        public bool displayComponents = true;
        public ElementAlignment componentAlignment = ElementAlignment.AfterName;

        public enum ComponentDisplayMode
        {
            All = 0,
            ScriptOnly = 1,
            Specified = 2,
            Ignore = 3
        }

        public ComponentDisplayMode componentDisplayMode = ComponentDisplayMode.Ignore;
        public string[] components = new string[] { "Transform", "RectTransform" };
        public bool ignoreComponentBase;
        public string[] baseComponents = new string[] { "Command", "EventHandler" };
        [HideInInspector] public int componentLimited = 0;
        [Range(12, 16)] public int componentSize = 16;
        public int componentSpacing = 0;
        public bool displayTag = true;
        public ElementAlignment tagAlignment = ElementAlignment.AfterName;
        public bool displayLayer = true;
        public ElementAlignment layerAlignment = ElementAlignment.AfterName;
        [HideInInspector] public bool applyStaticTargetAndChild = true;
        public bool applyTagTargetAndChild = false;
        public bool applyLayerTargetAndChild = true;
        public string separatorStartWith = "--->";
        public string separatorDefaultTag = "Untagged";
        public bool useInstantBackground = false;

        public List<InstantBackgroundColor> instantBackgroundColors = new List<InstantBackgroundColor>();

        public bool onlyDisplayWhileMouseEnter = false;
        public ContentDisplay contentDisplay = ContentDisplay.Component | ContentDisplay.Tag | ContentDisplay.Layer;


        public delegate void OnSettingsChangedCallback(string param);

        public OnSettingsChangedCallback onSettingsChanged;

        public void OnSettingsChanged(string param = "")
        {
            switch (param)
            {
                case nameof(componentSize):
                    if (componentSize % 2 != 0) componentSize -= 1;
                    break;

                case nameof(componentSpacing):
                    if (componentSpacing < 0) componentSpacing = 0;
                    break;
            }

            onSettingsChanged?.Invoke(param);
            hideFlags = HideFlags.None;
        }


        private static void OnUndoRedoPerformed()
        {
            SettingsService.NotifySettingsProviderChanged();

            if (instance != null)
            {
                instance.onSettingsChanged?.Invoke(nameof(instance.components)); // Refresh components on undo & redo
            }
        }

        internal static HierarchySettings GetAssets()
        {
            if (instance != null && !instance.IsPackageAsset)
                return instance;

            var guids = AssetDatabase.FindAssets($"t:{nameof(HierarchySettings)}", new[] { "Assets" });

            for (int i = 0; i < guids.Length; i++)
            {
                instance = AssetDatabase.LoadAssetAtPath<HierarchySettings>(AssetDatabase.GUIDToAssetPath(guids[i]));
                if (instance != null)
                    return instance;
            }

            return instance = CreateAssets();
        }

        internal static HierarchySettings CreateAssets()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Editor Default Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Editor Default Resources");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            string path = "Assets/Editor Default Resources/HierarchySettings.asset";
            HierarchySettings settings =
                Instantiate(AssetDatabase.LoadAssetAtPath<HierarchySettings>(
                    "Packages/com.truongnguyentungduy.hierarchy-2/DefaultSettings/HierarchySettings.asset"));

            settings.IsPackageAsset = false;
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = settings = AssetDatabase.LoadAssetAtPath<HierarchySettings>(path);
            return settings;
        }

        internal bool ImportFromJson()
        {
            string path = EditorUtility.OpenFilePanel("Import Hierarchy 2 settings", "", "json");
            if (path.Length > 0)
            {
                string json = string.Empty;
                using (StreamReader sr = new StreamReader(path))
                {
                    json = sr.ReadToEnd();
                }

                if (string.IsNullOrEmpty(json)) return false;
                JsonUtility.FromJsonOverwrite(json, this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }

            return false;
        }

        internal TextAsset ExportToJson()
        {
            string path = EditorUtility.SaveFilePanelInProject("Export Hierarchy 2 settings as...",
                "Hierarchy 2 Settings",
                "json",
                "");

            if (path.Length > 0)
            {
                string json = JsonUtility.ToJson(instance, true);
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(json);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                Selection.activeObject = asset;
                return asset;
            }

            return null;
        }
    }

    [CustomEditor(typeof(HierarchySettings))]
    internal class SettingsInspector : Editor
    {
        HierarchySettings settings;

        void OnEnable() => settings = target as HierarchySettings;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Go to Edit -> Project Settings -> Hierarchy tab", MessageType.Info);
            if (GUILayout.Button("Open Settings"))
                SettingsService.OpenProjectSettings("Project/Hierarchy");
        }
    }
}