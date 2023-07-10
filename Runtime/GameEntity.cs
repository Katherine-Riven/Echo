using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Echo
{
    public interface IGameEntity
    {
        string Name     { get; }
        bool   IsActive { get; }
        void   Instantiate(Vector3 position, Quaternion rotation);
        void   Destroy();
    }

    public interface IGameEntityOrder
    {
        IGameEntityReference Reference { get; }
    }

    public interface IGameEntityReference
    {
        GameObject InstantiateInstance(Vector3 position, Quaternion rotation);
        void       ReleaseInstance(GameObject  instance);
    }

    public abstract class GameEntity : IGameEntity
    {
        protected GameEntity(string name, IGameEntityOrder order)
        {
            Name        = name;
            m_Reference = order.Reference;
        }

        private readonly IGameEntityReference m_Reference;

        private GameObject m_Instance;

        public string Name { get; }

        public bool IsActive => GameManager.IsEntityActive(this);

        public void Instantiate(Vector3 position, Quaternion rotation)
        {
            if (GameManager.RegisterEntity(this) == false)
            {
                throw new Exception();
            }

            m_Instance      = m_Reference.InstantiateInstance(position, rotation);
            m_Instance.name = Name;
            OnInstantiate(m_Instance);
        }

        public void Destroy()
        {
            if (GameManager.UnRegisterEntity(this) == false)
            {
                throw new Exception();
            }

            OnDestroy();
            m_Reference.ReleaseInstance(m_Instance);
            m_Instance = null;
        }

        protected abstract void OnInstantiate(GameObject instance);
        protected abstract void OnDestroy();
    }

    public readonly struct GameEntityQuery<T> : IEnumerable<T>, IDisposable where T : class, IGameEntity
    {
        internal GameEntityQuery(List<GameEntity> entities)
        {
            m_List          = ListPool<GameEntity>.Get();
            m_List.Capacity = Mathf.Max(m_List.Capacity, entities.Capacity);
            foreach (var entity in entities)
            {
                m_List.Add(entity);
            }
        }

        private readonly List<GameEntity> m_List;

        public Enumerator             GetEnumerator() => new Enumerator(m_List);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
        void IDisposable.             Dispose()       => ListPool<GameEntity>.Release(m_List);

        public struct Enumerator : IEnumerator<T>
        {
            internal Enumerator(List<GameEntity> entities)
            {
                m_List    = entities;
                m_Current = null;
                m_Index   = 0;
            }

            private readonly List<GameEntity> m_List;

            private T   m_Current;
            private int m_Index;

            public bool MoveNext()
            {
                for (; m_Index < m_List.Count; m_Index++)
                {
                    if (m_List[m_Index] is T temp && temp.IsActive)
                    {
                        m_Current = temp;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                m_Current = null;
                m_Index   = 0;
            }

            public T Current => m_Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}