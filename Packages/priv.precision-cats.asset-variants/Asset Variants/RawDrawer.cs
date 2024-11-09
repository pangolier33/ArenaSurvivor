#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AssetVariants
{
    public class RawDrawer : PropertyDrawer
    {
        public static bool currentlyDrawNiceNames;

        public PropertyDrawer original;

        protected virtual bool VisibleOnly => false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool visibleOnly = VisibleOnly;

            if (Helper.HasChildren(property, visibleOnly))
            {
                // Foldout
                float height = EditorGUIUtility.singleLineHeight;

                // Children
                int childCount = 0;
                if (property.isExpanded)
                {
                    using (var prop = property.Copy())
                    {
                        int depth = prop.depth;
                        if (prop.NextChildSkipArray(visibleOnly))
                        {
                            do
                            {
                                var childLabel = prop.GetLabel(currentlyDrawNiceNames);
                                height += GetPropertyHeight(prop, childLabel);
                                childCount++;
                            }
                            while (prop.Next(false, visibleOnly) && depth < prop.depth);
                        }
                    }
                }
                height += childCount * EditorGUIUtility.standardVerticalSpacing;

                return height;
            }
            else
            {
                if (original != null)
                    return original.GetPropertyHeight(property, property.GetLabel(currentlyDrawNiceNames));
                else
                    return EditorGUI.GetPropertyHeight(property, property.GetLabel(currentlyDrawNiceNames), true);
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool visibleOnly = VisibleOnly;

            //using (var prop = property.Copy())
            //{
            //    int depth = prop.depth;
            //    if (prop.NextChildSkipArray(visibleOnly))
            //    {
            //        do
            //        {
            //            Debug.Log("CHILD PROPERTY: " + prop.propertyPath);
            //        }
            //        while (prop.Next(false) && depth < prop.depth);
            //    }
            //}

            if (Helper.HasChildren(property, visibleOnly))
            {
                float svs = EditorGUIUtility.standardVerticalSpacing;

                // Foldout
                Rect rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginProperty(rect, label, property);
                bool guiChanged = GUI.changed;
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
                GUI.changed = guiChanged; //TODO3: ? Would any code ever care?
                EditorGUI.EndProperty();
                rect.y += rect.height + svs;

                // Children
                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    using (var prop = property.Copy())
                    {
                        int depth = prop.depth;
                        if (prop.NextChildSkipArray(visibleOnly))
                        {
                            do
                            {
                                var childLabel = prop.GetLabel(currentlyDrawNiceNames);
                                rect.height = GetPropertyHeight(prop, childLabel);
                                OnGUI(rect, prop, childLabel);
                                rect.y += rect.height + svs;
                            }
                            while (prop.Next(false, visibleOnly) && depth < prop.depth);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (original != null)
                    original.OnGUI(position, property, property.GetLabel(currentlyDrawNiceNames));
                else
                    EditorGUI.PropertyField(position, property, property.GetLabel(currentlyDrawNiceNames), true); //label
            }
        }
    }
}
#endif
