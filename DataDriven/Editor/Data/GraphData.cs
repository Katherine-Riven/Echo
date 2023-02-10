using System.Collections.Generic;
using UnityEngine;

namespace Lucifer.DataDriven.Editor
{
    sealed class GraphData : ScriptableObject
    {
        public List<NodeData> Nodes = new List<NodeData>();
    }
}