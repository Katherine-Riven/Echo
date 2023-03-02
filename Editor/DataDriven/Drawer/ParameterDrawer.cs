using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Echo.DataDriven.Editor
{
    sealed class ParameterDrawer<T> : OdinValueDrawer<Parameter<T>>
    {
        private static GUIStyle ReferenceValueStyle;

        protected override void Initialize()
        {
            base.Initialize();
            if (ReferenceValueStyle == null)
            {
                ReferenceValueStyle                  = "ObjectFieldButton";
                ReferenceValueStyle.normal.textColor = ReferenceValueStyle.hover.textColor;
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            InspectorProperty referenceValue = Property.Children[0];
            InspectorProperty defaultValue   = Property.Children[1];
            if (referenceValue.ValueEntry.WeakSmartValue != null)
            {
                EditorGUILayout.PrefixLabel(label);
                if (GUILayout.Button(referenceValue.ValueEntry.WeakSmartValue.GetType().Name, ReferenceValueStyle))
                {
                }
            }
            else
            {
                defaultValue.Draw(label);
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.Refresh))
            {
                Type type = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .SelectMany(x => x.GetTypes())
                                     .First(x => x.IsAbstract == false && x.IsGenericType == false && typeof(IParameter<T>).IsAssignableFrom(x));
                referenceValue.ValueEntry.WeakSmartValue = Activator.CreateInstance(type);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}