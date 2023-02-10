using Sirenix.OdinInspector.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Lucifer.DataDriven.Editor
{
    class NodeView : Node
    {
        private InspectorProperty m_Property;
        private NodeData          m_Data;

        public string           PropertyPath => m_Property.Path;
        public HeadPortView     Head         { get; }
        public OutputPortView[] Outputs      { get; }

        public NodeView(InspectorProperty property, NodeData data)
        {
            m_Property  = property;
            m_Data      = data;
            base.SetPosition(m_Data.Position);

            title = property.GetTypeName();
            /*if (property.IsTreeRoot == false)
            {
                Head = new HeadPortView(property);
                titleContainer.Insert(0, Head);
            }*/

            Outputs = new OutputPortView[property.Children.Count];
            for (int i = 0; i < property.Children.Count; i++)
            {
                if (property.Children[i].Info.PropertyType != PropertyType.Value) continue;
                //Outputs[i] = m_GraphView.CreateOutputPort(property.Children[i]);
                outputContainer.Add(Outputs[i]);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            m_Data.Position = newPos;
        }
    }
}