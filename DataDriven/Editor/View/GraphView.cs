using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lucifer.DataDriven.Editor
{
    sealed class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        public GraphView(GraphEditWindow window)
        {
            m_Window         = window;
            graphViewChanged = GraphViewChangedImplementation;
            Insert(0, new Grid());
            InitializeView();
        }

        private GraphEditWindow m_Window;
        private VariableView    m_VariableView;
        private List<NodeView>  m_NodeViews;
        private List<EdgeView>  m_EdgeViews;

        private GraphViewChange GraphViewChangedImplementation(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove is {Count: > 0})
            {
                m_Window.RegisterCompleteObjectUndo("Remove Graph Elements");
            }

            return graphViewChange;
        }

        private void InitializeView()
        {
            m_VariableView = new VariableView(m_Window.Tree.GetPropertyAtPath("m_Variable"));
            AddElement(m_VariableView);

            m_NodeViews = new List<NodeView>(128);
            foreach (NodeData nodeData in m_Window.GraphData.Nodes)
            {
                InspectorProperty property = m_Window.Tree.GetPropertyAtPath(nodeData.PropertyPath);
                /*NodeView          nodeView = new NodeView(this, property, nodeData);
                AddNode(nodeView);*/
            }

            m_EdgeViews = new List<EdgeView>(128);
            foreach (NodeView nodeView in m_NodeViews)
            {
                foreach (OutputPortView port in nodeView.outputContainer.Query<OutputPortView>().ToList())
                {
                    if (string.IsNullOrEmpty(port.TargetPropertyPath))
                    {
                        continue;
                    }

                    NodeView connectTarget = m_NodeViews.FirstOrDefault(x => x.PropertyPath == port.TargetPropertyPath);
                    if (connectTarget == null)
                    {
                        continue;
                    }

                    Connect(port, connectTarget.Head);
                }
            }
        }

        private void ClearView()
        {
            foreach (NodeView nodeView in m_NodeViews)
            {
                RemoveElement(nodeView);
            }

            foreach (EdgeView edgeView in m_EdgeViews)
            {
                RemoveElement(edgeView);
            }
        }

        private void AddNode(NodeView nodeView)
        {
            m_NodeViews.Add(nodeView);
            AddElement(nodeView);
        }

        private void Connect(Port a, Port b)
        {
            EdgeView edgeView = a.ConnectTo<EdgeView>(b);
            m_EdgeViews.Add(edgeView);
            AddElement(edgeView);
        }

        public OutputPortView CreateOutputPort(InspectorProperty property)
        {
            OutputPortView port = new OutputPortView(property, new EdgeConnectorListener())
            {
                portName = property.GetPropertyName(),
            };

            return port;
        }

        class Grid : GridBackground
        {
        }

        class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
            {
            }
        }
    }
}