using System.Collections.Generic;
using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.Pool;

namespace EchoEditor.Abilities
{
    sealed class AbilityTagDrawer : OdinValueDrawer<AbilityTag>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GenericSelector<AbilityTag>.DrawSelectorDropdown(label, GUIHelper.TempContent(ValueEntry.SmartValue.ToString()), ShowSelector);
        }

        private GenericSelector<AbilityTag> ShowSelector(Rect rect)
        {
            GenericSelector<AbilityTag> selector = new GenericSelector<AbilityTag>(string.Empty, true, AbilityEditorUtility.TagItems);
            selector.CheckboxToggle   =  true;
            selector.SelectionChanged += SelectorOnSelectionChanged;

            AbilityTag       currentValue      = ValueEntry.SmartValue;
            List<AbilityTag> currentSelections = ListPool<AbilityTag>.Get();
            foreach (var item in AbilityEditorUtility.TagItems)
            {
                if (currentValue.HasAllTag(item.Value))
                {
                    currentSelections.Add(item.Value);
                }
            }

            selector.SetSelection(currentSelections);
            ListPool<AbilityTag>.Release(currentSelections);

            rect.x      = (int) rect.x;
            rect.y      = (int) rect.y;
            rect.width  = (int) rect.width;
            rect.height = (int) rect.height;
            selector.ShowInPopup(rect);
            return selector;

            void SelectorOnSelectionChanged(IEnumerable<AbilityTag> _)
            {
                AbilityTag value = AbilityTag.None;
                foreach (AbilityTag temp in selector.GetCurrentSelection())
                {
                    value |= temp;
                }

                if (ValueEntry.SmartValue == value)
                {
                    return;
                }

                ValueEntry.SmartValue = value;
                ValueEntry.ApplyChanges();
            }
        }
    }
}