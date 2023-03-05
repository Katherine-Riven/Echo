using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EchoEditor.Abilities
{
    [CustomEditor(typeof(AbilityAsset))]
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
            RectOffset margin = SirenixGUIStyles.PropertyMargin.margin;
            GUILayout.BeginHorizontal(SirenixGUIStyles.None);
            GUILayout.Space((float) -margin.left);
            GUILayout.BeginVertical(SirenixGUIStyles.None);
            GUILayout.Space((float) (-margin.top + 2));
            Tree.BeginDraw(true);
            m_AbilityProperty.Draw(null);
            Tree.EndDraw();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}