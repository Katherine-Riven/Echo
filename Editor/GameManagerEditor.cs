using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : OdinEditor
    {
        private InspectorProperty[] m_Systems;
        private InspectorProperty   m_Features;

        protected override void OnEnable()
        {
            base.OnEnable();
            List<InspectorProperty> systems = new List<InspectorProperty>();
            foreach (var property in Tree.EnumerateTree())
            {
                if (typeof(IGameSystem).IsAssignableFrom(property.Info.TypeOfValue))
                {
                    systems.Add(property);
                }
            }
            
            m_Systems = systems.ToArray();
        }

        protected override void DrawTree()
        {
            RectOffset margin = SirenixGUIStyles.PropertyMargin.margin;
            GUILayout.BeginHorizontal(SirenixGUIStyles.None);
            GUILayout.Space((float) -margin.left);
            GUILayout.BeginVertical(SirenixGUIStyles.None);
            GUILayout.Space((float) (-margin.top + 2));
            foreach (InspectorProperty systemProperty in m_Systems)
            {
                
            }
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}