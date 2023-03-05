using System;
using System.Collections.Generic;
using System.Linq;
using Echo;
using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EchoEditor.Abilities
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    sealed class AbilityFeatureArrayDrawer : OdinValueDrawer<AbilityFeature[]>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Action nextCustomAddFunction = CollectionDrawerStaticInfo.NextCustomAddFunction;
            CollectionDrawerStaticInfo.NextCustomAddFunction = AddNewVariable;
            CallNextDrawer(label);
            CollectionDrawerStaticInfo.NextCustomAddFunction = nextCustomAddFunction;
        }

        private void AddNewVariable()
        {
            GenericSelector<Type> selector = new GenericSelector<Type>(string.Empty, false, AbilityDrawerUtility.FeatureItems);
            selector.EnableSingleClickToSelect();
            selector.SelectionConfirmed += delegate(IEnumerable<Type> types)
            {
                Type type = types.FirstOrDefault();
                if (type == null)
                {
                    return;
                }

                object              newVariable = Activator.CreateInstance(type);
                ICollectionResolver resolver    = (ICollectionResolver) Property.ChildResolver;
                resolver.QueueAdd(new object[] {newVariable});
                resolver.ApplyChanges();
                Property.ValueEntry.ApplyChanges();
            };

            selector.ShowInPopup(Event.current.mousePosition);
        }
    }

    [DrawerPriority(value: 2000)]
    sealed class AbilityFeatureDrawer : OdinValueDrawer<AbilityFeature>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            string text    = AbilityMenuItemAttribute.GetMenuName(ValueEntry.SmartValue.GetType());
            string tooltip = ValueEntry.SmartValue.ToString();
            Property.State.Expanded = SirenixEditorGUI.Foldout(Property.State.Expanded, GUIHelper.TempContent(text, tooltip));
            if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(Property, this), Property.State.Expanded))
            {
                EditorGUI.indentLevel++;
                foreach (InspectorProperty child in Property.Children)
                {
                    child.Draw();
                }

                EditorGUI.indentLevel--;
            }

            SirenixEditorGUI.EndFadeGroup();
        }
    }
}