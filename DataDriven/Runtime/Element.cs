using System;
using UnityEngine;

namespace Lucifer.DataDriven
{
    public interface IElement
    {
    }

    [Serializable]
    public abstract class Element : IElement
    {
        protected internal DataDrivenObject Object { get; internal set; }

        protected internal abstract void OnInitialize();
        protected internal abstract void OnDispose();
    }

    [Serializable]
    public struct Element<T> where T : class, IElement
    {
        [SerializeReference] private T m_Reference;

        public T Get() => m_Reference;
    }
}