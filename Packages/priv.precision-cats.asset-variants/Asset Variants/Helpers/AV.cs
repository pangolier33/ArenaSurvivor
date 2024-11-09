#if UNITY_EDITOR

#if !ODIN_INSPECTOR
#undef ASSET_VARIANTS_DO_ODIN_PROPERTIES
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using PrecisionCats;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using S = AssetVariants.AVSettings;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
#endif

namespace AssetVariants
{
    internal class AV : IDisposable
    {
        public readonly AssetID assetID;
        public readonly Object asset;
        public AssetImporter Importer { get; private set; }

        public WeakReference<Object> cachedParent;

        public string UserDataKey
        {
            get
            {
                if (assetID.type == AssetID.AssetType.Main)
                    return "AssetVariantData.Main";
                else if (assetID.type == AssetID.AssetType.Importer)
                    return "AssetVariantData.Importer";
                else
                    return "AssetVariantData." + assetID.localId;
            }
        }
        public Object LoadParentAsset()
        {
            if (data.HasParent)
            {
                Object p;
                if (cachedParent != null && cachedParent.TryGetTarget(out p) && p != null)
                    return p;
                p = data.parent.Load();
                cachedParent = new WeakReference<Object>(p);
                return p;
            }
            return null;
        }

        private bool TryRenameOverride(string path, string name, string oldName)
        {
            bool dirty = false;

            string oldPath = path.Substring(0, path.Length - name.Length) + oldName;
            if (overridesCache.FastContainsChildrenPaths(oldPath, true))
            {
                string oldPathDot = oldPath + ".";

                int oldPathL = oldPath.Length;
                for (int i = 0; i < overridesCache.list.Count; i++)
                {
                    var o = overridesCache.list[i];
                    if (o == oldPath || o.StartsWith(oldPathDot, StringComparison.Ordinal))
                    {
                        var newO = path + o.Substring(oldPathL, o.Length - oldPathL);
                        Debug.Log("Renaming override: " + o + "\nto: " + newO + "\nbecause of the FormerlySerializedAsAttribute at:\n" + oldPath + Helper.LOG_END);
                        overridesCache.Remove(o);
                        overridesCache.Add(newO);
                        dirty = true;
                    }
                }
            }

            return dirty;
        }

        #region Renaming FormerlySerializedAs
        private static readonly HashSet<Object> alreadyRenamedOverrides = new HashSet<Object>();
        private bool MightHaveFSA()
        {
            //TODO: move this to shouldRenameOverrides?
            Type type = asset.GetType();
            for (int i = 0; i < S.S.formerlySerializedAsBaseAssetTypes.Length; i++)
            {
                var baseType = AVCommonHelper.FindType(S.S.formerlySerializedAsBaseAssetTypes[i]);
                if (baseType != null && baseType.IsAssignableFrom(type))
                {
                    return true;
                }
            }
            return false;
        }
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
        private void RenameOverrides(PropertyTree odinDst)
        {
#if DEBUG_ASSET_VARIANTS
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif

            bool dirty = false;

            var odinProp = odinDst.RootProperty();
            if (odinProp != null)
            {
                while (true)
                {
                    var prevProp = odinProp;
                    odinProp = odinProp.NextProperty(true, false); // TODO2: skip InternalOnInspectorGUI?
                    prevProp.Dispose();
                    if (odinProp == null)
                        break;

                    if (SkipDuplicate(odinProp))
                        break;

                    if (odinProp.Info.SerializationBackend == SerializationBackend.Odin)
                    {
                        foreach (var attribute in odinProp.Info.Attributes)
                        {
                            if (attribute is FormerlySerializedAsAttribute)
                            {
                                var fsa = (FormerlySerializedAsAttribute)attribute;
                                if (TryRenameOverride(odinProp.UnityPropertyPath, odinProp.Name, fsa.oldName))
                                    dirty = true;
                            }
                        }
                    }
                }
            }

#if DEBUG_ASSET_VARIANTS
            Debug.Log("Odin renaming duration: " + sw.Elapsed.TotalMilliseconds);
#endif

            if (dirty)
                SaveUserDataWithUndo();
        }
#endif
        private void RenameProp(SerializedProperty prop, ref bool dirty)
        {
            //Debug.Log("RenameProp: " + prop.propertyPath + " " + prop.hasChildren);

            int prevDepth;
            if (!ManagedReferencesHelper.SkipDuplicate(prop, out prevDepth))
            {
                R.args[0] = prop;
                R.args[1] = null; // out Type type
                var fieldInfo = R.getFieldInfoFromPropertyMI.Invoke(null, R.args) as FieldInfo;

                if (fieldInfo != null)
                {
                    foreach (var fsa in fieldInfo.GetCustomAttributes<FormerlySerializedAsAttribute>())
                    {
                        //Debug.Log(prop.propertyPath + " " + fsa.oldName);
                        if (TryRenameOverride(prop.propertyPath, prop.name, fsa.oldName))
                            dirty = true;
                    }
                }
                //else
                //Debug.Log("FieldInfo for: " + prop.propertyPath + " is null");

                if (prop.hasChildren)
                {
                    int depth = prop.depth;
                    var childrenIterator = prop.Copy();
                    if (childrenIterator.NextChildSkipArray())
                    {
                        while (depth < childrenIterator.depth)
                        {
                            RenameProp(childrenIterator, ref dirty);

                            if (!childrenIterator.Next(false))
                                break;
                        }
                    }
                    childrenIterator.Dispose();
                }
            }

            ManagedReferencesHelper.RestoreSkipDuplicateDepth(prevDepth);
        }
        private void RenameOverrides(SerializedObject dst)
        {
#if DEBUG_ASSET_VARIANTS
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif

            bool dirty = false;

            var prop = dst.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    RenameProp(prop, ref dirty);
                }
                while (prop.Next(false));
            }
            prop.Dispose();

#if DEBUG_ASSET_VARIANTS
            Debug.Log("Renaming duration: " + sw.Elapsed.TotalMilliseconds);
#endif

            if (dirty)
                SaveUserDataWithUndo();
        }
        #endregion

        private PropertyFilter srcFilter, dstFilter;

        #region Odin AttributeProcessor
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
        private class RemoveAttributeProcessor : OdinAttributeProcessor
        {
            public static bool active = false;

            public override bool CanProcessSelfAttributes(InspectorProperty property)
            {
                return active;
            }
            public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
            {
                attributes.Clear(); // TODO2:

                //for (int i = attributes.Count - 1; i >= 0; i--)
                //{
                //    Debug.Log(attributes[i]);
                //    //if (typeof(PropertyGroupAttribute).IsAssignableFrom(attributes[i].GetType()))
                //    //    attributes.RemoveAt(i);
                //}
            }
        }
        private static readonly RemoveAttributeProcessor removeAttributeProcessor = new RemoveAttributeProcessor();

        private class SkipAttributeProcessorLocator : OdinAttributeProcessorLocator
        {
            public override List<OdinAttributeProcessor> GetChildProcessors(InspectorProperty parentProperty, MemberInfo member)
            {
                return new List<OdinAttributeProcessor>();
            }
            public override List<OdinAttributeProcessor> GetSelfProcessors(InspectorProperty property)
            {
                return new List<OdinAttributeProcessor>() { removeAttributeProcessor };
            }
        }
        private static readonly SkipAttributeProcessorLocator skipAttributeProcessorLocator = new SkipAttributeProcessorLocator();
#endif
        #endregion

        public void PrepareImplicit()
        {
            AV parentAV;
            if (openAVs.TryGetValue(data.parent, out parentAV))
                parentAV.FlushAnyChangesToChildren();
        }
        public SerializedObject implicitSrc, implicitDst;
        private void PropCreateImplicit(SerializedObject src, PropertyFilter filter, SerializedProperty dstProp, ref SerializedProperty srcProp)
        {
            int prevDepth;
            if (!ManagedReferencesHelper.SkipDuplicate(dstProp, out prevDepth))
            {
                string path = dstProp.propertyPath; //Debug.Log(path);

                if (dstProp.editable && !filter.ShouldIgnore(dstProp.name, path))
                {
                    bool hasChildren = Helper.HasChildren(dstProp);

                    if (srcProp == null)
                    {
                        srcProp = src.FindProperty(path);
                    }
                    else if (srcProp.propertyPath != path)
                    {
                        srcProp.Dispose();
                        srcProp = src.FindProperty(path);
                    }

                    bool srcExists = srcProp != null;

                    bool never = S.S.missingSourceImplicitCreationType == S.MissingSourceImplicitCreationType.NeverCreateOverride;
                    if (never ? (srcExists && !hasChildren) : (!srcExists || !hasChildren))
                    {
                        if (!srcExists || !Helper.AreEqual(dstProp, srcProp))
                        {
                            if (data.HasParent && !overridesCache.IsOverridden(path))
                            {
                                overridesCache.Add(path);
                                SaveUserDataWithUndo();
                                //overridesToggled = true;
                                if (Helper.LogUnnecessary)
                                    //Debug.Log(asset.name + " - " + AVUpdater.timestamp + "Implicitly created override for: " + path + Helper.LOG_END);
                                    Debug.Log("Implicitly created override for: " + path + Helper.LOG_END);

                                OverrideIndicator.OverridesMaybeChanged();
                            }
                        }
                    }

                    if (hasChildren && srcExists)
                    {
                        var childDstIterator = dstProp.Copy();
                        if (!childDstIterator.isArray || childDstIterator.Next(true)) // Skip .Array
                        {
                            if (childDstIterator.Next(true)) // Enter children
                            {
                                int depth = dstProp.depth;
                                while (depth < childDstIterator.depth)
                                {
                                    PropCreateImplicit(src, filter, childDstIterator, ref srcProp);

                                    if (!childDstIterator.Next(false))
                                        break;
                                    if (srcProp != null && !srcProp.Next(false))
                                    {
                                        srcProp.Dispose();
                                        srcProp = null;
                                    }
                                }
                            }
                        }
                        childDstIterator.Dispose();
                    }
                }
            }

            ManagedReferencesHelper.RestoreSkipDuplicateDepth(prevDepth);
        }
        public void CreateImplicitOverrides(List<string> specificPaths)
        {
#if DEBUG_ASSET_VARIANTS
            Debug.Log("Searching Implicit" + Helper.LOG_END);
#endif

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            if (asset is SerializedScriptableObject)
            {
                //TODO: an actual odin implementation
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();
                CreateOverridesFromDifferences();
                //Debug.Log(sw.Elapsed.TotalMilliseconds);
                return;
            }
#endif

            var parent = LoadParentAsset();
            if (parent == null)
            {
                Debug.LogError("parent == null");
                return;
            }

            if (implicitSrc == null || implicitSrc.targetObject != parent)
                implicitSrc = new SerializedObject(parent);
            else
                implicitSrc.UpdateIfRequiredOrScript();

            if (implicitDst == null || implicitDst.targetObject != asset)
                implicitDst = new SerializedObject(asset);
            else
                implicitDst.UpdateIfRequiredOrScript();

            var filter = PropertyFilter.Get(asset.GetType());

            GetOverridesCache();

            SerializedProperty srcProp = null;
            if (specificPaths != null)
            {
                for (int i = 0; i < specificPaths.Count; i++)
                {
                    //Debug.Log("specificPath: " + specificPaths[i]);
                    var dstProp = implicitDst.FindProperty(specificPaths[i]);
                    if (dstProp == null)
                    {
                        Debug.LogWarning("");
                        continue;
                    }

                    ManagedReferencesHelper.InitSkipDuplicates();
                    PropCreateImplicit(implicitSrc, filter, dstProp, ref srcProp);

                    if (srcProp != null)
                    {
                        srcProp.Dispose();
                        srcProp = null;
                    }

                    dstProp.Dispose();
                }
            }
            else
            {
                ManagedReferencesHelper.InitSkipDuplicates();

                var dstIterator = implicitDst.GetIterator();
                if (dstIterator.NextVisible(true)) //?
                {
                    while (true)
                    {
                        PropCreateImplicit(implicitSrc, filter, dstIterator, ref srcProp);

                        if (!dstIterator.Next(false))
                            break;
                        if (srcProp != null && !srcProp.Next(false))
                        {
                            srcProp.Dispose();
                            srcProp = null;
                        }
                    }
                }
                dstIterator.Dispose();
            }

            if (srcProp != null)
                srcProp.Dispose();
        }
        public void CreateImplicitOverrides()
        {
            PrepareImplicit();

            CreateImplicitOverrides(null);

            Action<AV> ci;
            if (customCreateImplicitOverrides.TryGetValue(asset.GetType(), out ci))
            {
                if (ci != null)
                    ci(this);
            }
        }
        public static readonly Dictionary<Type, Action<AV>> customCreateImplicitOverrides = new Dictionary<Type, Action<AV>>();

        public void TryDirtyImplicit()
        {
            if (!data.HasParent)
                return;

            if (!needsToCreateImplicitOverrides)
            {
                needsToCreateImplicitOverrides = true;
                EditorApplication.update += UpdateFlushImplicit;

                FlushImplicit(false);
            }
        }
        private bool needsToCreateImplicitOverrides = false;
        private void UpdateFlushImplicit()
        {
            if (!needsToCreateImplicitOverrides)
            {
                EditorApplication.update -= UpdateFlushImplicit;
                Debug.LogWarning("?");
            }
            FlushImplicit(false);
        }
        public double implicitTimestamp = double.MinValue;
        public void FlushImplicit(bool force)
        {
            if (needsToCreateImplicitOverrides)
            {
                double now = EditorApplication.timeSinceStartup;
                if (force || (now - implicitTimestamp) >= S.S.implicitOverrideCreationInterval)
                {
                    EditorApplication.update -= UpdateFlushImplicit;
                    needsToCreateImplicitOverrides = false;

                    implicitTimestamp = now;

                    CreateImplicitOverrides();
                }
            }
        }

        public static readonly WeakTable<Object, int> revertingTimestamps = new WeakTable<Object, int>();

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
        private void RevertProp(Property srcProp, Property parentDstProp, ref Property dstPropGuess, out bool found, bool odinPass, bool enteredArray, ref int changes, Property dstProp = null)
        {
#else
        private void RevertProp(SerializedProperty srcProp, SerializedProperty parentDstProp, ref SerializedProperty dstPropGuess, out bool found, bool odinPass, bool enteredArray, ref int changes, SerializedProperty dstProp = null)
        {
#endif

            //if (odinPass)
            //    Debug.Log("RevertProp Odin: " + srcProp.odinProperty.UnityPropertyPath); // + " - " + srcProp.odinProperty.Path);

            int prevDepth;
            if (SkipDuplicate(srcProp, odinPass, out prevDepth))
            {
                // Not quite right, but good enough
                found = true;
                goto Exit;
            }

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            if (odinPass && srcProp.odinProperty.Info.PropertyType != PropertyType.Value)
            {
                Debug.LogWarning("Something went wrong");
                found = true;
                goto Exit;
            }
#endif

            found = false;

            string testPath = srcProp.propertyPath;

            if (overridesCache.Contains(testPath))
                goto Exit;
            string name = srcProp.name;
            if (srcFilter.ShouldIgnore(name, testPath, enteredArray) || dstFilter.ShouldIgnore(name, testPath, enteredArray))
                goto Exit;

            if (parentDstProp != null)
                dstProp = parentDstProp.FindChild(srcProp, dstPropGuess);
            if (dstProp != null && dstProp.editable)
            {
                bool currentlyInvalid = false;
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                currentlyInvalid = Helper.CurrentlyInvalid(srcProp, dstProp, odinPass);
#endif

                //Debug.Log("Found Dst: " + testPath + " - currentlyInvalid: " + currentlyInvalid);

                found = true;

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                bool dstHasChildren = Helper.HasChildren(dstProp);
                bool dstCanHaveChildren = dstHasChildren ||
                    (odinPass && !dstProp.odinIsSize && dstProp.odinProperty.IsCollection()); // Dictionary and HashSet need .IsCollection

                // So that a null collection will be copied as a whole
                if (dstCanHaveChildren && odinPass)
                {
                    if (srcProp.odinProperty.ValueEntry.WeakValues[0] == null)
                        dstCanHaveChildren = false;
                    else if (dstProp.odinProperty.ValueEntry.WeakValues[0] == null)
                        dstCanHaveChildren = false;
                }

                bool matchesType = dstProp.CompareType(srcProp);
#else
                bool dstCanHaveChildren = Helper.HasChildren(dstProp);
                bool matchesType = Helper.PropertyTypesMatch(dstProp.propertyType, srcProp.propertyType); //TODO2: compare .type and .arrayElementType?
#endif
                if (!currentlyInvalid && !dstCanHaveChildren && matchesType) // Children might have their own overrides, so children are copied one by one
                {
                    if (odinPass)
                    {
                        #region Copies Data
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                        if (srcProp.odinIsSize)
                        {
                            int size = srcProp.odinProperty.Children.Count;
                            int dstSize = dstProp.odinProperty.Children.Count;
                            if (dstSize != size)
                            {
                                changes++;

                                // Resizes
                                var dstValues = dstProp.odinProperty.ValueEntry.WeakValues;

                                var dstType = dstProp.odinProperty.Info.TypeOfValue;
                                if (dstType.IsArray)
                                {
                                    // TODO2: this doesn't need undo right?
                                    dstValues[0] = Helper.ResizeArray(dstValues[0], size);
                                }
                                else
                                {
                                    dstProp.odinProperty.RecordForUndo("Resize Array/List/Queue/Stack");

                                    // List<>, Queue<>, or Stack<>
                                    var collection = (ICollection)dstValues[0];

                                    var type = collection.GetType();

                                    if (collection.Count < size)
                                    {
                                        object newElement = null;
                                        foreach (var o in collection)
                                            newElement = o; // Finds the last element
                                        if (newElement == null)
                                        {
                                            Type elementType = type.GetGenericArguments()[0];
                                            if (elementType.IsValueType)
                                                newElement = Activator.CreateInstance(elementType);
                                            else
                                                newElement = null;
                                        }

                                        //if (type.InheritsFrom(typeof(List<>)))
                                        if (typeof(IList).IsAssignableFrom(type))
                                        {
                                            var list = (IList)collection;
                                            while (list.Count < size)
                                                list.Add(newElement);
                                        }
                                        else
                                        {
                                            object[] arg = new object[1] { newElement };

                                            MethodInfo grow = null;
                                            if (type.InheritsFrom(typeof(Queue<>)))
                                                // This will however shift the indices in the editor
                                                grow = type.GetMethod("Enqueue", BindingFlags.Instance | BindingFlags.Public);
                                            else //if (type.InheritsFrom(typeof(Stack<>)))
                                                grow = type.GetMethod("Push", BindingFlags.Instance | BindingFlags.Public);

                                            while (collection.Count < size)
                                                grow.Invoke(collection, arg);
                                        }
                                    }
                                    else
                                    {
                                        //if (type.InheritsFrom(typeof(List<>)))
                                        if (typeof(IList).IsAssignableFrom(type))
                                        {
                                            var list = (IList)collection;
                                            while (list.Count > size)
                                                list.RemoveAt(list.Count - 1);
                                        }
                                        else
                                        {
                                            MethodInfo shrink = null;
                                            if (type.InheritsFrom(typeof(Queue<>)))
                                                // This will however shift the indices in the editor
                                                shrink = type.GetMethod("Dequeue", BindingFlags.Instance | BindingFlags.Public);
                                            else //if (type.InheritsFrom(typeof(Stack<>)))
                                                shrink = type.GetMethod("Pop", BindingFlags.Instance | BindingFlags.Public);

                                            while (collection.Count > size)
                                                shrink.Invoke(collection, null);
                                        }
                                    }

                                    dstProp.odinProperty.ChildResolver.ForceUpdateChildCount();
                                }
                            }
                        }
                        else
                        {
                            //Debug.Log("RevertProp Odin: " + srcProp.odinProperty.UnityPropertyPath); // + " - " + srcProp.odinProperty.Path);

                            // TODO2: does this work?
                            if (!Helper.AreEqual(dstProp.odinProperty, srcProp.odinProperty)) //?
                            {
                                //Debug.Log(dstProp.odinProperty.ValueEntry.WeakValues[0] + " - " + srcProp.odinProperty.ValueEntry.WeakValues[0]);

                                dstProp.odinProperty.ValueEntry.WeakValues[0] =
                                    Helper.DeepCopy(srcProp.odinProperty.ValueEntry.WeakValues[0]); //dstProp.property.ValueEntry.WeakSmartValue = srcProp.property.ValueEntry.WeakSmartValue; // I'm pretty sure this is a shallow copy // Is this a deep copy or shallow copy?
                                changes++;
                            }
                            //else
                            //Debug.Log("Same: " + dstProp.odinProperty.ValueEntry.WeakValues[0] + " " + srcProp.odinProperty.ValueEntry.WeakValues[0]);
                        }
#endif
                        #endregion
                    }
                    else
                    {
                        if (testPath.CustomEndsWith(Helper.ARRAY_SIZE_SUFFIX))
                        {
                            // Prevents crash caused by invalid iterator
                            int srcCount = srcProp.ToSP().intValue;
                            int dstCount = dstProp.ToSP().intValue;

                            if (srcCount != dstCount)
                            {
                                SerializedProperty arraySP;
                                bool disposeArraySP;

                                if (parentDstProp != null)
                                {
                                    arraySP = parentDstProp.ToSP();
                                    disposeArraySP = false;
                                }
                                else
                                {
                                    // This only occurs from RevertPropertiesFromParentAsset
                                    // which sends null as parentDstProp
                                    var arrayPath = Helper.GetArrayPath(testPath);
                                    arraySP = dstProp.ToSP().serializedObject.FindProperty(arrayPath);
                                    disposeArraySP = true;
                                }

                                arraySP.arraySize = srcCount;
                                changes += Mathf.Abs(srcCount - dstCount); //? //changes++;

                                if (disposeArraySP)
                                    arraySP.Dispose();
                            }
                        }
                        else
                        {
                            if (Helper.CopyValueRecursively(dstProp.ToSP(), srcProp.ToSP()))
                                changes++;
                        }
                    }
                }
                else
                {
                    #region Removing mismatching from Odin HashSet/Dictionary
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                    bool fixMismatch = false;

                    bool isHashSet = odinPass && !currentlyInvalid && Helper.IsHashSet(dstProp.odinProperty);
                    bool isDictionary = odinPass && !currentlyInvalid && Helper.IsDictionary(dstProp.odinProperty);

                    if (!dstProp.odinIsSize && matchesType && (isHashSet || isDictionary))
                    {
                        if (isHashSet)
                            fixMismatch = true;
                        else if (isDictionary)
                        {
                            string sizePath = testPath + ".DictionarySize";
                            fixMismatch = !overridesCache.Contains(sizePath);
                        }
                        if (fixMismatch && dstHasChildren)
                        {
                            //Debug.Log("fixMismatch " + dstProp.propertyPath);

                            var value = dstProp.odinProperty.ValueEntry.WeakValues[0];
                            var dictionary = value as IDictionary;
                            MethodInfo hashSetRemove = null;
                            if (isHashSet)
                                hashSetRemove = value.GetType().GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);

                            bool dirty = false;

                            var dstChildrenIterator = dstProp.Copy();
                            int depth = dstChildrenIterator.depth;
                            Property childSrcPropGuess = srcProp.Copy();
                            if (!childSrcPropGuess.Next(true))
                                childSrcPropGuess = null;
                            if (dstChildrenIterator.Next(true))
                            {
                                while (depth < dstChildrenIterator.depth)
                                {
                                    var childSrcProp = srcProp.FindChild(dstChildrenIterator, childSrcPropGuess);
                                    if (childSrcProp == null)
                                    {
                                        if (!dirty)
                                        {
                                            dirty = true;
                                            changes++;
                                            dstProp.odinProperty.RecordForUndo("Remove Dictionary/HashSet Element");
                                        }

                                        object element = dstChildrenIterator.odinProperty.ValueEntry.WeakValues[0];

                                        if (isHashSet)
                                        {
                                            hashSetRemove.Invoke(value, new object[] { element });
                                        }
                                        else
                                        {
                                            element = element.GetType().GetField("Key").GetValue(element);
                                            dictionary.Remove(element);
                                        }
                                    }
                                    childSrcPropGuess = childSrcProp;

                                    if (!dstChildrenIterator.Next(false))
                                        break;
                                    if (childSrcPropGuess != null && !childSrcPropGuess.Next(false))
                                        childSrcPropGuess = null;
                                }
                            }
                            if (childSrcPropGuess != null)
                                childSrcPropGuess.Dispose();
                            dstChildrenIterator.Dispose();

                            if (dirty)
                            {
                                dstProp.odinProperty.ChildResolver.ForceUpdateChildCount();
                                dstProp.odinProperty.RefreshSetup();
                                EditorUtility.SetDirty(asset);
                            }
                        }
                    }
#endif
                    #endregion

                    bool copyChildren = dstCanHaveChildren
                        && Helper.HasChildren(srcProp);
                    if (copyChildren && (matchesType || !S.S.propertyTypesNeedToMatch))
                    {
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                        if (odinPass ? dstProp.odinProperty.IsCollection() : dstProp.unityProperty.isArray) enteredArray = true;
#else
                        if (dstProp.isArray) enteredArray = true;
#endif

                        using (var srcChildrenIterator = srcProp.Copy())
                        {
                            int depth = srcChildrenIterator.depth;

                            srcChildrenIterator.NextChildSkipArray();

                            var childDstPropGuess = dstPropGuess;
                            if (childDstPropGuess != null)
                            {
                                childDstPropGuess = childDstPropGuess.Copy();
                                if (!childDstPropGuess.NextChildSkipArray())
                                    childDstPropGuess = null;
                            }

                            do
                            {
                                bool foundChild;
                                RevertProp(srcChildrenIterator, dstProp, ref childDstPropGuess, out foundChild, odinPass, enteredArray, ref changes);

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                                #region Dictionary/HashSet add missing element
                                if (fixMismatch && !foundChild && (isDictionary || isHashSet))
                                {
                                    object srcValue = srcChildrenIterator.odinProperty.ValueEntry.WeakValues[0];

                                    // TODO2: find out the proper Odin way to append elements?

                                    BindingFlags pi = BindingFlags.Public | BindingFlags.Instance;

                                    dstProp.odinProperty.RecordForUndo("Add missing element from Dictionary/HashSet");

                                    var collection = dstProp.odinProperty.ValueEntry.WeakValues[0];

                                    if (isDictionary)
                                    {
                                        // SrcValue is EditableKeyValuePair
                                        var kvPairType = srcValue.GetType();
                                        var kvKeyPI = kvPairType.GetField("Key", pi);
                                        var kvValuePI = kvPairType.GetField("Value", pi);
                                        var dictionary = (IDictionary)collection;
                                        dictionary.Add(Helper.DeepCopy(kvKeyPI.GetValue(srcValue)),
                                            Helper.DeepCopy(kvValuePI.GetValue(srcValue)));
                                    }
                                    else //if (isHashSet)
                                    {
                                        object[] arg = new object[] { Helper.DeepCopy(srcValue) };
                                        collection.GetType().GetMethod("Add", pi).Invoke(collection, arg);
                                    }

                                    changes++;

                                    dstProp.odinProperty.ChildResolver.ForceUpdateChildCount();
                                    EditorUtility.SetDirty(asset);
                                }
                                #endregion
#endif

                                if (!srcChildrenIterator.Next(false))
                                    break;
                                if (childDstPropGuess != null && !childDstPropGuess.Next(false))
                                    childDstPropGuess = null;
                            }
                            while (depth < srcChildrenIterator.depth);

                            if (childDstPropGuess != null)
                                childDstPropGuess.Dispose();
                        }
                    }
                }
            }
            else
                found = false;

            Exit:

            ManagedReferencesHelper.RestoreSkipDuplicateDepth(prevDepth);

            dstPropGuess = dstProp;
        }
        public bool RevertAssetFromParentAsset(List<string> propertyPaths = null, AV selfParent = null)
        {
            if (asset == null)
            {
                Debug.LogWarning("Asset is null!" + Helper.LOG_END);
            }

            if (S.disablePropagation(asset))
            {
                Debug.LogWarning("Propagation is disabled for: " + asset.name + Helper.LOG_END);
                return false;
            }

            if (propertyPaths != null && propertyPaths.Count == 0)
                return false;

            if (!this.Valid())
                return false;

            var parentAsset = LoadParentAsset();
            if (parentAsset == null || parentAsset is DefaultAsset)
            {
                if (selfParent == null || selfParent.assetID != data.parent)
                {
                    Debug.LogError("RevertAssetFromParentAsset called for child asset with no parent or a reimporting/DefaultAsset parent. Child in question: " + asset.Path() + Helper.LOG_END);
                    return false;
                }
                parentAsset = selfParent.asset;
                Debug.LogWarning("\n");
            }

            GetOverridesCache();
            srcFilter = PropertyFilter.Get(parentAsset.GetType());
            dstFilter = PropertyFilter.Get(asset.GetType());

            bool renameOverrides = S.shouldRenameOverrides(asset) && MightHaveFSA() && !alreadyRenamedOverrides.Contains(asset); // Prevents redundant checks until domain is reloaded or undo
            alreadyRenamedOverrides.Add(asset);

            int changes = 0;

            bool _;

            bool alreadyRecordedUndo = false;

            revertingTimestamps.Remove(asset);
            revertingTimestamps.Add(asset, AVUpdater.timestamp);

            #region Odin Pass
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            if (asset.IsOdinSerializable())
            {
                RemoveAttributeProcessor.active = true;
                RawPropertyResolver.active = true;

                var odinSrc = PropertyTree.Create(parentAsset);
                var odinDst = PropertyTree.Create(asset);
                odinSrc.AttributeProcessorLocator = skipAttributeProcessorLocator;
                odinDst.AttributeProcessorLocator = skipAttributeProcessorLocator; //odinSrc.StateUpdaterLocator //odinSrc.SetSearchable //odinSrc.SetUpForValidation(); //odinDst.SetUpForValidation();

                if (renameOverrides)
                {
                    InitSkipDuplicates();
                    RenameOverrides(odinDst);
                }

#if ODIN_INSPECTOR_3
                bool prevRUFC = odinDst.RecordUndoForChanges;
                odinDst.RecordUndoForChanges = true;
#endif

                if (propertyPaths == null)
                {
                    InitSkipDuplicates();

                    var outerSrcIterator = odinSrc.GetOdinIterator();
                    var outerDstIterator = odinDst.GetOdinIterator();
                    Property dstPropGuess = null;
                    if (outerSrcIterator.NextVisible(true, true))
                    {
                        while (true)
                        {
                            // Originally I thought that Odin could serialize fields serializable only by Odin that are children of unity serialized fields.
                            // But that is not the case.
                            // Just skips the entire property if it's serialized by Unity.
                            if (!Helper.IsUnitySerialized(outerSrcIterator.odinProperty))
                                RevertProp(outerSrcIterator, outerDstIterator, ref dstPropGuess, out _, true, false, ref changes);

                            if (!outerSrcIterator.Next(false))
                                break;
                            if (dstPropGuess != null && !dstPropGuess.Next(false))
                                dstPropGuess = null;
                        }
                    }
                    outerSrcIterator.Dispose();
                    outerDstIterator.Dispose();
                    if (dstPropGuess != null)
                        dstPropGuess.Dispose();
                }
                else
                {
                    for (int i = propertyPaths.Count - 1; i >= 0; i--)
                    {
                        //if (IsOverridden(propertyPaths[i]))
                        //    continue;

                        string path = propertyPaths[i];
                        bool isSize = path.CustomEndsWith(Helper.ARRAY_SIZE_SUFFIX);
                        if (isSize)
                            path = Helper.GetArrayPath(path);

                        InitSkipDuplicates();

                        using (var odinSrcProp = odinSrc.GetPropertyAtUnityPath(path))
                        using (var odinDstProp = odinDst.GetPropertyAtUnityPath(path))
                        {
                            if (odinSrcProp != null &&
                                odinDstProp != null && !Helper.IsUnitySerialized(odinDstProp))
                            {
                                Property srcProp = new Property()
                                {
                                    odinProperty = odinSrcProp,
                                    odinIsSize = isSize,
                                };
                                Property dstProp = new Property()
                                {
                                    odinProperty = odinDstProp,
                                    odinIsSize = isSize,
                                };
                                Property dstPropGuess = null;

                                int subChanges = 0;

                                bool enteredArray = false; // I don't know if it is or not but it doesn't matter all that much
                                RevertProp(srcProp, null, ref dstPropGuess, out _, true, enteredArray, ref subChanges, dstProp: dstProp);

                                if (dstPropGuess != null)
                                    dstPropGuess.Dispose();

                                if (subChanges > 0)
                                    if (Helper.LogUnnecessary)
                                        Debug.Log("RevertAssetFromParentAsset called for InspectorProperty: " + propertyPaths[i] + Helper.LOG_END);
                                changes += subChanges;

                                propertyPaths.RemoveAt(i);
                            }
                        }
                    }
                }

#if ODIN_INSPECTOR_3
                if (changes > S.REGULAR_RECORD_UNDO_MAX_CHANGES)
                {
                    odinDst.RecordUndoForChanges = false;
                    Undo.RegisterCompleteObjectUndo(asset, "RevertAssetFromParentAsset");
                    alreadyRecordedUndo = true;
                    odinDst.ApplyChanges();
                }
                else
                {
                    odinDst.RecordUndoForChanges = true;
                    alreadyRecordedUndo = odinDst.ApplyChanges();
                }
                odinDst.RecordUndoForChanges = prevRUFC;
#else
                alreadyRecordedUndo = odinDst.ApplyChanges();
#endif

                odinSrc.Dispose();
                odinDst.Dispose();

                RawPropertyResolver.active = false;
                RemoveAttributeProcessor.active = false;
            }
#endif
            #endregion

            #region Unity Pass
            var src = new SerializedObject(parentAsset);
            var dst = new SerializedObject(asset);

            if (renameOverrides)
            {
                InitSkipDuplicates();
                RenameOverrides(dst);
            }

            if (propertyPaths == null)
            {
                InitSkipDuplicates();

                var outerSrcIterator = src.GetUnityIterator();
                var outerDstIterator = dst.GetUnityIterator();
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                Property dstPropGuess = null;
#else
                SerializedProperty dstPropGuess = null;
#endif
                if (outerSrcIterator.NextVisible(true))
                {
                    while (true)
                    {
                        RevertProp(outerSrcIterator, outerDstIterator, ref dstPropGuess, out _, false, false, ref changes);

                        if (!outerSrcIterator.Next(false))
                            break;
                        if (dstPropGuess != null && !dstPropGuess.Next(false))
                            dstPropGuess = null;
                    }
                }
                outerSrcIterator.Dispose();
                outerDstIterator.Dispose();
                if (dstPropGuess != null)
                    dstPropGuess.Dispose();
            }
            else
            {
                for (int i = propertyPaths.Count - 1; i >= 0; i--)
                {
                    string path = propertyPaths[i];

                    InitSkipDuplicates();

                    using (var unitySrcProp = src.FindProperty(path))
                    using (var unityDstProp = dst.FindProperty(path))
                    {
                        if (unitySrcProp != null && unityDstProp != null)
                        {
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                            Property srcProp = new Property() { unityProperty = unitySrcProp };
                            Property dstProp = new Property() { unityProperty = unityDstProp };
                            Property dstPropGuess = null;
#else
                            SerializedProperty srcProp = unitySrcProp;
                            SerializedProperty dstProp = unityDstProp;
                            SerializedProperty dstPropGuess = null;
#endif

                            int subChanges = 0;

                            bool enteredArray = false; // I don't know if it is or not but it doesn't matter all that much
                            RevertProp(srcProp, null, ref dstPropGuess, out _, false, enteredArray, ref subChanges, dstProp: dstProp);

                            if (dstPropGuess != null)
                                dstPropGuess.Dispose();

                            if (subChanges > 0)
                                if (Helper.LogUnnecessary)
                                    Debug.Log("RevertAssetFromParentAsset called for SerializedProperty: " + path + Helper.LOG_END);
                            changes += subChanges;

                            propertyPaths.RemoveAt(i);
                        }
                    }
                }
            }

            if (changes > S.REGULAR_RECORD_UNDO_MAX_CHANGES || alreadyRecordedUndo)
            {
                if (!alreadyRecordedUndo)
                    Undo.RegisterCompleteObjectUndo(asset, "RevertAssetFromParentAsset");
                if (dst.ApplyModifiedPropertiesWithoutUndo() && asset != null)
                    EditorUtility.SetDirty(asset); //?
            }
            else
            {
                if (dst.ApplyModifiedProperties() && asset != null)
                    EditorUtility.SetDirty(asset);
            }

            src.Dispose();
            dst.Dispose();
            #endregion

            CustomRevertAssetFromParentAsset crafpa;
            if (customRevertAssetFromParentAsset.TryGetValue(asset.GetType(), out crafpa))
            {
                if (crafpa != null && crafpa(this, parentAsset))
                    changes++;
            }

            if (changes > 0)
                childrenNeedUpdate = true;

            return changes > 0;
        }
        public delegate bool CustomRevertAssetFromParentAsset(AV av, Object parentAsset);
        public static Dictionary<Type, CustomRevertAssetFromParentAsset> customRevertAssetFromParentAsset = new Dictionary<Type, CustomRevertAssetFromParentAsset>();

        private bool updatingFromParent = false;
        public void UpdateAssetFromParentAsset(AV selfParent = null)
        {
            if (updatingFromParent)
            {
                Debug.LogError("Possible cyclical variant dependency found at asset of path: " + asset.Path() + Helper.LOG_END);
                return;
            }

            updatingFromParent = true;
            {
                RevertAssetFromParentAsset(null, selfParent);
                if (childrenNeedUpdate)
                    PropagateChangesToChildren();
            }
            updatingFromParent = false;
        }

#if DEBUG_ASSET_VARIANTS
        private static bool timingPropagateChangesToChildren = false;
#endif
        public void PropagateChangesToChildren() //RecursivelyApplyChangesToChildren
        {
#if DEBUG_ASSET_VARIANTS
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            bool timing = !timingPropagateChangesToChildren;
            if (timing)
                sw.Start();
            timingPropagateChangesToChildren = true;
#endif

            childrenNeedUpdate = false; // Needs to be on top
            for (int i = 0; i < data.children.Count; i++)
            {
                using (AV childAV = Open(data.children[i], null))
                {
                    Debug.Log("Calling UpdateAssetFromParentAsset()" + Helper.LOG_END);
                    if (!childAV.Valid())
                        Debug.LogError("Could not call UpdateAssetFromParentAsset() for an invalid child." + Helper.LOG_END);
                    else
                        childAV.UpdateAssetFromParentAsset(this);
                }
            }

#if DEBUG_ASSET_VARIANTS
            if (timing)
            {
                timingPropagateChangesToChildren = false;
                Debug.Log("Root PropagateChangesToChildren duration: " + sw.Elapsed.TotalMilliseconds);
            }
#endif
        }
        public void FlushAnyChangesToChildren()
        {
            if (childrenNeedUpdate)
            {
                PropagateChangesToChildren();
            }
        }

        public void FlushDirty()
        {
            if (unsavedUserData)
                SaveUserDataWithUndo(true, false);
            FlushAnyChangesToChildren();
        }

        #region Skipping Duplicates
        private static void InitSkipDuplicates()
        {
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            odinVisitedObjects.Clear();
#endif
            ManagedReferencesHelper.InitSkipDuplicates();
        }

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
        private static bool SkipDuplicate(InspectorProperty prop)
        {
#if !ODIN_INSPECTOR_3
            //prop.Path=="InjectedMember"
            //TODO:::: what is that?
            if (prop.ValueEntry == null)
                return false;
#endif
            var value = prop.ValueEntry.WeakValues[0];
            if (value == null)
                return false;
            if (value.GetType().IsValueType) //?
                return false;
            return !odinVisitedObjects.Add(value);
        }

        private static readonly HashSet<object> odinVisitedObjects = new HashSet<object>(); //TODO: contain the InspectorProperty instead?
        private static bool SkipDuplicate(Property prop, bool odinPass, out int prevDepth)
        {
            if (odinPass)
            {
                prevDepth = default(int);
                return SkipDuplicate(prop.odinProperty);
            }
            else
                return ManagedReferencesHelper.SkipDuplicate(prop.ToSP(), out prevDepth);
        }
#else
        private static bool SkipDuplicate(SerializedProperty prop, bool _, out int prevDepth)
        {
            return ManagedReferencesHelper.SkipDuplicate(prop, out prevDepth);
        }
#endif
        #endregion

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
        private bool COFDDoProp(Property srcProp, Property parentDstProp, ref Property dstPropGuess, List<string> differences, out bool missing, bool odinPass, bool enteredArray)
#else
        private bool COFDDoProp(SerializedProperty srcProp, SerializedProperty parentDstProp, ref SerializedProperty dstPropGuess, List<string> differences, out bool missing, bool odinPass, bool enteredArray)
#endif
        {
            // If there is no similarity (no shared property values)
            // then an outer override can be made. Otherwise, overrides are created for each different child property.
            bool hasSimilarity;

            int prevDepth;
            if (SkipDuplicate(srcProp, odinPass, out prevDepth))
            {
                // (Not quite right, but good enough)
                hasSimilarity = false;
                missing = false;
                goto Exit;
            }

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            if (odinPass && srcProp.odinProperty.Info.PropertyType != PropertyType.Value)
            {
                Debug.LogWarning("Something went wrong");
                hasSimilarity = false;
                missing = false;
                goto Exit;
            }
#endif

            string testPath = srcProp.propertyPath;

            if (overridesCache.Contains(testPath))
            {
                hasSimilarity = false;
                missing = false;
                goto Exit;
            }
            string name = srcProp.name;
            if (srcFilter.ShouldIgnore(name, testPath, enteredArray) || dstFilter.ShouldIgnore(name, testPath, enteredArray))
            {
                hasSimilarity = false;
                missing = false;
                goto Exit;
            }

            var dstProp = parentDstProp.FindChild(srcProp, dstPropGuess);
            if (dstProp != null && dstProp.editable)
            {
                bool currentlyInvalid = false;
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                // This might never happen, where an invalid has a valid child.
                currentlyInvalid = Helper.CurrentlyInvalid(srcProp, dstProp, odinPass);
#endif

                missing = false;

                bool srcHasChildren = Helper.HasChildren(srcProp);
                bool dstHasChildren = Helper.HasChildren(dstProp);

                bool dstHasVisibleChildren = odinPass ? dstHasChildren : (dstProp.ToSP().hasVisibleChildren || dstProp.ToSP().isArray); //? //TODO2: in newer versions is the .Array.size now not a visible child? //InvalidOperationException: 'vertices' is an array so it cannot be read with boxedValue. UnityEditor.SerializedProperty.get_structValue()
                if (!currentlyInvalid && srcHasChildren != dstHasChildren)
                {
                    // (Although (aside from HashSet) it could be more accurate to consider this an override of collection size, not the whole property)
                    hasSimilarity = false;
                    differences.Add(testPath);
                }
                else if (srcHasChildren && (currentlyInvalid || dstHasVisibleChildren ||
                    !S.S.createParentOverrideIfChildrenAreInvisible))
                {
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                    bool countDiffers = false;
                    bool isHashSet = false, isDictionary = false;
                    if (odinPass && !srcProp.odinIsSize && dstProp.CompareType(srcProp))
                    {
                        var srcChildren = srcProp.odinProperty.Children;
                        var dstChildren = dstProp.odinProperty.Children;
                        countDiffers = srcChildren.Count != dstChildren.Count;
                        isHashSet = Helper.IsHashSet(srcProp.odinProperty);
                        isDictionary = Helper.IsDictionary(srcProp.odinProperty);
                    }
                    bool missingChildren = false;
#endif

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                    if (odinPass ? dstProp.odinProperty.IsCollection() : dstProp.unityProperty.isArray) enteredArray = true;
#else
                    if (dstProp.isArray) enteredArray = true;
#endif

                    List<string> subDifferences = new List<string>();
                    bool childrenHaveSimilarity = false;

                    using (var srcChildrenIterator = srcProp.Copy())
                    {
                        int depth = srcChildrenIterator.depth;

                        srcChildrenIterator.NextChildSkipArray();

                        var childDstPropGuess = dstPropGuess;
                        if (childDstPropGuess != null)
                        {
                            childDstPropGuess = childDstPropGuess.Copy();
                            if (!childDstPropGuess.NextChildSkipArray())
                                childDstPropGuess = null;
                        }

                        do
                        {
                            bool missingChild;
                            if (COFDDoProp(srcChildrenIterator, dstProp, ref childDstPropGuess, subDifferences, out missingChild, odinPass, enteredArray))
                                childrenHaveSimilarity = true;
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                            if (missingChild)
                                missingChildren = true;
#endif

                            if (!srcChildrenIterator.Next(false))
                                break;
                            if (childDstPropGuess != null && !childDstPropGuess.Next(false))
                                childDstPropGuess = null;
                        }
                        while (depth < srcChildrenIterator.depth);

                        if (childDstPropGuess != null)
                            childDstPropGuess.Dispose();
                    }

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
                    if (isHashSet)
                    {
                        if (missingChildren || countDiffers)
                        {
                            subDifferences.Clear();
                            subDifferences.Add(testPath); // So that (subDifferences.Count > 0)
                            childrenHaveSimilarity = false;
                        }
                    }
                    else if (isDictionary && childrenHaveSimilarity && (missingChildren || countDiffers))
                    {
                        differences.Add(testPath + ".DictionarySize"); // A fake property path used only by Asset Variants
                    }
#endif

                    if (!currentlyInvalid && !childrenHaveSimilarity)
                    {
                        if (subDifferences.Count > 0)
                            differences.Add(testPath);
                        hasSimilarity = false;
                    }
                    else
                    {
                        differences.AddRange(subDifferences);
                        hasSimilarity = childrenHaveSimilarity;
                    }
                }
                else if (!currentlyInvalid)
                {
                    if (Helper.AreEqual(srcProp, dstProp))
                    {
                        hasSimilarity = true;
                    }
                    else
                    {
                        differences.Add(testPath);
                        hasSimilarity = false;
                    }
                }
                else
                {
                    hasSimilarity = false;
                }
            }
            else
            {
                missing = true;
                hasSimilarity = false;
            }

            Exit:

            ManagedReferencesHelper.RestoreSkipDuplicateDepth(prevDepth);

            return hasSimilarity;
        }
        public void CreateOverridesFromDifferences()
        {
            var parentAsset = LoadParentAsset();
            if (parentAsset == null)
                Debug.LogWarning("CreateOverridesFromDifferences called for parentless asset: " + asset.Path() + Helper.LOG_END);

            bool dirty = true;

            GetOverridesCache();
            srcFilter = PropertyFilter.Get(parentAsset.GetType());
            dstFilter = PropertyFilter.Get(asset.GetType());

            List<string> allDifferences = new List<string>();

            #region Unity Pass
            var src = new SerializedObject(parentAsset);
            var dst = new SerializedObject(asset);

            InitSkipDuplicates();

            var outerSrcIterator = src.GetUnityIterator();
            var outerDstIterator = dst.GetUnityIterator();
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            Property dstPropGuess = null;
#else
            SerializedProperty dstPropGuess = null;
#endif
            bool _;
            if (outerSrcIterator.NextVisible(true))
            {
                while (true)
                {
                    COFDDoProp(outerSrcIterator, outerDstIterator, ref dstPropGuess, allDifferences, out _, false, false);

                    if (!outerSrcIterator.Next(false))
                        break;
                    if (dstPropGuess != null && !dstPropGuess.Next(false))
                        dstPropGuess = null;
                }
            }
            outerSrcIterator.Dispose();
            outerDstIterator.Dispose();
            if (dstPropGuess != null)
                dstPropGuess.Dispose();

            src.Dispose();
            dst.Dispose();
            #endregion

            #region Odin Pass
#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
            if (asset.IsOdinSerializable())
            {
                InitSkipDuplicates();

                RemoveAttributeProcessor.active = true;
                RawPropertyResolver.active = true;

                var odinSrc = PropertyTree.Create(parentAsset);
                var odinDst = PropertyTree.Create(asset);
                odinSrc.AttributeProcessorLocator = skipAttributeProcessorLocator;
                odinDst.AttributeProcessorLocator = skipAttributeProcessorLocator;

                outerSrcIterator = odinSrc.GetOdinIterator();
                outerDstIterator = odinDst.GetOdinIterator();
                dstPropGuess = null;
                if (outerSrcIterator.NextVisible(true, true))
                {
                    while (true)
                    {
                        if (!Helper.IsUnitySerialized(outerSrcIterator.odinProperty))
                            COFDDoProp(outerSrcIterator, outerDstIterator, ref dstPropGuess, allDifferences, out _, true, false);

                        if (!outerSrcIterator.Next(false))
                            break;
                        if (dstPropGuess != null && !dstPropGuess.Next(false))
                            dstPropGuess = null;
                    }
                }
                outerSrcIterator.Dispose();
                outerDstIterator.Dispose();
                if (dstPropGuess != null)
                    dstPropGuess.Dispose();

                odinSrc.Dispose();
                odinDst.Dispose();

                RawPropertyResolver.active = false;
                RemoveAttributeProcessor.active = false;
            }
#endif
            #endregion

            CustomCreateOverridesFromDifferences ccofd;
            if (customCreateOverridesFromDifferences.TryGetValue(asset.GetType(), out ccofd))
            {
                if (ccofd != null)
                    ccofd(this, parentAsset, allDifferences);
            }

            for (int i = 0; i < allDifferences.Count; i++)
            {
                if (!overridesCache.Contains(allDifferences[i]))
                {
                    if (Helper.LogUnnecessary)
                        Debug.Log("Created override from differences: " + allDifferences[i] + Helper.LOG_END);

                    overridesCache.Add(allDifferences[i]);
                    dirty = true;
                }
            }

            if (dirty)
                SaveUserDataWithUndo();
        }
        public delegate void CustomCreateOverridesFromDifferences(AV av, Object parentAsset, List<string> allDifferences);
        public static Dictionary<Type, CustomCreateOverridesFromDifferences> customCreateOverridesFromDifferences = new Dictionary<Type, CustomCreateOverridesFromDifferences>();

        [Serializable]
        public class Data
        {
            public AssetID parent = AssetID.Null;
            public List<AssetID> children = new List<AssetID>();
            public List<string> overrides = new List<string>();

            public bool HasParent => !string.IsNullOrEmpty(parent.guid);

            public bool HasParentlessOverrides => !HasParent && overrides.Count > 0;

            public bool Exists => HasParent || children.Count > 0 || overrides.Count > 0;

            public string ToJSON()
            {
                if (Exists)
                    return JsonUtility.ToJson(this);
                else
                    return "";
            }
        }
        public Data data;

        internal OverridesCache overridesCache = null;
        public OverridesCache GetOverridesCache()
        {
            if (overridesCache == null)
            {
                overridesCache = new OverridesCache();
                overridesCache.Init(data.overrides);
            }
            return overridesCache;
        }
        public void UpdateOverridesCache()
        {
            if (overridesCache != null)
                overridesCache.Init(data.overrides);
        }
        public bool IsOverridden(string path)
        {
            if (overridesCache != null)
                return overridesCache.Contains(path);
            else
                return data.overrides.Contains(path);
        }
        public bool TryCreateOverride(string path)
        {
            if (overridesCache != null)
            {
                return overridesCache.Add(path);
            }
            else
            {
                if (data.overrides.Contains(path))
                    return false;
                data.overrides.Add(path);
                return true;
            }
        }
        public bool RemoveOverride(string path)
        {
            if (overridesCache != null)
                return overridesCache.Remove(path);
            else
                return data.overrides.Remove(path);
        }

        private HashSet<Object> descendants = null;
        public HashSet<Object> GetDescendants()
        {
            if (descendants == null)
            {
                descendants = new HashSet<Object>();
                FillDescendants(descendants);
            }

            return descendants;
        }
        private void FillDescendants(HashSet<Object> descendants)
        {
            for (int i = 0; i < data.children.Count; i++)
            {
                var child = data.children[i].Load();
                if (child == null)
                {
                    Debug.LogWarning("?");
                    continue;
                }

                descendants.Add(child);

                using (var childAV = Open(child))
                {
                    if (childAV.Valid())
                        childAV.FillDescendants(descendants);
                }
            }
        }
        public void ClearDescendants()
        {
            //TODO: AVEditor should call this?

            descendants = null;
        }
        public static void ClearAllDescendants()
        {
            foreach (var kv in openAVs)
                kv.Value.ClearDescendants();
        }

        public bool childrenNeedUpdate = false; // If the properties of the asset have changed and need to be applied to the children

        public void TryAddChild(AssetID newChild)
        {
            using (AV newChildAV = Open(newChild, null))
            {
                if (newChildAV == null)
                    return;

                if (newChildAV.data.parent != assetID)
                {
                    newChildAV.data.parent = assetID;
                    newChildAV.SaveUserDataWithUndo();

                    if (Helper.LogUnnecessary)
                        Debug.Log("Added child: " + newChildAV.asset + Helper.LOG_END);
                }
            }

            if (!data.children.Contains(newChild))
            {
                data.children.Add(newChild);
                SaveUserDataWithUndo();

                HierarchyChanged();
            }
        }
        public void TryRemoveChild(AssetID oldChild)
        {
            using (AV oldChildAV = Open(oldChild, null))
            {
                if (oldChildAV == null)
                {
                    if (data.children.Remove(oldChild))
                        SaveUserDataWithUndo();
                }
                else
                {
                    if (oldChildAV.data.parent == assetID)
                    {
                        oldChildAV.data.parent = AssetID.Null;
                        oldChildAV.SaveUserDataWithUndo();

                        if (Helper.LogUnnecessary)
                            Debug.Log("Removed child: " + oldChildAV.asset + Helper.LOG_END);
                    }

                    if (data.children.Remove(oldChild))
                        SaveUserDataWithUndo();

                    HierarchyChanged();
                }
            }
        }

        public void HierarchyChanged()
        {
            descendants = null;
            OverrideIndicator.ClearCaches();
            AVHeader.InitAllTargets();
        }

        private static readonly List<AV> cyclicalSearch = new List<AV>();
        private void SearchCyclical()
        {
            for (int i = 0; i < data.children.Count; i++)
            {
                var childAV = Open(data.children[i], null);
                if (!cyclicalSearch.Contains(childAV))
                {
                    cyclicalSearch.Add(childAV);
                    childAV.SearchCyclical();
                }
                else
                    childAV.Dispose();
            }
        }
        private static void ClearCyclical()
        {
            for (int i = 0; i < cyclicalSearch.Count; i++)
                cyclicalSearch[i].Dispose();
            cyclicalSearch.Clear();
        }

        public void ChangeParent(AssetID newParent)
        {
            if (newParent == data.parent)
                return;

            if (newParent == assetID)
            {
                Debug.LogError("Can't set Parent to itself: " + assetID + Helper.LOG_END);
                return;
            }

            if (data.HasParent)
            {
                using (AV oldParentAV = Open(data.parent, null))
                {
                    if (oldParentAV != null)
                        oldParentAV.TryRemoveChild(assetID);
                }
            }

            using (AV newParentAV = Open(newParent, null))
            {
                if (newParentAV != null)
                {
                    ClearCyclical();
                    SearchCyclical();
                    if (cyclicalSearch.Contains(newParentAV))
                    {
                        data.parent = AssetID.Null;
                        SaveUserDataWithUndo();
                        Debug.LogError("Cyclical dependencies found. Did not set parent of: " + assetID.Load().Path() + "\nto: " + newParent.Load().Path() + Helper.LOG_END);
                    }
                    else
                    {
                        if (Helper.LogUnnecessary)
                            Debug.Log("Changed parent from: " + data.parent.Load() + " to: " + newParentAV.asset + Helper.LOG_END);

                        data.parent = newParent;
                        SaveUserDataWithUndo();
                        newParentAV.TryAddChild(assetID);
                        CreateOverridesFromDifferences();
                    }
                    ClearCyclical();
                }
                else
                {
                    data.parent = AssetID.Null;
                    SaveUserDataWithUndo();
                }
            }

            cachedParent = null;
            ClearAllDescendants(); // Easiest and safest to just clear all
        }
        internal static bool skipValidateRelations;
        public void ValidateRelations()
        {
            if (skipValidateRelations)
                return;

            if (EditorApplication.isUpdating)
            {
                Debug.LogWarning("EditorApplication.isUpdating==true. Shouldn't call ValidateRelations() now.");
                return;
            }

            // This makes sure all relations make sense

            bool dirty = false;

            if (data.parent == assetID)
            {
                Debug.LogWarning("Can't set Parent to itself" + Helper.LOG_END);
                data.parent = AssetID.Null;
                data.children.Remove(assetID);
                dirty = true;
            }
            else if (data.HasParent)
            {
                Type selfType = asset.GetType();
                Object parentAsset = LoadParentAsset();
                if (parentAsset == null)
                {
                    Debug.LogWarning(asset.Path() + "'s parent: " + data.parent + " does not exist, it could not be loaded. Parent is now set to null." + Helper.LOG_END);
                    data.parent = AssetID.Null;
                    dirty = true;
                }
                else
                {
                    Type parentType = parentAsset.GetType();
                    if (S.S.directInheritanceRequired && !parentType.IsAssignableFrom(selfType) && !selfType.IsAssignableFrom(parentType))
                    {
                        Debug.LogWarning("Parent type: " + parentType + " is not convertible to or from variant type: " + selfType + "\nIf you want to allow this, set AssetVariants.Settings.DIRECT_INHERITANCE_REQUIRED to false. Parent is now set to null." + Helper.LOG_END);
                        data.parent = AssetID.Null;
                        dirty = true;
                    }
                    else
                    {
                        using (AV parentAV = Open(data.parent, null))
                        {
                            parentAV.TryAddChild(assetID);
                        }
                    }
                }
            }

            for (int i = 0; i < data.children.Count; i++)
            {
                if (data.children[i].Load() == null) // TODO2: optimize?
                {
                    Debug.LogWarning(asset.Path() + " has a nonexistent child of AssetID: " + data.children[i] + "\nRemoved reference to it." + Helper.LOG_END);
                    data.children.RemoveAt(i);
                    i--;
                    dirty = true;
                }
                else
                {
                    using (AV childAV = Open(data.children[i], null))
                    {
                        if (childAV.data.parent != assetID)
                        {
                            Debug.LogWarning("Child " + data.children[i] + " does not have " + assetID + " as Parent" + Helper.LOG_END);
                            data.children.RemoveAt(i);
                            i--;
                            dirty = true;
                        }
                    }
                }
            }

            if (dirty)
                SaveUserDataWithUndo(); // TODO2: Should this skip recording undo?
        }

        public void LoadUserData()
        {
            if (Importer == null)
            {
                Importer = AssetImporter.GetAtPath(assetID.guid.ToPath());
                if (Importer == null)
                {
                    Debug.LogError("Importer == null");
                    return;
                }
            }

            SharedUserDataUtility.ValidateOwnership validateOwnership;
            string userData = Importer.GetUserData(UserDataKey, out validateOwnership);

            if (validateOwnership == null) // Asset Variants has never been used without SharedUserDataUtility, no need to use validateOwnership
            {
                data = JsonUtility.FromJson<Data>(userData);
                if (data == null)
                    data = new Data();
            }
            else
            {
                // If validateOwnership exists then the userData cannot be for Asset Variants.
                data = new Data();
            }

            UpdateOverridesCache();
        }
        public void SaveUserDataWithUndo(bool force = false, bool undo = true)
        {
            if (!force && S.S.onlySaveUserDataOnLeave)
            {
                unsavedUserData = true;
                return;
            }
            unsavedUserData = false;

            if (force)
                if (Helper.LogUnnecessary)
                    Debug.Log("Forced save of Asset Variants userData" + Helper.LOG_END);

            string newUserData = data.ToJSON();
            if (Importer.SetUserData(UserDataKey, newUserData, undo ? "Modify Asset Variants Data" : null))
                EditorUtility.SetDirty(Importer); // I don't know if this is necessary
        }
        private bool unsavedUserData = false;

        public static void LoadAndValidateAll(string guid)
        {
            foreach (var kv in openAVs)
            {
                if (kv.Key.guid == guid)
                {
                    kv.Value.LoadUserData();
                    kv.Value.ValidateRelations();
                }
            }
        }

        public static AV Open(Object asset)
        {
            return Open(asset.ToAssetID(), asset);
        }
        public static AV Open(AssetID assetID, Object asset)
        {
            AV openAV;
            if (openAVs.TryGetValue(assetID, out openAV))
            {
                openAV.UserCount++;
                return openAV;
            }
            var importer = AssetImporter.GetAtPath(assetID.guid.ToPath());
            if (importer == null)
                return null;
            if (asset == null)
            {
                asset = assetID.Load();
                if (asset == null)
                    return null;
            }
            AV av = new AV(assetID, asset, importer);
            openAVs.Add(assetID, av);
            av.UserCount = 1;
            av.LoadUserData();
            return av;
        }
        private static readonly Dictionary<AssetID, AV> openAVs = new Dictionary<AssetID, AV>();

        internal void IncrementUserCount()
        {
            UserCount++;
        }

        public int UserCount { get; private set; } = 0;
        private AV(AssetID assetID, Object asset, AssetImporter importer)
        {
            this.assetID = assetID;
            this.asset = asset;
            this.Importer = importer;
        }
        public void Dispose()
        {
            UserCount--;
            if (UserCount == 0)
            {
                FlushImplicit(true);

                if (unsavedUserData)
                    SaveUserDataWithUndo(true, false);

                FlushAnyChangesToChildren();

                //if (!EditorApplication.isUpdating)
                //{
                //    if (Helper.IsDirty(importer) && S.S.saveAndReimportOnLeave)
                //    {
                //        importer.SaveAndReimport(); // (Note that this calls the Editor's OnDisable())
                //        if (Helper.LogUnnecessary)
                //            Debug.Log("AssetVariant SaveAndReimport() called automatically");
                //    }
                //}

                if (UserCount == 0)
                    openAVs.Remove(assetID);

                if (implicitSrc != null)
                {
                    implicitSrc.Dispose();
                    implicitSrc = null;
                }
                if (implicitDst != null)
                {
                    implicitDst.Dispose();
                    implicitDst = null;
                }
            }
        }

        private static void UndoRedoPerformed()
        {
            alreadyRenamedOverrides.Clear();

            ClearAllDescendants();

            foreach (var kv in openAVs)
            {
                if (!kv.Value.unsavedUserData)
                {
                    try
                    {
                        kv.Value.LoadUserData();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;

            EditorApplication.playModeStateChanged += (c) =>
            {
                //Debug.Log(c);
                if (c == PlayModeStateChange.ExitingEditMode)
                {
                    List<AV> all = new List<AV>(); // To prevent InvalidOperationException: Collection was modified; enumeration operation may not execute.
                    foreach (var kv in openAVs)
                        all.Add(kv.Value);
                    for (int i = 0; i < all.Count; i++)
                        all[i].FlushDirty();
                    all.Clear();
                }
            };
        }

        ~AV()
        {
            if (UserCount != 0)
                Debug.LogError("(UserCount != 0) - Implementation bug, fix it Steffen - UserCount: " + UserCount + "\n" + asset.name + Helper.LOG_END);
        }
    }
}

#endif
