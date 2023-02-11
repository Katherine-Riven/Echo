using System;
using UnityEngine;

namespace Lucifer.DataDriven
{
    public interface IBehaviour
    {
    }

    [Serializable]
    public struct Behaviour<T> where T : class, IBehaviour
    {
        [SerializeReference] private T m_Reference;

        public T Get() => m_Reference;
    }
}