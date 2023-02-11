using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Lucifer.Editor
{
    class GUIStyleOverview : EditorWindow
    {
        [MenuItem("Lucifer/GUIStyle Overview")]
        static void Open()
        {
            GetWindow<GUIStyleOverview>("GUIStyle Overview").Show();
        }

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            SirenixEditorGUI.BeginVerticalList();
            foreach (GUIStyle style in GUI.skin)
            {
                SirenixEditorGUI.BeginListItem(false);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(style.name);
                GUILayout.Button(style.name, style);
                EditorGUILayout.EndHorizontal();
                SirenixEditorGUI.EndListItem();
            }

            SirenixEditorGUI.EndVerticalList();
            EditorGUILayout.EndScrollView();
        }
    }
}