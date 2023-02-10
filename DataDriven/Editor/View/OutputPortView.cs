using Sirenix.OdinInspector.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Lucifer.DataDriven.Editor
{
    sealed class OutputPortView : Port
    {
        public OutputPortView(InspectorProperty property, IEdgeConnectorListener listener) :
            base(Orientation.Horizontal, Direction.Output, Capacity.Multi, property.Info.TypeOfValue)
        {
            m_EdgeConnector = new EdgeConnector<EdgeView>(listener);
            this.AddManipulator(m_EdgeConnector);
        }

        public string PropertyPath       { get; }
        public string TargetPropertyPath { get; }
    }
}