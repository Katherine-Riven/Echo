using System;
using UnityEngine;

namespace Lucifer.DataDriven
{
    public interface IParameter<out T>
    {
        T Value { get; }
    }

    [Serializable]
    public struct Parameter<T>
    {
        [SerializeReference] private IParameter<T> m_ReferenceValue;
        [SerializeField]     private T             m_DefaultValue;

        public T Value => m_ReferenceValue == null ? m_DefaultValue : m_ReferenceValue.Value;
    }
}