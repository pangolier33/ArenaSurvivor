#if UNITY_EDITOR

#if !ODIN_INSPECTOR
#undef ASSET_VARIANTS_DO_ODIN_PROPERTIES
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

namespace AssetVariants
{
    [Serializable]
    public struct AssetID // UnityEditor.Build.Content.ObjectIdentifier is problematic, needed to reimplement some of it
    {
        public enum AssetType
        {
            Main, // Shortcut
            Sub,
            Importer // AssetDatabase doesn't really consider an AssetImporter a sub asset
        }

        public string guid;
        public AssetType type;
        public long localId; // Only used if type == Type.Sub

        public Object Load()
        {
            var path = guid.ToPath();
            if (type == AssetType.Importer)
                return AssetImporter.GetAtPath(path);
            var mainAsset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (type == AssetType.Main)
                return mainAsset;
#if UNITY_2018_2_OR_NEWER // (Technically UNITY_2018_1 has TryGetGUIDAndLocalFileIdentifier, but with int localId)
            string _; long localId;
            if (mainAsset != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mainAsset, out _, out localId))
            {
                if (localId == this.localId)
                {
                    return mainAsset;
                }
                else
                {
                    var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                    for (int i = 0; i < allAssets.Length; i++)
                    {
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(allAssets[i], out _, out localId))
                            if (localId == this.localId)
                                return allAssets[i];
                    }
                    Debug.LogWarning("Could not find a sub asset for " + mainAsset + " with the localId: " + localId + Helper.LOG_END);
                }
            }
#endif
            return null;
        }

        public static AssetType GetType(Object asset)
        {
            if (AssetDatabase.IsMainAsset(asset))
                return AssetType.Main;
            else if (Helper.TypeIsImporter(asset.GetType()))
                return AssetType.Importer;
            else
                return AssetType.Sub;
        }

        public static AssetID Get(Object asset)
        {
            if (asset == null)
                return Null;
            if (AssetDatabase.IsMainAsset(asset))
                return new AssetID(asset.GUID(), AssetType.Main, 0); //11400000
            if (Helper.TypeIsImporter(asset.GetType()))
                return new AssetID(asset.GUID(), AssetType.Importer, 0); //3
#if UNITY_2018_2_OR_NEWER // (Technically UNITY_2018_1 has TryGetGUIDAndLocalFileIdentifier, but with int localId)
            string guid;
            long localId;
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localId))
                return new AssetID(guid, AssetType.Sub, localId);
#endif
            return Null;
        }

        public AssetID(string guid, AssetType type, long localId)
        {
            this.guid = guid;
            this.type = type;
            this.localId = localId;
        }

        public static readonly AssetID Null = new AssetID() { guid = null };

        public static bool operator ==(AssetID x, AssetID y)
        {
            if (string.IsNullOrWhiteSpace(x.guid))
                return string.IsNullOrWhiteSpace(y.guid);
            return x.guid == y.guid && x.localId == y.localId;
        }
        public static bool operator !=(AssetID x, AssetID y)
        {
            return !(x == y);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (type == AssetType.Importer)
                return guid + ".Importer";
            if (type == AssetType.Main)
                return guid + ".Main";
            return guid + "." + localId;
        }
    }

    public class PropertyFilter
    {
        public List<string> ignoreNameEquals = new List<string>();
        /// <summary>
        /// For performance concerns this does not support ignoring children of an array
        /// </summary>
        public List<string> ignorePathStartsWith = new List<string>()
        {
            "m_Name", // TODO: not importers?
            "m_EditorClassIdentifier", // TODO: is this only for MonoBehaviour?
            "m_Script", //TODO: for ScriptableObject only
#if ODIN_INSPECTOR
            "serializationData", //TODO: for SerializableScriptableObject only?
#endif
        };
        public List<string> ignorePathRegexPatterns = new List<string>();
        public bool ShouldIgnore(string name, string path, bool enteredArray = false)
        {
            for (int i = 0; i < ignoreNameEquals.Count; i++) // TODO2: could the names differ in Odin?
                if (name == ignoreNameEquals[i])
                    return true;

            if (path != null)
            {
                if (!enteredArray)
                {
                    for (int i = 0; i < ignorePathStartsWith.Count; i++)
                        if (path.StartsWith(ignorePathStartsWith[i]))
                            return true;
                }

                for (int i = 0; i < ignorePathRegexPatterns.Count; i++)
                    if (System.Text.RegularExpressions.Regex.IsMatch(path, ignorePathRegexPatterns[i]))
                        return true;
            }

            return false;
        }

        private static readonly Dictionary<Type, PropertyFilter> inheritedPropertyFilters = new Dictionary<Type, PropertyFilter>();
        private static PropertyFilter GetInherited(Type assetType)
        {
            PropertyFilter filter;
            if (!inheritedPropertyFilters.TryGetValue(assetType, out filter))
            {
                filter = new PropertyFilter();
                inheritedPropertyFilters.Add(assetType, filter);
            }
            return filter;
        }

        public static void InheritedIgnorePropertyName(Type assetType, string fullName)
        {
            var filter = GetInherited(assetType);
            if (!filter.ignoreNameEquals.Contains(fullName))
            {
                filter.ignoreNameEquals.Add(fullName);

                foreach (var kv in propertyFilters)
                    if (assetType.IsAssignableFrom(kv.Key))
                        kv.Value.ignoreNameEquals.Add(fullName);
            }
        }
        public static void InheritedIgnorePropertyPath(Type assetType, string absolutePathStart)
        {
            var filter = GetInherited(assetType);
            if (!filter.ignorePathStartsWith.Contains(absolutePathStart))
            {
                filter.ignorePathStartsWith.Add(absolutePathStart);

                foreach (var kv in propertyFilters)
                    if (assetType.IsAssignableFrom(kv.Key))
                        kv.Value.ignorePathStartsWith.Add(absolutePathStart);
            }
        }
        public static void InheritedIgnorePropertyPathRegex(Type assetType, string regexPattern)
        {
            var filter = GetInherited(assetType);
            if (!filter.ignorePathRegexPatterns.Contains(regexPattern))
            {
                filter.ignorePathRegexPatterns.Add(regexPattern);

                foreach (var kv in propertyFilters)
                    if (assetType.IsAssignableFrom(kv.Key))
                        kv.Value.ignorePathRegexPatterns.Add(regexPattern);
            }
        }

        public static void IgnorePropertyName(Type assetType, string fullName)
        {
            var filter = Get(assetType);
            filter.ignoreNameEquals.Remove(fullName);
            filter.ignoreNameEquals.Add(fullName);
        }
        public static void IgnorePropertyPath(Type assetType, string absolutePathStart)
        {
            if (absolutePathStart.Contains(".Array.data["))
            {
                Debug.LogWarning("Ignores created with IgnorePropertyPath() are skipped for elements in an array. This path will not be ignored: " + absolutePathStart + "\nYou can use IgnorePropertyName or IgnorePropertyPathRegex instead." + Helper.LOG_END);
                return;
            }

            var filter = Get(assetType);
            filter.ignorePathStartsWith.Remove(absolutePathStart);
            filter.ignorePathStartsWith.Add(absolutePathStart);
        }
        public static void IgnorePropertyPathRegex(Type assetType, string regexPattern)
        {
            var filter = Get(assetType);
            filter.ignorePathRegexPatterns.Remove(regexPattern);
            filter.ignorePathRegexPatterns.Add(regexPattern);
        }

        public static readonly Dictionary<Type, PropertyFilter> propertyFilters = new Dictionary<Type, PropertyFilter>();
        public static PropertyFilter Get(Type assetType)
        {
            PropertyFilter filter;
            if (!propertyFilters.TryGetValue(assetType, out filter))
            {
                filter = new PropertyFilter();

                if (Helper.TypeIsImporter(assetType))
                    filter.ignorePathStartsWith.Add("m_UserData");

                foreach (var kv in inheritedPropertyFilters)
                {
                    if (kv.Key.IsAssignableFrom(assetType))
                    {
                        filter.ignoreNameEquals.AddRange(kv.Value.ignoreNameEquals);
                        filter.ignorePathStartsWith.AddRange(kv.Value.ignorePathStartsWith);
                        filter.ignorePathRegexPatterns.AddRange(kv.Value.ignorePathRegexPatterns);
                    }
                }

                propertyFilters.Add(assetType, filter);
            }
            return filter;
        }
    }

#if ODIN_INSPECTOR
    /// <summary>
    /// Just a static class with the same name as the generic, storing its active state (whether it should return true in CanResolveForPropertyFilter).
    /// </summary>
    public static class RawPropertyResolver
    {
        /// <summary>
        /// This will be true when AVEditor is initializing/drawing, and when AV is calling RevertAssetFromParentAsset/CreateOverridesFromDifferences
        /// </summary>
        public static bool active = false;

        internal static PropertyTree currentTree = null;
        internal static AVTargets treeAVTargets = null;
    }

    /// <summary>
    /// RectPropertyResolver uses the fake property paths: .position.xy,.size.xy instead of .x,.y,.width,.height
    /// This is a problem because the paths do not match the underlying data of the field.
    /// This is a generic replacement for such problematic PropertyResolvers.
    /// It is a  ProcessedMemberPropertyResolver with [ResolverPriority(1000)]
    /// which also replaces the SerializationBackend of its children with its own
    /// (fixing what I believe is a bug in Odin where e.g. RectInt's children are normally SerializationBackend.None and therefore Asset Variants would show no override buttons for it).
    /// Simply inherit from this class like so: 
    /// public class RectRPR : RawPropertyResolver&lt;Rect&gt; { },
    /// but with another class that has a problematic OdinPropertyResolver, and it might show override buttons for it.
    /// </summary>
    [ResolverPriority(1000)] //double.MaxValue
    public abstract class RawPropertyResolver<T> : ProcessedMemberPropertyResolver<T>
    {
        public override bool CanResolveForPropertyFilter(InspectorProperty property)
        {
            if (!base.CanResolveForPropertyFilter(property))
                return false;
            if (RawPropertyResolver.active)
                return true;
            var tree = property.Tree;
            if (RawPropertyResolver.currentTree != tree)
            {
                RawPropertyResolver.currentTree = tree;
                var so = tree.UnitySerializedObject;
                if (so != null && AVFilter.Should(so.targetObject))
                    RawPropertyResolver.treeAVTargets = AVTargets.Get(so);
                else
                    RawPropertyResolver.treeAVTargets = null;
            }
            if (RawPropertyResolver.treeAVTargets == null)
                return false;
            return RawPropertyResolver.treeAVTargets.AnyHasParent
                && !RawPropertyResolver.treeAVTargets.propertyFilter.ShouldIgnore(property.Name, property.UnityPropertyPath);
        }

        protected override InspectorPropertyInfo[] GetPropertyInfos()
        {
            var sb = Property.ValueEntry.SerializationBackend;
            var infos = base.GetPropertyInfos();
            for (int i = 0; i < infos.Length; i++)
            {
                var old = infos[i];
                infos[i] = InspectorPropertyInfo.CreateValue
                    (old.PropertyName, old.Order, sb, old.GetGetterSetter(), old.Attributes);
            }
            return infos;
        }
    }

    public class RectRPR : RawPropertyResolver<Rect> { }
    public class RectIntRPR : RawPropertyResolver<RectInt> { }
    public class BoundsRPR : RawPropertyResolver<Bounds> { }
    public class BoundsIntRPR : RawPropertyResolver<BoundsInt> { }
#endif

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
    internal class Property : IDisposable
    {
        public SerializedProperty unityProperty;

        public bool IsUnity => unityProperty != null;
        public bool IsOdin => odinProperty != null;

        public InspectorProperty odinProperty;
        public int odinDepth = -1;
        private bool odinDontDispose = false;
        public bool odinIsSize = false; // Odin does not have Array/List size as a property

        public bool CompareType(Property other)
        {
            if (IsUnity)
                return Helper.PropertyTypesMatch(unityProperty.propertyType, other.unityProperty.propertyType);
            else
                return odinProperty.Info.TypeOfValue == other.odinProperty.Info.TypeOfValue;
        }

        public int depth => IsUnity ? unityProperty.depth : odinDepth;
        public string name => IsUnity ? unityProperty.name : odinProperty.Name;
        public string propertyPath
        {
            get
            {
                if (IsUnity)
                    return unityProperty.propertyPath;

                if (odinIsSize)
                    return odinProperty.UnityPropertyPath + Helper.ARRAY_SIZE_SUFFIX;
                return odinProperty.UnityPropertyPath;
            }
        }
        public bool editable => IsUnity ? unityProperty.editable : odinProperty.Info.IsEditable;

        public void Dispose()
        {
            if (IsUnity)
            {
                unityProperty.Dispose();
            }
            else
            {
                if (!odinDontDispose && odinProperty != null)
                    odinProperty.Dispose();
            }
        }

        public Property Copy(bool copyIsOwner = false)
        {
            if (IsUnity)
            {
                return new Property()
                {
                    unityProperty = unityProperty.Copy(),
                    odinProperty = null,
                };
            }

            var copy = new Property()
            {
                unityProperty = null,
                odinProperty = odinProperty,
                odinIsSize = odinIsSize,
                odinDepth = odinDepth,
                odinDontDispose = true
            };
            if (copyIsOwner)
            {
                copy.odinDontDispose = odinDontDispose;
                odinDontDispose = true;
            }
            return copy;
        }

        public void FindDepth()
        {
            if (IsOdin)
            {
                odinDepth = 0;
                var path = odinProperty.Path;
                int l = path.Length;
                for (int i = 0; i < l; i++)
                {
                    if (path[i] == '.')
                        odinDepth++;
                }
            }
        }

        public bool NextVisible(bool enterChildren, bool skipInternalOnInspectorGUI = false)
        {
            if (IsUnity)
                return unityProperty.NextVisible(enterChildren);

            if (!OdinNextProperty(enterChildren, true))
                return false;
            if (skipInternalOnInspectorGUI && propertyPath == "InternalOnInspectorGUI")
            {
                if (!OdinNextProperty(enterChildren, true))
                    return false;
            }
            return true;
        }
        public bool Next(bool enterChildren)
        {
            if (IsUnity)
                return unityProperty.Next(enterChildren);
            return OdinNextProperty(enterChildren, false);
        }
        public bool NextChildSkipArray()
        {
            if (IsUnity)
                return unityProperty.NextChildSkipArray();
            return OdinNextProperty(true, false);
        }
        private bool OdinNextProperty(bool enterChildren, bool visibleOnly = true)
        {
            if (enterChildren && !odinIsSize && odinProperty.IsCollectionWithSize())
            {
                // TODO2: no odin serialized collection has an actual size property right?
                odinDepth++;
                odinIsSize = true;
                return true;
            }
            else
            {
                var prev = odinProperty;
                if (odinIsSize)
                {
                    odinIsSize = false;
                    odinProperty = odinProperty.NextProperty(true, visibleOnly); // Moves to children, because property is stored as the parent property when isSize
                    if (odinProperty != null && odinProperty.Parent != prev)
                        FindDepth();
                }
                else if (enterChildren && Helper.HasChildren(this))
                {
                    odinDepth++;
                    odinProperty = odinProperty.NextProperty(true, visibleOnly);
                }
                else
                {
                    odinProperty = odinProperty.NextProperty(false, visibleOnly);
                    if (odinProperty != null && odinProperty.Parent != prev.Parent)
                        FindDepth();
                }
                if (!odinDontDispose)
                    prev.Dispose();
                else
                    odinDontDispose = false;
                return odinProperty != null;
            }
        }
    }
#endif

    internal class OverridesCache // Uses a HashSet to optimize when checking if a property has an override or not
    {
        public List<string> list;
        public HashSet<string> hashSet = new HashSet<string>();

        public HashSet<string> parentPaths = new HashSet<string>();

        public bool Contains(string path)
        {
            return hashSet.Contains(path);
        }

        private void AddParentPathsTo(string path, HashSet<string> paths)
        {
            while (true)
            {
                int index = path.LastIndexOf('.');
                if (index == -1)
                    break;
                path = path.Substring(0, index);
                if (!paths.Add(path)) // Already been added, therefore further parents are also already added.
                    break;
            }
        }
        private void FixParentPaths(string path)
        {
            int index = -1;
            while (true)
            {
                index = path.IndexOf('.', index + 1);
                if (index == -1)
                    break;
                string parentPath = path.Substring(0, index);
                if (!ContainsChildrenPaths(parentPath, false))
                {
                    if (!parentPaths.Remove(parentPath)) // Doesn't exist, therefore children paths also shouldn't exist.
                        break;
                }
            }
        }

        public bool Add(string path)
        {
            if (hashSet.Add(path))
            {
                list.Add(path);
                AddParentPathsTo(path, parentPaths);
                return true;
            }
            else
            {
                Debug.LogWarning("Override already exists: " + path + Helper.LOG_END);
                return false;
            }
        }
        public bool Remove(string path)
        {
            list.Remove(path);
            if (hashSet.Remove(path))
            {
                FixParentPaths(path);
                return true;
            }
            return false;
        }

        public void Init(List<string> list)
        {
            this.list = list;

            hashSet.Clear();
            parentPaths.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                string path = list[i];

                if (!hashSet.Add(path))
                {
                    Debug.LogWarning("Removed duplicate override: " + list[i] + Helper.LOG_END);
                    list.RemoveAt(i);
                    i--;

                    //TODO: add bool dirty to this class
                }
                else
                {
                    AddParentPathsTo(path, parentPaths);
                }
            }
        }

        private static readonly HashSet<string> dirtyParentPaths = new HashSet<string>();
        public bool RemoveChildrenPaths(string path) //RemoveSubOverrides
        {
            bool dirty = false;

            dirtyParentPaths.Clear();

            string childStart = path + "."; // Albeit odin serialized children of unity serialized properties might not be matched
            for (int i = list.Count - 1; i >= 0; i--)
            {
                string o = list[i];
                if (o.StartsWith(childStart))
                {
                    list.RemoveAt(i);
                    hashSet.Remove(o);
                    if (Helper.LogUnnecessary)
                        Debug.Log("Removed child override: " + o + Helper.LOG_END);
                    dirty = true;
                    parentPaths.Remove(o); // For optimization
                    AddParentPathsTo(o, dirtyParentPaths);
                }
            }
            if (dirty)
            {
                foreach (string p in dirtyParentPaths)
                {
                    if (parentPaths.Contains(p) && !ContainsChildrenPaths(p, false))
                        parentPaths.Remove(p);
                }
            }

            return dirty;
        }

        public bool ContainsChildrenPaths(string path, bool includeSelf = true) //ContainsSubOverrides
        {
            if (includeSelf && Contains(path))
                return true;

            string childStart = path + ".";
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StartsWith(childStart))
                    return true;
            }

            return false;
        }
        public bool FastContainsChildrenPaths(string path, bool includeSelf = true) //ContainsSubOverrides
        {
            if (includeSelf && hashSet.Contains(path))
                return true;
            if (parentPaths.Contains(path))
                return true;
            return false;
        }

        public bool IsOverridden(string path)
        {
            while (true)
            {
                if (Contains(path))
                    return true;

                int index = path.LastIndexOf('.');
                if (index == -1)
                    return false;
                path = path.Substring(0, index);
                //if (path.Length == 0)
                //    return false;
            }
        }
    }
}

#endif
