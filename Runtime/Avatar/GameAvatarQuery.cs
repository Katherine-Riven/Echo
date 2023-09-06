using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Echo
{
    public readonly struct GameAvatarQuery<T> : IEnumerable<T>, IDisposable where T : IGameAvatar
    {
        internal GameAvatarQuery(List<IGameAvatar> entities)
        {
            m_List          = ListPool<IGameAvatar>.Get();
            m_List.Capacity = Mathf.Max(m_List.Capacity, entities.Capacity);
            foreach (IGameAvatar avatar in entities)
            {
                if (avatar is T) m_List.Add(avatar);
            }
        }

        private readonly List<IGameAvatar> m_List;

        public Enumerator             GetEnumerator() => new Enumerator(m_List);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
        void IDisposable.             Dispose()       => ListPool<IGameAvatar>.Release(m_List);

        public struct Enumerator : IEnumerator<T>
        {
            internal Enumerator(List<IGameAvatar> entities)
            {
                m_List    = entities;
                m_Current = default;
                m_Index   = 0;
            }

            private readonly List<IGameAvatar> m_List;

            private T   m_Current;
            private int m_Index;

            public bool MoveNext()
            {
                for (; m_Index < m_List.Count; m_Index++)
                {
                    if (m_List[m_Index] is T temp && temp.HasBeenDestroyed == false)
                    {
                        m_Current = temp;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                m_Current = default;
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