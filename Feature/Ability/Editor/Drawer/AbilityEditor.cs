using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EchoEditor.Abilities
{
    [CustomEditor(typeof(AbilityProfile), true)]
    sealed class AbilityEditor : OdinEditor
    {
        private InspectorProperty m_AbilityProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_AbilityProperty = Tree.GetPropertyAtUnityPath("m_Ability");
        }

        protected override void DrawTree()
        {
            if (m_AbilityProperty == null)
            {
                base.DrawTree();
                return;
            }

            RectOffset margin = SirenixGUIStyles.PropertyMargin.margin;
            GUILayout.BeginHorizontal(SirenixGUIStyles.None);
            GUILayout.Space((float) -margin.left);
            GUILayout.BeginVertical(SirenixGUIStyles.None);
            GUILayout.Space((float) (-margin.top + 2));
            Tree.BeginDraw(true);
            foreach (InspectorProperty child in m_AbilityProperty.Children)
            {
                child.Draw();
            }

            Tree.EndDraw();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawProfile()
        {
            foreach (var child in Tree.RootProperty.Children)
            {
                if (child == m_AbilityProperty)
                {
                    continue;
                }

                child.Draw();
            }
        }
    }
}