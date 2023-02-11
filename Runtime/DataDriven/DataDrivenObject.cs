using System;
using UnityEngine;

// ReSharper disable Unity.RedundantHideInInspectorAttribute

namespace Lucifer.DataDriven
{
    [Serializable]
    public abstract class DataDrivenObject : ScriptableObject
    {
        [HideInInspector, SerializeReference] private DataDrivenElement[] m_Elements  = new DataDrivenElement[0];
        [SerializeReference]                  private IVariable[]         m_Variables = new IVariable[0];

        public void Initialize()
        {
            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                m_Variables[i].Reset();
            }

            OnInitialize();

            for (int i = 0, length = m_Elements.Length; i < length; i++)
            {
                m_Elements[i].Object = this;
                m_Elements[i].OnInitialize();
            }
        }

        public void Dispose()
        {
            for (int i = 0, length = m_Elements.Length; i < length; i++)
            {
                m_Elements[i].OnDispose();
                m_Elements[i].Object = null;
            }

            OnDispose();
        }

        public void SetVariable<T>(string variableName, T value)
        {
            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                if (m_Variables[i].Name == variableName && m_Variables[i] is Variable<T> variable)
                {
                    variable.Value = value;
                    break;
                }
            }
        }

        public T GetVariable<T>(string variableName, T fallback = default)
        {
            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                if (m_Variables[i].Name == variableName && m_Variables[i] is Variable<T> variable)
                {
                    return variable.Value;
                }
            }

            return fallback;
        }

        protected abstract void OnInitialize();
        protected abstract void OnDispose();
    }
}