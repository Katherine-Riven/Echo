using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Echo.UI
{
    [Serializable]
    internal sealed class UISystem : IUISystem
    {
        private const int DefaultCapacity = 8;

        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private Canvas m_Canvas;

        [SerializeField]
        private EventSystem m_EventSystem;

        private int m_InteractionDisableCounter = 0;

        private List<IView> m_ViewStack  = new List<IView>(DefaultCapacity);
        private List<IView> m_UnusedView = new List<IView>(DefaultCapacity);

        void IGameSystem.OnInitialize()
        {
        }

        void IGameSystem.OnStart()
        {
        }

        void IGameSystem.OnDispose()
        {
        }

        void IGameSystem.OnUpdate()
        {
        }

        void IGameSystem.OnFixedUpdate()
        {
        }

        void IGameSystem.OnLateUpdate()
        {
        }

        public float ViewExpireTime { get; set; }

        public Camera GetCamera() => m_Camera;

        public Canvas GetCanvas() => m_Canvas;

        public EventSystem GetEventSystem() => m_EventSystem;

        public void EnableInteraction()
        {
            m_InteractionDisableCounter--;
            if (m_InteractionDisableCounter > 0)
            {
                return;
            }

            m_InteractionDisableCounter = 0;
            m_EventSystem.enabled       = true;
        }

        public void DisableInteraction()
        {
            if (m_InteractionDisableCounter == 0)
            {
                m_EventSystem.enabled = false;
            }

            m_InteractionDisableCounter++;
        }

        public void PopTip(string text)
        {
        }

        public IView Show<TView>(IArgs args = null) where TView : class, IView, new()
        {
            IView result = PopOrNewView<TView>();
            m_ViewStack.Add(result);
            result.RectTransform.SetAsLastSibling();
            result.HandleArgs(args);
            return result;
        }

        public void Hide(IView view)
        {
            if (m_ViewStack.Remove(view) == false)
            {
                return;
            }
        }

        public void ReleaseAllUnusedView()
        {
        }

        private IView PopOrNewView<TView>() where TView : class, IView, new()
        {
            for (int i = 0; i < m_ViewStack.Count; i++)
            {
                if (m_ViewStack[i] is TView target)
                {
                    m_ViewStack.RemoveAt(i);
                    return target;
                }
            }

            for (int i = 0; i < m_UnusedView.Count; i++)
            {
                if (m_UnusedView[i] is TView target)
                {
                    m_UnusedView.RemoveAt(i);
                    return target;
                }
            }

            return new TView();
        }
    }
}