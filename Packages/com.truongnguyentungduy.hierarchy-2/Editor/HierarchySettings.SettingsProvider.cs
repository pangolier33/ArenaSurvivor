using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy2
{
    internal partial class HierarchySettings
    {
        [SettingsProvider]
        static SettingsProvider UIElementSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Hierarchy", SettingsScope.Project)
            {
                label = "Hierarchy",

                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = GetAssets();

                    float TITLE_MARGIN_TOP = 14;
                    float TITLE_MARGIN_BOTTOM = 8;
                    float CONTENT_MARGIN_LEFT = 10;

                    HorizontalLayout horizontalLayout = new HorizontalLayout();
                    horizontalLayout.style.backgroundColor = new Color(0, 0, 0, 0.2f);
                    horizontalLayout.style.paddingTop = 4;
                    horizontalLayout.style.paddingBottom = 10;
                    rootElement.Add(horizontalLayout);

                    Label hierarchyTitle = new Label("Hierarchy");
                    hierarchyTitle.StyleFontSize(20);
                    hierarchyTitle.StyleMargin(10, 0, 2, 2);
                    hierarchyTitle.StyleFont(FontStyle.Bold);
                    horizontalLayout.Add(hierarchyTitle);

                    Label importButton = new Label();
                    importButton.text = "  Import";
                    importButton.style.unityFontStyleAndWeight = FontStyle.Italic;
                    Color importExportButtonColor = new Color32(102, 157, 246, 255);
                    importButton.style.color = importExportButtonColor;
                    importButton.RegisterCallback<PointerUpEvent>(evt => instance.ImportFromJson());
                    horizontalLayout.Add(importButton);

                    Label exportButton = new Label();
                    exportButton.text = "| Export";
                    exportButton.style.unityFontStyleAndWeight = FontStyle.Italic;
                    exportButton.style.color = importExportButtonColor;
                    exportButton.RegisterCallback<PointerUpEvent>(evt => instance.ExportToJson());
                    horizontalLayout.Add(exportButton);

                    ScrollView scrollView = new ScrollView();
                    rootElement.Add(scrollView);

                    VerticalLayout verticalLayout = new VerticalLayout();
                    verticalLayout.StylePadding(8, 8, 8, 8);
                    scrollView.Add(verticalLayout);

                    var Object = new Label("Object");
                    Object.StyleFont(FontStyle.Bold);
                    Object.StyleMargin(0, 0, 0, TITLE_MARGIN_BOTTOM);
                    verticalLayout.Add(Object);

                    var displayCustomObjectIcon = new Toggle("Display Custom Icon");
                    displayCustomObjectIcon.value = settings.displayCustomObjectIcon;
                    displayCustomObjectIcon.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayCustomObjectIcon = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayCustomObjectIcon));
                    });

                    displayCustomObjectIcon.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayCustomObjectIcon);

                    var View = new Label("View");
                    View.StyleFont(FontStyle.Bold);
                    View.StyleMargin(0, 0, TITLE_MARGIN_TOP, TITLE_MARGIN_BOTTOM);
                    verticalLayout.Add(View);

                    var displayRowBackground = new Toggle("Display RowBackground");
                    displayRowBackground.value = settings.displayRowBackground;
                    displayRowBackground.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayRowBackground = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayRowBackground));
                    });

                    displayRowBackground.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayRowBackground);

                    var displayTreeView = new Toggle("Display TreeView");
                    displayTreeView.value = settings.displayTreeView;
                    displayTreeView.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayTreeView = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayTreeView));
                    });

                    displayTreeView.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayTreeView);

                    var displayGrid = new Toggle("Display Grid");
                    displayGrid.value = settings.displayGrid;
                    displayGrid.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayGrid = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayGrid));
                    });

                    displayGrid.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayGrid);

                    var Components = new Label("Components");
                    Components.StyleFont(FontStyle.Bold);
                    Components.StyleMargin(0, 0, TITLE_MARGIN_TOP, TITLE_MARGIN_BOTTOM);
                    verticalLayout.Add(Components);

                    var displayComponents = new Toggle("Display Components Icon");
                    displayComponents.value = settings.displayComponents;
                    displayComponents.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayComponents = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayComponents));
                    });

                    displayComponents.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayComponents);

                    var componentAlignment = new EnumField(settings.componentAlignment);
                    componentAlignment.label = "Component Alignment";
                    componentAlignment.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.componentAlignment = (ElementAlignment) evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.componentAlignment));
                    });

                    componentAlignment.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(componentAlignment);

                    var componentDisplayMode = new EnumField(settings.componentDisplayMode);
                    componentDisplayMode.label = "Component Display Mode";
                    componentDisplayMode.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(componentDisplayMode);

                    var componentListInput = new TextField("Components");
                    componentListInput.multiline = true;
                    //componentListInput.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
                    componentListInput.value = string.Join(" ", settings.components);
                    componentListInput.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(componentListInput);
                    componentListInput.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.components = evt.newValue.Split(' ');
                        settings.OnSettingsChanged(nameof(settings.components));
                    });

                    componentDisplayMode.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.componentDisplayMode = (ComponentDisplayMode) evt.newValue;
                        switch (settings.componentDisplayMode)
                        {
                            case ComponentDisplayMode.Specified:
                                componentListInput.StyleDisplay(true);
                                break;

                            case ComponentDisplayMode.Ignore:
                                componentListInput.StyleDisplay(true);
                                break;

                            case ComponentDisplayMode.All:
                                componentListInput.StyleDisplay(false);
                                break;

                            case ComponentDisplayMode.ScriptOnly:
                                componentListInput.StyleDisplay(false);
                                break;
                        }

                        settings.OnSettingsChanged(nameof(settings.componentDisplayMode));
                    });

                    var ignoreComponentBase = new Toggle("Ignore Component Base");
                    ignoreComponentBase.value = settings.ignoreComponentBase;
                    ignoreComponentBase.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(ignoreComponentBase);

                    var baseComponents = new TextField("Base Components");
                    baseComponents.value = string.Join(" ", settings.baseComponents);
                    baseComponents.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    baseComponents.style.display = settings.ignoreComponentBase ? DisplayStyle.Flex : DisplayStyle.None;
                    verticalLayout.Add(baseComponents);
                    baseComponents.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.baseComponents = evt.newValue.Split(' ');
                        settings.OnSettingsChanged(nameof(settings.baseComponents));
                    });

                    ignoreComponentBase.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        baseComponents.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;

                        settings.ignoreComponentBase = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.ignoreComponentBase));
                    });

                    var componentSizeEnum = ComponentSize.Normal;
                    switch (settings.componentSize)
                    {
                        case 12:
                            componentSizeEnum = ComponentSize.Small;
                            break;

                        case 14:
                            componentSizeEnum = ComponentSize.Normal;
                            break;

                        case 16:
                            componentSizeEnum = ComponentSize.Large;
                            break;
                    }

                    var componentSize = new EnumField(componentSizeEnum);
                    componentSize.label = "Component Size";
                    componentSize.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    componentSize.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        switch (evt.newValue)
                        {
                            case ComponentSize.Small:
                                settings.componentSize = 12;
                                break;

                            case ComponentSize.Normal:
                                settings.componentSize = 14;
                                break;

                            case ComponentSize.Large:
                                settings.componentSize = 16;
                                break;
                        }

                        settings.OnSettingsChanged(nameof(settings.componentSize));
                    });

                    verticalLayout.Add(componentSize);

                    var componentSpacing = new IntegerField();
                    componentSpacing.label = "Component Spacing";
                    componentSpacing.value = settings.componentSpacing;
                    componentSpacing.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    componentSpacing.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.componentSpacing = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.componentSpacing));
                    });

                    verticalLayout.Add(componentSpacing);

                    var TagAndLayer = new Label("Tag And Layer");
                    TagAndLayer.StyleFont(FontStyle.Bold);
                    TagAndLayer.StyleMargin(0, 0, TITLE_MARGIN_TOP, TITLE_MARGIN_BOTTOM);
                    verticalLayout.Add(TagAndLayer);

                    var displayTag = new Toggle("Display Tag");
                    displayTag.value = settings.displayTag;
                    displayTag.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayTag = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayTag));
                    });

                    displayTag.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayTag);

                    var applyTagTargetAndChild = new Toggle("Tag Recursive Change");
                    applyTagTargetAndChild.value = settings.applyTagTargetAndChild;
                    applyTagTargetAndChild.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.applyTagTargetAndChild = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.applyTagTargetAndChild));
                    });

                    applyTagTargetAndChild.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(applyTagTargetAndChild);

                    var tagAlignment = new EnumField(settings.tagAlignment);
                    tagAlignment.label = "Tag Alignment";
                    tagAlignment.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.tagAlignment = (ElementAlignment) evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.tagAlignment));
                    });

                    tagAlignment.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(tagAlignment);

                    var displayLayer = new Toggle("Display Layer");
                    displayLayer.value = settings.displayLayer;
                    displayLayer.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.displayLayer = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.displayLayer));
                    });

                    displayLayer.style.marginTop = 8;
                    displayLayer.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(displayLayer);

                    var applyLayerTargetAndChild = new Toggle("Layer Recursive Change");
                    applyLayerTargetAndChild.value = settings.applyLayerTargetAndChild;
                    applyLayerTargetAndChild.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.applyLayerTargetAndChild = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.applyLayerTargetAndChild));
                    });

                    applyLayerTargetAndChild.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(applyLayerTargetAndChild);

                    var layerAlignment = new EnumField(settings.layerAlignment);
                    layerAlignment.label = "Layer Alignment";
                    layerAlignment.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.layerAlignment = (ElementAlignment) evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.layerAlignment));
                    });

                    layerAlignment.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(layerAlignment);

                    var advanced = new Label("Advanced");
                    advanced.StyleFont(FontStyle.Bold);
                    advanced.StyleMargin(0, 0, TITLE_MARGIN_TOP, TITLE_MARGIN_BOTTOM);
                    verticalLayout.Add(advanced);

                    var separatorStartWith = new TextField();
                    separatorStartWith.label = "Separator StartWith";
                    separatorStartWith.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    separatorStartWith.value = settings.separatorStartWith;
                    separatorStartWith.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.separatorStartWith = evt.newValue == string.Empty ? "--->" : evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.separatorStartWith));
                    });

                    verticalLayout.Add(separatorStartWith);

                    var headerDefaultTag = new TagField();
                    headerDefaultTag.label = "Separator Default Tag";
                    headerDefaultTag.value = settings.separatorDefaultTag;
                    headerDefaultTag.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.separatorDefaultTag = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.separatorDefaultTag));
                    });

                    headerDefaultTag.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    headerDefaultTag.StyleMarginBottom(4);
                    verticalLayout.Add(headerDefaultTag);

                    SerializedObject serializedSetting = new SerializedObject(settings);
                    IMGUIContainer instantBackgroundIMGUI = new IMGUIContainer(() =>
                    {
                        EditorGUILayout.BeginHorizontal();
                        settings.useInstantBackground =
                            EditorGUILayout.Toggle("Use Instant Background", settings.useInstantBackground);

                        EditorGUILayout.BeginVertical("helpbox");
                        EditorGUILayout.PropertyField(serializedSetting.FindProperty(nameof(instantBackgroundColors)),
                            GUIContent.none);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        serializedSetting.ApplyModifiedProperties();
                    });

                    instantBackgroundIMGUI.StyleMarginTop(7);
                    instantBackgroundIMGUI.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(instantBackgroundIMGUI);

                    var onlyDisplayWhileMouseHovering = new Toggle("Display Hovering");
                    onlyDisplayWhileMouseHovering.tooltip = "Only display while mouse hovering";
                    onlyDisplayWhileMouseHovering.StyleMarginTop(7);
                    onlyDisplayWhileMouseHovering.value = settings.onlyDisplayWhileMouseEnter;
                    onlyDisplayWhileMouseHovering.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.onlyDisplayWhileMouseEnter = evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.onlyDisplayWhileMouseEnter));
                    });

                    onlyDisplayWhileMouseHovering.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                    verticalLayout.Add(onlyDisplayWhileMouseHovering);

                    var contentMaskEnumFlags = new EnumFlagsField(settings.contentDisplay);
                    contentMaskEnumFlags.StyleDisplay(onlyDisplayWhileMouseHovering.value);
                    contentMaskEnumFlags.label = "Content Mask";
                    onlyDisplayWhileMouseHovering.RegisterValueChangedCallback((evt) =>
                    {
                        contentMaskEnumFlags.StyleDisplay(evt.newValue);
                    });

                    contentMaskEnumFlags.RegisterValueChangedCallback((evt) =>
                    {
                        Undo.RecordObject(settings, "Change Settings");

                        settings.contentDisplay = (ContentDisplay) evt.newValue;
                        settings.OnSettingsChanged(nameof(settings.contentDisplay));
                    });

                    contentMaskEnumFlags.style.marginLeft = CONTENT_MARGIN_LEFT;
                    verticalLayout.Add(contentMaskEnumFlags);

                    var Theme = new Label("Theme");
                    Theme.StyleFont(FontStyle.Bold);
                    Theme.StyleMargin(0, 0, TITLE_MARGIN_TOP, TITLE_MARGIN_BOTTOM);
                    verticalLayout.Add(Theme);

                    if (EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        EditorHelpBox themeWarningPlaymode =
                            new EditorHelpBox("This setting only available on edit mode.", MessageType.Info);

                        verticalLayout.Add(themeWarningPlaymode);
                    }
                    else
                    {
                        EditorHelpBox selectionColorHelpBox = new EditorHelpBox(
                            "Theme selection color require editor assembly recompile to take affect.\nBy selecting any script, right click -> Reimport. it will force the editor to recompile.",
                            MessageType.Info);

                        selectionColorHelpBox.StyleDisplay(false);
                        verticalLayout.Add(selectionColorHelpBox);

                        ColorField colorRowEven = new ColorField("Row Even");
                        colorRowEven.value = settings.usedTheme.colorRowEven;
                        colorRowEven.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorRowEven.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorRowEven = evt.newValue;
                            else
                                settings.personalTheme.colorRowEven = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorRowEven);

                        ColorField colorRowOdd = new ColorField("Row Odd");
                        colorRowOdd.value = settings.usedTheme.colorRowOdd;
                        colorRowOdd.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorRowOdd.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorRowOdd = evt.newValue;
                            else
                                settings.personalTheme.colorRowOdd = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorRowOdd);

                        ColorField colorGrid = new ColorField("Grid Color");
                        colorGrid.value = settings.usedTheme.colorGrid;
                        colorGrid.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorGrid.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorGrid = evt.newValue;
                            else
                                settings.personalTheme.colorGrid = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorGrid);

                        ColorField colorTreeView = new ColorField("TreeView");
                        colorTreeView.value = settings.usedTheme.colorTreeView;
                        colorTreeView.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorTreeView.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorTreeView = evt.newValue;
                            else
                                settings.personalTheme.colorTreeView = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorTreeView);

                        ColorField colorLockIcon = new ColorField("Lock Icon");
                        colorLockIcon.value = settings.usedTheme.colorLockIcon;
                        colorLockIcon.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorLockIcon.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorLockIcon = evt.newValue;
                            else
                                settings.personalTheme.colorLockIcon = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorLockIcon);

                        ColorField tagColor = new ColorField("Tag Text");
                        tagColor.value = settings.usedTheme.tagColor;
                        tagColor.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        tagColor.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.tagColor = evt.newValue;
                            else
                                settings.personalTheme.tagColor = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(tagColor);

                        ColorField layerColor = new ColorField("Layer Text");
                        layerColor.value = settings.usedTheme.layerColor;
                        layerColor.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        layerColor.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.layerColor = evt.newValue;
                            else
                                settings.personalTheme.layerColor = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(layerColor);

                        ColorField colorHeaderTitle = new ColorField("Header Title");
                        colorHeaderTitle.value = settings.usedTheme.colorHeaderTitle;
                        colorHeaderTitle.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorHeaderTitle.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorHeaderTitle = evt.newValue;
                            else
                                settings.personalTheme.colorHeaderTitle = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorHeaderTitle);

                        ColorField colorHeaderBackground = new ColorField("Header Background");
                        colorHeaderBackground.value = settings.usedTheme.colorHeaderBackground;
                        colorHeaderBackground.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        colorHeaderBackground.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.colorHeaderBackground = evt.newValue;
                            else
                                settings.personalTheme.colorHeaderBackground = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(colorHeaderBackground);

                        ColorField comSelBGColor = new ColorField("Component Selection");
                        comSelBGColor.value = settings.usedTheme.comSelBGColor;
                        comSelBGColor.StyleMarginLeft(CONTENT_MARGIN_LEFT);
                        comSelBGColor.RegisterValueChangedCallback((evt) =>
                        {
                            Undo.RecordObject(settings, "Change Settings");

                            if (EditorGUIUtility.isProSkin)
                                settings.professionalTheme.comSelBGColor = evt.newValue;
                            else
                                settings.personalTheme.comSelBGColor = evt.newValue;

                            settings.OnSettingsChanged();
                        });

                        verticalLayout.Add(comSelBGColor);
                    }

                    Undo.undoRedoPerformed -= OnUndoRedoPerformed;
                    Undo.undoRedoPerformed += OnUndoRedoPerformed;
                },

                deactivateHandler = () => Undo.undoRedoPerformed -= OnUndoRedoPerformed,

                keywords = new HashSet<string>(new[] { "Hierarchy" })
            };

            return provider;
        }
    }
}