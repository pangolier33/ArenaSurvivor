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
using Object = UnityEngine.Object;
using S = AssetVariants.AVSettings;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
#endif
using SPT = UnityEditor.SerializedPropertyType;

namespace AssetVariants
{
    internal static class R
    {
        public static readonly object[] arg = new object[1];
        public static readonly object[] args = new object[2];

        internal const BindingFlags nps = BindingFlags.NonPublic | BindingFlags.Static;
        internal const BindingFlags npi = BindingFlags.NonPublic | BindingFlags.Instance;

        public static Assembly assembly = typeof(Editor).Assembly;
        public static Type sauType = assembly.GetType("UnityEditor.ScriptAttributeUtility");
        public static MethodInfo getFieldInfoFromPropertyMI = sauType.GetMethod("GetFieldInfoFromProperty", nps);
        public static MethodInfo GetFieldInfoFromPropertyPath = sauType.GetMethod("GetFieldInfoFromPropertyPath", nps);
    }

    internal static class Helper
    {
        public const string ARRAY_SIZE_SUFFIX = ".Array.size";

        public static string GetArrayPath(string path)
        {
            return path.Substring(0, path.Length - ARRAY_SIZE_SUFFIX.Length);
        }


        public static bool Valid(this AV av)
        {
            if (av == null || av.asset is DefaultAsset)
            {
                Debug.LogWarning("Invalid AV!" + LOG_END);
                return false;
            }
            return true;
        }


        public static bool CustomEndsWith(this string s, string end)
        {
            // It baffles me, but this seems to cut down the time a lot
            // compared to string.EndsWith, which was sometimes a bottleneck.

            //TODO: use str.EndsWith("s", StringComparison.Ordinal)

            int endL = end.Length;
            int sL = s.Length;
            if (sL < endL)
                return false;
            int off = sL - endL;
            for (int i = off; i < sL; i++)
            {
                if (s[i] != end[i - off])
                    return false;
            }
            return true;
        }


        public static bool PropertyTypesMatch(SPT a, SPT b)
        {
            // SPT.ArraySize as well?
            if ((a == SPT.Integer || a == SPT.Enum) && (b == SPT.Integer || b == SPT.Enum))
                return true;
            return a == b;
        }


        public static bool CopyValueRecursively(SerializedProperty outerDstProp, SerializedProperty srcProp)
        {
            bool dirty = false;

            bool copyChildren;
            if (!CopyValueIfMatchType(outerDstProp, srcProp, out copyChildren, ref dirty) && copyChildren)
            {
                using (var srcIterator = srcProp.Copy())
                {
                    int depth = srcIterator.depth;
                    if (srcIterator.NextChildSkipArray())
                    {
                        do
                        {
                            var dst = outerDstProp.serializedObject;
                            var dstProp = dst.FindProperty(srcIterator.propertyPath); //?
                            if (dstProp == null || CopyValueIfMatchType(dstProp, srcIterator, out copyChildren, ref dirty))
                            {
                                if (!srcIterator.Next(false))
                                    break;
                            }
                            else
                            {
                                if (!srcIterator.Next(copyChildren))
                                    break;
                            }
                        }
                        while (depth < srcIterator.depth);
                    }
                }
            }

            return dirty;
        }
        private static bool NeedsCopyChildren(SerializedProperty prop)
        {
            if (!prop.hasChildren)
                return false;
            var pt = prop.propertyType;
#if UNITY_2019_3_OR_NEWER
            if (pt == SPT.Generic || pt == SPT.Gradient || pt == SPT.ManagedReference)
#else
            if (pt == SPT.Generic || pt == SPT.Gradient)
#endif
                return true;
            return HasChildren(prop);
        }
        private static bool CopyValueIfMatchType(SerializedProperty dstProp, SerializedProperty srcProp, out bool copyChildren, ref bool dirty)
        {
            bool dstNCC = NeedsCopyChildren(dstProp);
            bool srcNCC = NeedsCopyChildren(srcProp);
#if DEBUG_ASSET_VARIANTS
            if (dstNCC != srcNCC)
                Debug.LogWarning("dstNCC != srcNCC");
#endif
            copyChildren = dstNCC && srcNCC;
            if (!dstNCC && !srcNCC && PropertyTypesMatch(dstProp.propertyType, srcProp.propertyType))
            {
                if (CopyValue(dstProp, srcProp))
                {
                    if (S.S.logPropertyCopies || forceLog)
                    {
                        string src = srcProp.serializedObject.targetObject.name;
                        string dst = dstProp.serializedObject.targetObject.name;
                        Debug.Log("Copied value of property: " + dstProp.propertyPath + "\nFrom: " + src + "\nTo: " + dst + LOG_END);
                    }
                    dirty = true;
                }
                return true;
            }
            return false;
        }
        private static bool CopyValue(SerializedProperty dstProp, SerializedProperty srcProp)
        {
            // Apparently boxedValue is absurdly limited:
            /*
            "boxedValue has some limitations for properties of type SerializedPropertyType.Generic:
            Structs and objects serialized by-value can be accessed, unless they contain fields with the SerializeReference attribute or fixed buffers.
            The property cannot be an array or list. But accessing a property that is an element of an array or list is supported.
            Unity built-in Struct types that are categorized in the Generic type cannot be accessed. But built-in types like Vector3 that have a their own entry in the SerializedPropertyType enum do work."
            */
            //#if false //UNITY_2022_1_OR_NEWER
            ///*
            //NotSupportedException: The value of 'unityEvent' cannot be retrieved with boxedValue because it is a built-in type 'UnityEvent'.
            //UnityEditor.SerializedProperty.get_structValue () (at <5700f1e4d61f4c96b68ca2ee3cdd8eae>:0)
            //UnityEditor.SerializedProperty.get_boxedValue () (at <5700f1e4d61f4c96b68ca2ee3cdd8eae>:0)
            //*/
            //var srcValue = srcProp.boxedValue;
            //bool equals;
            //if (srcValue == null)
            //    equals = dstProp.boxedValue == null;
            //else
            //    equals = srcValue.Equals(dstProp.boxedValue);
            //if (!equals)
            //{
            //    dstProp.boxedValue = srcValue;
            //    return true;
            //}
            //return false;
            //#else

            // (Can't use CopyFromSerializedProperty/CopyFromSerializedPropertyIfDifferent because those search for the matching property path, which is O(n^2))

            try
            {
                switch (dstProp.propertyType)
                {
                    //case SPT.Generic: dstProp.structValue = value; break;
                    case SPT.Enum: goto case SPT.Integer;
                    case SPT.ArraySize: goto case SPT.Integer;
                    case SPT.Integer:
                        {
#if UNITY_2022_1_OR_NEWER
                            if (srcProp.numericType == SerializedPropertyNumericType.UInt64)
                            { if (dstProp.ulongValue != srcProp.ulongValue) { dstProp.ulongValue = srcProp.ulongValue; return true; } return false; }
                            else
                            { if (dstProp.longValue != srcProp.longValue) { dstProp.longValue = srcProp.longValue; return true; } return false; }
#else
                            if (dstProp.intValue != srcProp.intValue) { dstProp.intValue = srcProp.intValue; return true; }
                            return false;
#endif
                        }
                    case SPT.Boolean: if (dstProp.boolValue != srcProp.boolValue) { dstProp.boolValue = srcProp.boolValue; return true; } return false;
                    case SPT.Float:
                        {
#if UNITY_2022_1_OR_NEWER
                            if (dstProp.numericType == SerializedPropertyNumericType.Double)
                            { if (dstProp.doubleValue != srcProp.doubleValue) { dstProp.doubleValue = srcProp.doubleValue; return true; } return false; }
                            else
                            { if (dstProp.floatValue != srcProp.floatValue) { dstProp.floatValue = srcProp.floatValue; return true; } return false; }
#else
                            if (dstProp.floatValue != srcProp.floatValue) { dstProp.floatValue = srcProp.floatValue; return true; }
                            return false;
#endif
                        }
                    case SPT.String: if (dstProp.stringValue != srcProp.stringValue) { dstProp.stringValue = srcProp.stringValue; return true; } return false;
                    case SPT.Color: if (dstProp.colorValue != srcProp.colorValue) { dstProp.colorValue = srcProp.colorValue; return true; } return false;
                    case SPT.ObjectReference: if (dstProp.objectReferenceValue != srcProp.objectReferenceValue) { dstProp.objectReferenceValue = srcProp.objectReferenceValue; return true; } return false;
                    case SPT.LayerMask: if (dstProp.intValue != srcProp.intValue) { dstProp.intValue = srcProp.intValue; return true; } return false;
                    case SPT.Vector2: if (dstProp.vector2Value != srcProp.vector2Value) { dstProp.vector2Value = srcProp.vector2Value; return true; } return false;
                    case SPT.Vector3: if (dstProp.vector3Value != srcProp.vector3Value) { dstProp.vector3Value = srcProp.vector3Value; return true; } return false;
                    case SPT.Vector4: if (dstProp.vector4Value != srcProp.vector4Value) { dstProp.vector4Value = srcProp.vector4Value; return true; } return false;
                    case SPT.Rect: if (dstProp.rectValue != srcProp.rectValue) { dstProp.rectValue = srcProp.rectValue; return true; } return false;
#if UNITY_2022_1_OR_NEWER
                    case SPT.Character: if (dstProp.uintValue != srcProp.uintValue) { dstProp.uintValue = srcProp.uintValue; return true; } return false;
#else
                    case SPT.Character: if (dstProp.intValue != srcProp.intValue) { dstProp.intValue = srcProp.intValue; return true; } return false;
#endif
                    case SPT.AnimationCurve: if (dstProp.animationCurveValue != srcProp.animationCurveValue) { dstProp.animationCurveValue = srcProp.animationCurveValue; return true; } return false;
                    case SPT.Bounds: if (dstProp.boundsValue != srcProp.boundsValue) { dstProp.boundsValue = srcProp.boundsValue; return true; } return false;
#if UNITY_2022_1_OR_NEWER
                    case SPT.Gradient: if (dstProp.gradientValue != srcProp.gradientValue) { dstProp.gradientValue = srcProp.gradientValue; return true; } return false;
#else
                    case SPT.Gradient: Debug.LogError("Before 2022.1 Unity lacked SerializedProperty.gradientValue."); return false; //\nYou should remove SerializedPropertyType.Gradient from Settings.considerWhole." + LOG_END); return false;
#endif
                    case SPT.Quaternion: if (!dstProp.quaternionValue.Equals(srcProp.quaternionValue)) { dstProp.quaternionValue = srcProp.quaternionValue; return true; } return false; // Needs to be .Equals because != "tests whether dot product of two quaternions is [not] close to 1.0", and (0,0,0,0) can exist (even though it's invalid).
                    case SPT.ExposedReference: if (dstProp.exposedReferenceValue != srcProp.exposedReferenceValue) { dstProp.exposedReferenceValue = srcProp.exposedReferenceValue; return true; } return false;
#if UNITY_2017_2_OR_NEWER
                    case SPT.Vector2Int: if (dstProp.vector2IntValue != srcProp.vector2IntValue) { dstProp.vector2IntValue = srcProp.vector2IntValue; return true; } return false;
                    case SPT.Vector3Int: if (dstProp.vector3IntValue != srcProp.vector3IntValue) { dstProp.vector3IntValue = srcProp.vector3IntValue; return true; } return false;
                    //case SPT.RectInt: if (dstProp.rectIntValue != srcProp.rectIntValue) { dstProp.rectIntValue = srcProp.rectIntValue; return true; } return false;
                    case SPT.RectInt: if (!dstProp.rectIntValue.Equals(srcProp.rectIntValue)) { dstProp.rectIntValue = srcProp.rectIntValue; return true; } return false;
                    case SPT.BoundsInt: if (dstProp.boundsIntValue != srcProp.boundsIntValue) { dstProp.boundsIntValue = srcProp.boundsIntValue; return true; } return false;
#endif
#if UNITY_2021_2_OR_NEWER
                    case SPT.ManagedReference: if (dstProp.managedReferenceValue != srcProp.managedReferenceValue) { dstProp.managedReferenceValue = srcProp.managedReferenceValue; return true; } return false;
#elif UNITY_2019_3_OR_NEWER
                    case SPT.ManagedReference: Debug.LogError("Before 2021.2 Unity lacked SerializedProperty.managedReferenceValue.get" + LOG_END); return false;
#endif
#if UNITY_2021_1_OR_NEWER
                    case SPT.Hash128: if (dstProp.hash128Value != srcProp.hash128Value) { dstProp.hash128Value = srcProp.hash128Value; return true; } return false;
#endif

                    default: // FixedBufferSize is read-only
                        return false; //throw new NotSupportedException(string.Format("Set on boxedValue property is not supported on \"{0}\" because it has an unsupported propertyType {1}.", propertyPath, propertyType));
                }
            }
            catch (InvalidCastException e)
            {
                Debug.LogError(e + LOG_END);
                return false;
            }
        }

        public static bool forceLog = false;
        public const string LOG_END = "\n"; // To give some spacing between it and the stack trace
        public static bool LogUnnecessary => forceLog || S.S.logUnnecessary;

        public static bool IsDirty(Object o)
        {
#if UNITY_201_1_OR_NEWER
            return EditorUtility.IsDirty(o);
#else
            return false;
#endif
        }

#if ODIN_INSPECTOR
        public static bool IsUnitySerialized(this InspectorProperty prop)
        {
            var sb = prop.Info.SerializationBackend;
#if ODIN_INSPECTOR_3
            return sb == SerializationBackend.Unity || sb == SerializationBackend.UnityPolymorphic; //?
#else
            return sb == SerializationBackend.Unity;
#endif
        }

        public static object DeepCopy(object o)
        {
            if (o == null)
                return null;
            if (o.GetType().IsValueType)
                return o;
            else
                return Sirenix.Serialization.SerializationUtility.CreateCopy(o);
        }

        public static bool IsHashSet(this InspectorProperty property)
        {
            return property.Info.TypeOfValue.InheritsFrom(typeof(HashSet<>));
        }
        public static bool IsDictionary(this InspectorProperty property)
        {
            return property.Info.TypeOfValue.InheritsFrom(typeof(Dictionary<,>));
        }

        public static bool IsCollectionWithSize(this InspectorProperty property)
        {
#if ODIN_INSPECTOR_3
            if (!property.ChildResolver.IsCollection)
                return false;
#endif
            var type = property.Info.TypeOfValue;
#if !ODIN_INSPECTOR_3 //???????
            //TODO:
            if (type == null)
                return false;
#endif
            //if (type.IsArray || type.InheritsFrom(typeof(List<>)))
            if (type.IsArray || typeof(IList).IsAssignableFrom(type))
                return true;
            if (type.InheritsFrom(typeof(Queue<>)) || type.InheritsFrom(typeof(Stack<>)))
                return true;
            return false;
        }
#endif

        public static SerializedProperty FindChild(this SerializedProperty parentDst, SerializedProperty srcProp, SerializedProperty dstPropGuess)
        {
            string srcPath = srcProp.propertyPath;

            // These first are to optimize against O(n^2) slowdown with huge arrays

            if (dstPropGuess != null)
            {
                if (dstPropGuess.propertyPath == srcPath)
                {
                    //Debug.Log("Fast " + srcPath);
                    return dstPropGuess;
                }

                dstPropGuess.Dispose();
            }
            //else
            //Debug.Log("Null guess");

            string parentDstPath = parentDst.propertyPath;
            if (parentDstPath == "")
            {
                var so = parentDst.serializedObject;
                //Debug.Log("Slow global: " + srcPath);
                return so.FindProperty(srcPath);
            }
            else
            {
                int srcLength = srcPath.Length;
                int parentDstLength = parentDstPath.Length;
                if (srcLength == parentDstLength)
                {
                    Debug.LogError("\n");
                    return null;
                }
                parentDstLength++; // Remove the "."

                //Debug.Log(srcPath + " " + parentDstPath); 

                //Debug.Log("Slow local " + srcPath);

                string subPath = srcPath.Substring(parentDstLength, srcLength - parentDstLength);
                return parentDst.FindPropertyRelative(subPath);
            }
        }

        public static bool AreEqual(SerializedProperty a, SerializedProperty b)
        {
            // The problem may be related to: https://issuetracker.unity3d.com/issues/serializedproperty-dot-dataequals-returns-true-for-two-strings-that-are-different
            //if (SerializedProperty.DataEquals(a, b))
            //{
            //    return true;
            //}
            //Debug.Log("Difference at: " + a.propertyPath + " and: " + b.propertyPath + " - " + a.propertyType);
            //if (a.propertyType == SPT.Integer)
            //    Debug.Log(a.intValue + " =?= " + b.intValue);
            //return false;
            // Basically it says there is a difference for: Difference at: gradient.m_NumAlphaKeys and: gradient.m_NumAlphaKeys - Integer
            // when 2 =?= 2, when the text field immediately after it is different.

            //if (!PropertyTypesMatch(a.propertyType, b.propertyType))
            //    return false;
            return SerializedProperty.DataEquals(a, b);
        }

        public static SerializedProperty ToSP(this SerializedProperty property)
        {
            return property;
        }

#if ASSET_VARIANTS_DO_ODIN_PROPERTIES
        public static object ResizeArray(object array, int n)
        {
            var type = array.GetType();
            var elemType = type.GetElementType();
            var resizeMethod = typeof(Array).GetMethod("Resize", BindingFlags.Static | BindingFlags.Public);
            var properResizeMethod = resizeMethod.MakeGenericMethod(elemType);
            var parameters = new object[] { array, n };
            properResizeMethod.Invoke(null, parameters);
            return parameters[0];
        }

        public static bool IsOdinSerializable(this Object asset)
        {
            return typeof(Sirenix.OdinInspector.SerializedScriptableObject).IsAssignableFrom(asset.GetType());
        }

        public static SerializedProperty ToSP(this Property property)
        {
            return property.unityProperty;
        }

        public static bool CurrentlyInvalid(Property src, Property dst, bool odinPass)
        {
            if (odinPass)
            {
                //TODO: is this proper?
                //It was based on an incorrect assumption.

                // When iterating over the Odin PropertyTree in the odin pass, need to ignore properties that are not serialized with Odin,
                // There might be children properties that actually are serialized with Odin.
                var srcSB = src.odinProperty.Info.SerializationBackend;
                var dstSB = dst.odinProperty.Info.SerializationBackend;
                return srcSB != SerializationBackend.Odin || dstSB != SerializationBackend.Odin;
            }
            else
                return false;
        }

        public static bool AreEqual(InspectorProperty x, InspectorProperty y)
        {
            // Deep comparison, for multidimensional arrays mostly, which are reference type but have 0 InspectorProperty Children (I don't know why)
            return ObjectComparer.AreEqual(x.ValueEntry.WeakValues[0], y.ValueEntry.WeakValues[0]); //return x.property.ValueEntry.ValueTypeValuesAreEqual(y.property.ValueEntry);
        }
        public static bool AreEqual(Property x, Property y)
        {
            if (x.IsUnity)
            {
                return AreEqual(x.unityProperty, y.unityProperty);
            }

            return AreEqual(x.odinProperty, y.odinProperty);
        }

        public static bool HasChildren(Property prop)
        {
            if (prop.IsUnity)
                return Helper.HasChildren(prop.unityProperty);   

            if (prop.odinIsSize)
                return false;
            if (prop.odinProperty.IsCollectionWithSize()) // Accounts for the isSize "fake property"
                return true;
            int childCount = prop.odinProperty.Children.Count;
            if (childCount == 0) // (Children that are invisible don't seem to exist in Odin)
                return false;
            return true;
        }

        public static Property FindChild(this Property parentDst, Property srcProp, Property dstPropGuess)
        {
            string srcPath = srcProp.propertyPath;

            if (dstPropGuess != null)
            {
                if (dstPropGuess.propertyPath == srcPath)
                    return dstPropGuess;
                dstPropGuess.Dispose();
            }

            if (parentDst.IsUnity)
            {
                SerializedProperty prop = parentDst.unityProperty.FindChild(srcProp.unityProperty, null);

                if (prop == null)
                    return null;
                return new Property()
                {
                    unityProperty = prop,
                    odinProperty = null,
                };
            }
            else //if (parentDst.IsOdin)
            {
                if (srcProp.odinIsSize)
                {
                    if (dstPropGuess != null)
                        dstPropGuess.Dispose();

                    return new Property()
                    {
                        odinProperty = parentDst.odinProperty,
                        odinIsSize = true,
                        odinDepth = parentDst.depth + 1,
                    };
                }

                InspectorProperty prop;

                if (dstPropGuess != null)
                {
                    if (dstPropGuess.Next(false))
                    {
                        if (dstPropGuess.propertyPath == srcPath)
                            return dstPropGuess;
                    }
                    dstPropGuess.Dispose();

                    //int prevIndex = prevDstProp.odinProperty.Index;
                    //if (prevIndex + 1 < children.Count)
                    //{
                    //    using var nextChild = children[prevIndex + 1];
                    //    if (children[prevIndex + 1].UnityPropertyPath == odinPath)
                    //        prop = nextChild;
                    //}
                }

                if (parentDst.odinProperty.UnityPropertyPath == "") // TODO2: .Path instead ?
                {
                    var tree = parentDst.odinProperty.Tree;
                    prop = tree.GetPropertyAtUnityPath(srcPath);
                }
                else
                {
                    prop = null;

                    var children = parentDst.odinProperty.Children;

                    if (prop == null)
                    {
                        for (int i = 0; i < children.Count; i++)
                        {
                            using (var child = children[i]) // TODO2: Should this even be "using"?
                            {
                                if (child.UnityPropertyPath == srcPath)
                                {
                                    prop = child;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (prop == null)
                    return null;
                return new Property()
                {
                    unityProperty = null,
                    odinProperty = prop,
                    odinDepth = parentDst.depth + 1, //?
                };
            }
        }

        public static Property GetUnityIterator(this SerializedObject so)
        {
            return new Property()
            {
                unityProperty = so.GetIterator(),
                odinProperty = null,
            };
        }
        public static Property GetOdinIterator(this PropertyTree tree)
        {
            return new Property()
            {
                unityProperty = null,
                odinProperty = tree.RootProperty(),
            };
        }


#if !ODIN_INSPECTOR_3
        public static void RecordForUndo(this InspectorProperty property, string message)
        {
            //TODO:: could this have terrible performance impact?
            property.SerializationRoot.RecordForUndo(message);
        }
        public static InspectorProperty NextProperty(this InspectorProperty property, bool includeChildren, bool visibleOnly)
        {
            //TODO::: how big of a problem is this?
            return property.NextProperty(includeChildren);
        }
#endif
        public static InspectorProperty RootProperty(this PropertyTree propertyTree)
        {
            //Debug.Log(propertyTree.GetRootProperty(0).Parent == propertyTree.RootProperty);
#if ODIN_INSPECTOR_3
            return propertyTree.RootProperty;
#else
            return propertyTree.SecretRootProperty;
#endif
        }
        public static bool IsCollection(this InspectorProperty prop)
        {
#if ODIN_INSPECTOR_3
            return prop.ChildResolver.IsCollection;
#else
            return IsCollectionWithSize(prop) || prop.Info.TypeOfValue.InheritsFrom(typeof(Dictionary<,>));
#endif
        }


#else
        public static SerializedProperty GetUnityIterator(this SerializedObject so)
        {
            return so.GetIterator();
        }
#endif

        public static bool ValidType(Type type)
        {
            return !typeof(Component).IsAssignableFrom(type);
        }

        public static bool TypeIsImporter(Type type)
        {
            return typeof(AssetImporter).IsAssignableFrom(type);
        }

        public static string Path(this Object o)
        {
            return AssetDatabase.GetAssetPath(o);
        }
        public static string GUID(this Object o)
        {
            return AssetDatabase.AssetPathToGUID(o.Path());
        }
        public static AssetID ToAssetID(this Object o)
        {
            return AssetID.Get(o);
        }

        public static string ToPath(this string guid)
        {
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        public static GUIContent GetLabel(this SerializedProperty property, bool useDisplayName = true)
        {
            return new GUIContent(useDisplayName ? property.displayName : property.name, property.tooltip);
        }

        public static bool Next(this SerializedProperty prop, bool enterChildren, bool visibleOnly)
        {
            if (visibleOnly)
                return prop.NextVisible(enterChildren);
            else
                return prop.Next(enterChildren);
        }

        public static bool NextChildSkipArray(this SerializedProperty prop, bool visibleOnly = false)
        {
            if (visibleOnly)
            {
                return prop.NextVisible(true);
            }
            else
            {
                if (S.S.skipArrayDotArraySubProperty && prop.isArray)
                {
                    if (!prop.Next(true))
                        return false;
                }
                return prop.Next(true);
            }
        }

        public static bool HasChildren(SerializedProperty prop, bool visibleOnly = false)
        {
            if (!prop.hasChildren)
                return false;

            // Welp, no way to distinguish between Color and Color32...
            //if (prop.propertyType == SerializedPropertyType.Color)
            //    Debug.Log(prop.type + " " + prop.propertyPath);

            if (prop.hasVisibleChildren)
            {
                if (S.S.considerSeparate.Contains(prop))
                    return true;

                return !S.S.considerWhole.Contains(prop);
            }
            else
            {
                if (visibleOnly)
                    return false;

                if (S.S.considerSeparate.Contains(prop))
                    return true;

                if (S.S.considerWholeAllInvisibleChildren)
                    return false;

                return !S.S.considerWhole.Contains(prop);
            }
        }
    }
}

#endif
