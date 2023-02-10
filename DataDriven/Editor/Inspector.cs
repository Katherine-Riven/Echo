using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Lucifer.DataDriven.Editor
{
    [CustomEditor(typeof(DataDrivenObject), true)]
    sealed class Inspector : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Open Graph"))
            {
                GraphEditWindow.Open((DataDrivenObject) target);
            }

            SerializedProperty variables = serializedObject.FindProperty("m_Variables");
            SirenixEditorGUI.BeginHorizontalToolbar();
            SirenixEditorGUI.Title("Variable Blackboard", string.Empty, TextAlignment.Center, false);
            SirenixEditorGUI.EndHorizontalToolbar();
            SirenixEditorGUI.BeginVerticalList();
            for (int i = 0; i < variables.arraySize; i++)
            {
                SerializedProperty variable      = variables.GetArrayElementAtIndex(i);
                SerializedProperty variableName  = variable.FindPropertyRelative("m_Name");
                SerializedProperty variableValue = variable.FindPropertyRelative("m_Value");
                SirenixEditorGUI.BeginListItem(false);
                EditorGUILayout.PropertyField(variableValue, GUIHelper.TempContent(variableName.stringValue));
                SirenixEditorGUI.EndListItem();
            }

            SirenixEditorGUI.EndVerticalList();
        }
    }
}