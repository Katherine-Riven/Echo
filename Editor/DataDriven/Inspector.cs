/*using System;
using Lucifer.Editor;
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
            DrawVariables();
            Tree.BeginDraw(true);
            foreach (InspectorProperty child in Tree.RootProperty.Children)
            {
                Type childType = child.Info.TypeOfValue;
                if (childType.IsConstructedGenericType)
                {
                    if (childType == typeof(Parameter<>).MakeGenericType(childType.GenericTypeArguments))
                    {
                        DrawParameter(child);
                        continue;
                    }

                    if (childType == typeof(Behaviour<>).MakeGenericType(childType.GenericTypeArguments))
                    {
                        DrawBehaviour(child);
                        continue;
                    }
                }

                child.Draw();
            }

            Tree.EndDraw();
        }

        private void DrawVariables()
        {
            SerializedProperty variables = serializedObject.FindProperty("m_Variables");


            SirenixEditorGUI.EndVerticalList();
        }

        private void DrawParameter(InspectorProperty property)
        {
            
        }

        private void DrawBehaviour(InspectorProperty property)
        {
        }

        private void ShowDropdownMenu(Type parentType)
        {
        }
    }
}*/