using System;
using UnityEngine;

namespace Echo.DataDriven
{
    public interface IVariable
    {
        string Name { get; }
        void   Reset();
    }

    [Serializable]
    public abstract class Variable<T> : IVariable, IParameter<T>
    {
        [SerializeField] private string m_Name;
        [SerializeField] private T      m_Value;

        public string Name    => m_Name;
        public string Tooltip => $"{m_Name}:{m_Value}";

        public T Value { get; internal set; }

        public void Reset()
        {
            Value = m_Value;
        }
    }
}