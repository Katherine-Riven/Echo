using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Lucifer.DataDriven.Editor
{
    sealed class GraphEditWindow : EditorWindow, IDisposable
    {
        public static void Open(DataDrivenObject dataDriven)
        {
            bool            hasAlreadyOpen = HasOpenInstances<GraphEditWindow>();
            GraphEditWindow window         = GetWindow<GraphEditWindow>();
            if (hasAlreadyOpen)
            {
                window.Dispose();
            }
            else
            {
                window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 800);
            }

            window.Initialize(dataDriven);
            window.Focus();
        }

        private static GUIContent InvalidNotify = new GUIContent("当前未打开任何数据");

        [SerializeField] private DataDrivenObject m_DataDriven;
        [SerializeField] private GraphData        m_GraphData;

        private GraphView    m_GraphView;
        private PropertyTree m_Tree;

        public DataDrivenObject DataDriven => m_DataDriven;
        public GraphData        GraphData  => m_GraphData;
        public PropertyTree     Tree       => m_Tree ??= PropertyTree.Create(m_DataDriven);

        #region API

        public void Initialize(DataDrivenObject dataDriven)
        {
            if (m_DataDriven == dataDriven) return;
            m_DataDriven = dataDriven;
            UpdateTitle();
            InitializeGraphData();
            InitializeGraphEditView();
            Repaint();

            void InitializeGraphData()
            {
                string   assetPath = AssetDatabase.GetAssetPath(m_DataDriven);
                Object[] assets    = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                m_GraphData = assets.OfType<GraphData>().FirstOrDefault();
                if (m_GraphData == null)
                {
                    m_GraphData           = CreateInstance<GraphData>();
                    m_GraphData.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInBuild;
                    m_GraphData.Nodes.Add(new NodeData(Tree.RootProperty, Vector2.zero));
                    AssetDatabase.AddObjectToAsset(m_GraphData, m_DataDriven);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    InitializeGraphData();
                }
            }

            void InitializeGraphEditView()
            {
                if (m_GraphView != null)
                {
                    m_GraphView.RemoveFromHierarchy();
                }

                m_GraphView = new GraphView(this);
                m_GraphView.SetupZoom(0.05f, 10);
                m_GraphView.AddManipulator(new ContentDragger());
                m_GraphView.AddManipulator(new SelectionDragger());
                m_GraphView.AddManipulator(new RectangleSelector());
                m_GraphView.AddManipulator(new ClickSelector());
                m_GraphView.StretchToParentSize();
                rootVisualElement.Add(m_GraphView);
            }
        }

        public void Dispose()
        {
            ClearUndo();
            m_Tree?.Dispose();
            m_DataDriven = null;
            m_GraphData  = null;
            m_Tree       = null;
            UpdateTitle();
        }

        public void RegisterCompleteObjectUndo(string actionName)
        {
            Undo.RegisterCompleteObjectUndo(new Object[]
            {
                m_DataDriven,
                m_GraphData,
            }, actionName);
        }

        public void PingAsset()
        {
            EditorGUIUtility.PingObject(m_DataDriven);
        }

        #endregion

        #region Unity Event

        private void OnGUI()
        {
            if (ValidateAssets()) return;

            ShowNotification(InvalidNotify);
        }

        #endregion

        #region Utility

        private void UpdateTitle()
        {
            GUIContent content = titleContent;
            if (m_DataDriven != null) content.text = $"{m_DataDriven.name} Graph";
            else content.text                      = $"No Data";
            titleContent = content;
        }

        private bool ValidateAssets()
        {
            if (m_DataDriven == null || m_GraphData == null)
            {
                Dispose();
                return false;
            }

            return true;
        }

        private void ClearUndo()
        {
            if (m_DataDriven != null) Undo.ClearUndo(m_DataDriven);
            if (m_GraphData  != null) Undo.ClearUndo(m_GraphData);
        }

        #endregion
    }
}