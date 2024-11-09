using UnityEditor;
using UnityEngine;

namespace Bones.Gameplay.Effects.Provider.Debug
{
	public class SpellDebugWindow : EditorWindow, IHasCustomMenu
	{
		private Vector2 _scrollPos;
		private MonoScript _script;

		[MenuItem("Bones/Window/Spell Debug")]
		public static void Open()
		{
			var window = GetWindow<SpellDebugWindow>();
			window.Show();
		}

		private void OnEnable()
		{
			_script = MonoScript.FromScriptableObject(this);
			titleContent = new GUIContent("Spell Debug", "Spell | ITrace | IEffect | Stat realtime view values");
		}

		private GUIStyle _style;

		private void OnGUI()
		{
			_style ??= new GUIStyle("label");
			_style.stretchHeight = true;
			_style.alignment = TextAnchor.UpperLeft;
			EditorGUILayout.HelpBox("Просматриваем текущие значения способностей (Spell | ITrace | IEffect | Stat) игрока, во время геймплея", MessageType.Info);
			EditorGUILayout.ObjectField(_script, typeof(MonoScript), false);

			if (GUILayout.Button("Clear Current"))
			{
				DebugNode.ClearCurrent();
			}
			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			DrawInspectorForNode(DebugNode.Root, 1);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndScrollView();
		}

		private void DrawInspectorForNode(DebugNode node, int indent = 1, int currentDepth = int.MaxValue)
		{
			string prefix = $"[{node.Value?.GetType().Name}] ";
			string text = node.Value?.ToString() ?? "root";

			if (node.Children.Count == 0)
			{
				LabelField(prefix, text);
				return;
			}

			var foldoutTitle = text;
			if (foldoutTitle.Contains('\n'))
			{
				foldoutTitle = foldoutTitle.Split('\n')[0];
			}
			if (foldoutTitle != text)
			{
				LabelField(prefix, text);
			}
			node.Foldout = EditorGUILayout.Foldout(node.Foldout, foldoutTitle);

			if (!node.Foldout)
				return;

			// if (currentDepth <= 0)
			// {
			// 	return;
			// }

			foreach (var child in node.Children)
			{
				EditorGUI.indentLevel = indent;
				DrawInspectorForNode(child, indent + 1, currentDepth - 1);
			}
		}

		private void LabelField(string prefix, string text)
		{
			if (string.IsNullOrEmpty(prefix))
			{
				EditorGUILayout.LabelField(text, _style, GUILayout.MaxHeight(5000));
			}
			else
			{
				EditorGUILayout.LabelField(prefix + text, _style, GUILayout.MaxHeight(5000));
			}
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Edit Script"), true, OpenScript);
		}

		protected void OpenScript()
		{
			AssetDatabase.OpenAsset(MonoScript.FromScriptableObject(this));
		}
	}
}