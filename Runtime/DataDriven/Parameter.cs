using System;
using UnityEngine;

namespace Lucifer.DataDriven
{
    public interface IParameter
    {
    }

    public interface IParameter<out T> : IParameter
    {
        T Value { get; }
    }

    [Serializable]
    public struct Parameter<T>
    {
        [SerializeReference] private IParameter m_ReferenceValue;
        [SerializeField]     private T          m_DefaultValue;

        public T Value => m_ReferenceValue == null ? m_DefaultValue : ((IParameter<T>) m_ReferenceValue).Value;
    }
}