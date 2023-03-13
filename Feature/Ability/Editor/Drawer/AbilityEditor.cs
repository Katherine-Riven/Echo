using System;
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
        private const string JsonFieldName = "m_Json";

        [SerializeReference]
        private Ability m_Ability;

        private GUIContent   m_TypeContent;
        private PropertyTree m_EditorTree;

        private SerializedProperty JsonProperty => serializedObject.FindProperty(JsonFieldName);

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Ability     = AbilityEditorUtility.GetAbility((AbilityProfile) target);
            m_TypeContent = new GUIContent(ObjectNames.NicifyVariableName(m_Ability.GetType().Name));
            m_EditorTree  = PropertyTree.Create(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_EditorTree.Dispose();
        }

        protected override void DrawTree()
        {
            RectOffset margin = SirenixGUIStyles.PropertyMargin.margin;
            GUILayout.BeginHorizontal(SirenixGUIStyles.None);
            GUILayout.Space((float) -margin.left);
            GUILayout.BeginVertical(SirenixGUIStyles.None);
            GUILayout.Space((float) (-margin.top + 2));
            DrawProfile();
            DrawAbility();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            serializedObject.Update();
            JsonProperty.stringValue = JsonUtility.ToJson(m_Ability);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void DrawProfile()
        {
            Tree.BeginDraw(true);
            if (Tree.RootProperty.Children.Count > 1)
            {
                SirenixEditorGUI.BeginBox("Profile");
                foreach (InspectorProperty child in Tree.RootProperty.Children)
                {
                    if (child.Name == JsonFieldName)
                    {
                        continue;
                    }

                    child.Draw();
                }

                SirenixEditorGUI.EndBox();
            }

            Tree.EndDraw();
        }

        private void DrawAbility()
        {
            SirenixEditorGUI.BeginBox(m_TypeContent);
            m_EditorTree.BeginDraw(true);
            foreach (InspectorProperty child in m_EditorTree.RootProperty.FindChild(x => x.Name == nameof(m_Ability), false).Children)
            {
                child.Draw();
            }

            m_EditorTree.EndDraw();
            SirenixEditorGUI.EndBox();
        }
    }
}