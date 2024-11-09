using TMPro;
using UnityEditor;
using UnityEngine;
#if PROCEDURAL_UI_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
#endif

namespace Mitaywalle.ProceduralUI
{
    public abstract class LabeledWidget : Widget
    {
        [SerializeField] protected TMP_Text _label;

        public TMP_Text GetText() => _label;

#if PROCEDURAL_UI_LOCALIZATION
        [SerializeField] protected LocalizeStringEvent _localize;

        public void Set(string value, bool addIfNotFound = true)
        {
            LocalizedString localizeStringReference = _localize.StringReference;
            localizeStringReference.SetReference(localizeStringReference.TableReference, value);

            StringTable table = LocalizationSettings.StringDatabase.GetTable(localizeStringReference.TableReference,
                LocalizationSettings.SelectedLocale);

            if (!table.ContainsKey(localizeStringReference.TableEntryReference.KeyId))
            {
                _label.text = value;

                if (addIfNotFound)
                {
                    var sharedEntry = table.SharedData.AddKey(value);
                    table.AddEntry(sharedEntry.Key, value);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(table);
                    EditorUtility.SetDirty(table.SharedData);
#endif
                }
            }

            localizeStringReference.TableEntryReference = value;
        }
#else
        public void Set(string value, bool addIfNotFound = true)
        {
            _label.text = value;
        }
#endif
    }
}