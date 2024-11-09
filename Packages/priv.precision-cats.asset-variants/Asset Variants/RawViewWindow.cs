#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using PrecisionCats;

namespace AssetVariants
{
    public class RawViewWindow : EditorWindow
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (CurrentlyDrawing)
                return;

            if (AVSettings.S.disabledRawViewPopup)
                return;

            if (!AVFilter.Should(property.serializedObject.targetObject))
                return;

            var prop = property.Copy();

            menu.AddItem(new GUIContent("Edit in Raw View"), false, () =>
            {
                Popup(prop);
            });

            menu.AddSeparator("");
        }

        public static bool CurrentlyDrawing { get; private set; }

        public static System.Action<RawViewWindow> setup;

        public static bool onlyIMGUI = false;

        public SerializedProperty serializedProperty;
        [System.NonSerialized]
        public Editor editor;
        public System.Action additional;
        public bool autoClose;
        public bool autoHeight;

        public static void Popup(SerializedProperty serializedProperty)
        {
            serializedProperty.isExpanded = true;

            setup = (w) =>
            {
                w.serializedProperty = serializedProperty;
                w.editor = null;
                w.autoHeight = true;
                w.Focus();
                w.autoClose = true;
            };
            var window = CreateWindow<RawViewWindow>("Raw View");
            setup = null;
        }

        private float imguiHeight;

        private void Update()
        {
            if ((serializedProperty == null && editor == null) || (autoClose && focusedWindow != this))
            {
                Close();
                return;
            }

            if (autoHeight)
            {
                var h = container != null ? container.layout.height : imguiHeight;
                minSize = new Vector2(minSize.x, h);
                //maxSize = new Vector2(maxSize.x, h);
            }

            Repaint(); //TODO:?
        }

        public Vector2 scrollPosition;

        private void Awake()
        {
            if (setup != null)
                setup(this);
            else
                Close();
        }

        private void OnGUI()
        {
            //Debug.Log("OnGUI");

            RawDrawer.currentlyDrawNiceNames = AVSettings.currentlyRawViewDrawNiceNames();

            if (container == null)
            {
                if (!autoHeight)
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                var r = EditorGUILayout.BeginVertical();
                imguiHeight = r.height;

                int l = AVSettings.S.rawViewWindowLeftPadding;

                if (serializedProperty != null)
                {
                    Draw(serializedProperty, l);
                }

                if (editor != null)
                {
                    var iterator = editor.serializedObject.GetIterator();
                    if (iterator.NextVisible(true))
                    {
                        do
                        {
                            var prop = iterator.Copy();
                            Draw(prop, l);
                            prop.Dispose();
                        }
                        while (iterator.Next(false));
                    }
                }

                if (additional != null)
                {
                    additional();
                }

                if (!autoHeight)
                    EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            if (autoClose)
                if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
                    Close();
        }

        private static readonly RawDrawer rawDrawer = new RawDrawer();

        private static void Draw(SerializedProperty prop, int l)
        {
            bool prevHierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = true; //?
            {
                CurrentlyDrawing = true;
                try
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        var label = Helper.GetLabel(prop, RawDrawer.currentlyDrawNiceNames);
                        float height = rawDrawer.GetPropertyHeight(prop, label);
                        var rect = EditorGUILayout.GetControlRect(true, height);
                        rect.x += l;
                        rect.width -= l;
                        rawDrawer.OnGUI(rect, prop, label);
                    }
                    if (EditorGUI.EndChangeCheck())
                        prop.serializedObject.ApplyModifiedProperties();
                }
                finally
                {
                    CurrentlyDrawing = false;
                }
            }
            EditorGUIUtility.hierarchyMode = prevHierarchyMode;

            //TODO: update UI Toolkit's OverrideButtons?
            // An event in here for onEndChangeCheck
        }

        private VisualElement container = null;

        private void CreateGUI()
        {
            if (onlyIMGUI)
                return;

            //Debug.Log("Actually called");

            var sss = AVSettings.S.overrideIndicatorStyle;

            if (!autoHeight)
                container = new ScrollView(ScrollViewMode.Vertical);
            else
                container = new VisualElement();
            rootVisualElement.Add(container);

            int l = AVSettings.S.rawViewWindowLeftPadding;

            if (serializedProperty != null)
            {
                var c = new IMGUIContainer(() =>
                {
                    Draw(serializedProperty, l);
                });
                c.style.marginBottom = EditorGUIUtility.standardVerticalSpacing;
                container.Add(c);
            }

            if (editor != null)
            {
                var iterator = editor.serializedObject.GetIterator();
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        var prop = iterator.Copy();
                        var c = new IMGUIContainer(() =>
                        {
                            if (editor != null)
                                Draw(prop, l);
                        });
                        c.style.marginBottom = EditorGUIUtility.standardVerticalSpacing;
                        container.Add(c);
                    }
                    while (iterator.Next(false));
                }

                container.Add(new IMGUIContainer(() =>
                {
                    if (editor != null)
                        editor.serializedObject.UpdateIfRequiredOrScript();
                }));
            }

            if (additional != null)
            {
                var c = new IMGUIContainer(() =>
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(l);
                    EditorGUILayout.BeginVertical();

                    if (editor != null)
                        additional();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                });
                container.Add(c);
            }

            container.Add(new IMGUIContainer(() => { OnGUI(); })); //TODO:??
        }
    }
}
#endif
