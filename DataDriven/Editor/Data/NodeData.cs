using System;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Lucifer.DataDriven.Editor
{
    [Serializable]
    sealed class NodeData
    {
        public string PropertyPath;
        public Rect   Position;

        private NodeData()
        {
            PropertyPath = string.Empty;
        }

        public NodeData(InspectorProperty property, Vector2 position)
        {
            PropertyPath = property.Path;
            Position     = new Rect(position, Vector2.zero);
        }
    }
}