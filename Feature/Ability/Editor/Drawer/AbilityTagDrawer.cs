using System.Collections.Generic;
using Echo;
using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;

namespace EchoEditor.Abilities
{
    sealed class AbilityTagDrawer : OdinValueDrawer<AbilityTag>
    {
        private GenericSelector<AbilityTag> m_Selector;

        private AbilityTag m_Tag => ValueEntry.SmartValue;

        protected override void Initialize()
        {
            base.Initialize();
            m_Selector                = new GenericSelector<AbilityTag>(string.Empty, true, AbilityDrawerUtility.TagItems);
            m_Selector.CheckboxToggle = true;

            AbilityTag       currentValue = m_Tag;
            List<AbilityTag> selections   = ListPool<AbilityTag>.Get();
            foreach (var item in AbilityDrawerUtility.TagItems)
            {
                if (currentValue.HasAllTag(item.Value))
                {
                    selections.Add(item.Value);
                }
            }

            m_Selector.SetSelection(selections);
            ListPool<AbilityTag>.Release(selections);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GenericSelector<AbilityTag>.DrawSelectorDropdown(label, GUIHelper.TempContent(m_Tag.ToString()), ShowSelector);
            IEnumerable<AbilityTag> selections = m_Selector.GetCurrentSelection();
            AbilityTag              value      = AbilityTag.None;
            foreach (var temp in selections)
            {
                value |= temp;
            }

            if (m_Tag == value)
            {
                return;
            }

            ValueEntry.SmartValue = value;
            ValueEntry.ApplyChanges();
        }

        private GenericSelector<AbilityTag> ShowSelector(Rect rect)
        {
            GenericSelector<AbilityTag> selector = m_Selector;
            rect.x      = (int) rect.x;
            rect.y      = (int) rect.y;
            rect.width  = (int) rect.width;
            rect.height = (int) rect.height;
            selector.ShowInPopup(rect);
            return selector;
        }
    }
}