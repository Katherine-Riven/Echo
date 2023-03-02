using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Echo.DataDriven.Editor
{
    sealed class VariablesDrawer : OdinValueDrawer<IVariable[]>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            SirenixEditorGUI.Title("Variable Blackboard", string.Empty, TextAlignment.Center, false);
            SirenixEditorGUI.EndHorizontalToolbar();
            SirenixEditorGUI.BeginVerticalList();
            if (Property.Children.Count == 0)
            {
                SirenixEditorGUI.BeginListItem(false);
                EditorGUILayout.LabelField("There is no variable.");
                SirenixEditorGUI.EndListItem();
            }
            else
            {
                foreach (InspectorProperty variable in Property.Children)
                {
                    InspectorProperty variableName  = variable.Children[0];
                    InspectorProperty variableValue = variable.Children[1];
                    SirenixEditorGUI.BeginListItem(false);
                    variableValue.Draw(GUIHelper.TempContent((string) variableName.ValueEntry.WeakSmartValue));
                    SirenixEditorGUI.EndListItem();
                }
            }

            SirenixEditorGUI.EndVerticalList();
        }
    }
}