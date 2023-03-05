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
    abstract class AbilityEffectsDrawer<T> : OdinValueDrawer<T>
    {
        private InspectorProperty ArrayProperty;

        protected abstract IReadOnlyList<GenericSelectorItem<Type>> SelectorItems { get; }

        protected override void Initialize()
        {
            base.Initialize();
            ArrayProperty = Property.FindChild(x => x.Name == "m_Effects", false);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            Action nextCustomAddFunction = CollectionDrawerStaticInfo.NextCustomAddFunction;
            CollectionDrawerStaticInfo.NextCustomAddFunction = AddNewVariable;
            ArrayProperty.Draw(label);
            CollectionDrawerStaticInfo.NextCustomAddFunction = nextCustomAddFunction;
        }

        private void AddNewVariable()
        {
            GenericSelector<Type> selector = new GenericSelector<Type>(string.Empty, false, SelectorItems);
            selector.EnableSingleClickToSelect();
            selector.SelectionConfirmed += delegate(IEnumerable<Type> types)
            {
                Type type = types.FirstOrDefault();
                if (type == null)
                {
                    return;
                }

                object              newVariable = Activator.CreateInstance(type);
                ICollectionResolver resolver    = (ICollectionResolver) ArrayProperty.ChildResolver;
                resolver.QueueAdd(new object[] {newVariable});
                resolver.ApplyChanges();
                ArrayProperty.ValueEntry.ApplyChanges();
            };

            selector.ShowInPopup(Event.current.mousePosition);
        }
    }

    sealed class AbilityEffectsDrawer : AbilityEffectsDrawer<AbilityEffects>
    {
        protected override IReadOnlyList<GenericSelectorItem<Type>> SelectorItems => AbilityDrawerUtility.EffectItems;
    }

    sealed class AbilityCancelableEffectsDrawer : AbilityEffectsDrawer<AbilityCancelableEffects>
    {
        protected override IReadOnlyList<GenericSelectorItem<Type>> SelectorItems => AbilityDrawerUtility.CancelableEffectItems;
    }

    [DrawerPriority(value: 2000)]
    sealed class AbilityEffectDrawer : OdinValueDrawer<AbilityEffect>
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