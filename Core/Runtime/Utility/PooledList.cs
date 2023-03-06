using System;
using System.Collections;
using System.Collections.Generic;

namespace Echo.Utility
{
    public struct PooledList<T> :
        IList<T>,
        ICollection<T>,
        IEnumerable<T>,
        IReadOnlyList<T>,
        IReadOnlyCollection<T>,
        IDisposable

    {
        private static readonly Stack<List<T>> s_Pooled = new Stack<List<T>>();

        public static PooledList<T> Get()
        {
            List<T> list = s_Pooled.TryPop(out List<T> pooled) ? pooled : new List<T>();
            return new PooledList<T>(list);
        }

        private List<T> m_List;

        private PooledList(List<T> list)
        {
            m_List = list;
        }

        public void Dispose()
        {
            if (m_List == null) return;
            m_List.Clear();
            s_Pooled.Push(m_List);
            m_List = null;
        }

        public T this[int index]
        {
            get => m_List[index];
            set => m_List[index] = value;
        }

        public int Count => m_List.Count;

        public void Add(T                   item)                             => m_List.Add(item);
        public void AddRange(IEnumerable<T> collection)                       => m_List.AddRange(collection);
        public bool Remove(T                item)                             => m_List.Remove(item);
        public void RemoveAll(Predicate<T>  match)                            => m_List.RemoveAll(match);
        public void RemoveAt(int            index)                            => m_List.RemoveAt(index);
        public void RemoveRange(int         index, int            count)      => m_List.RemoveRange(index, count);
        public void Insert(int              index, T              item)       => m_List.Insert(index, item);
        public void InsertRange(int         index, IEnumerable<T> collection) => m_List.InsertRange(index, collection);
        public void Clear() => m_List.Clear();

        public bool Contains(T          item)                       => m_List.Contains(item);
        public bool Exists(Predicate<T> match)                      => m_List.Exists(match);
        public int  IndexOf(T           item)                       => m_List.IndexOf(item);
        public int  IndexOf(T           item, int index)            => m_List.IndexOf(item, index);
        public int  IndexOf(T           item, int index, int count) => m_List.IndexOf(item, index, count);

        public T       Find(Predicate<T>          match)                                              => m_List.Find(match);
        public List<T> FindAll(Predicate<T>       match)                                              => m_List.FindAll(match);
        public int     FindIndex(Predicate<T>     match)                                              => m_List.FindIndex(match);
        public int     FindIndex(int              startIndex, Predicate<T> match)                     => m_List.FindIndex(startIndex, match);
        public int     FindIndex(int              startIndex, int          count, Predicate<T> match) => m_List.FindIndex(startIndex, count, match);
        public T       FindLast(Predicate<T>      match)                                              => m_List.FindLast(match);
        public int     FindLastIndex(Predicate<T> match)                                              => m_List.FindLastIndex(match);
        public int     FindLastIndex(int          startIndex, Predicate<T> match)                     => m_List.FindLastIndex(startIndex, match);
        public int     FindLastIndex(int          startIndex, int          count, Predicate<T> match) => m_List.FindLastIndex(startIndex, count, match);

        public void Sort()                                                      => m_List.Sort();
        public void Sort(Comparison<T> comparison)                              => m_List.Sort(comparison);
        public void Sort(IComparer<T>  comparer)                                => m_List.Sort(comparer);
        public void Sort(int           index, int count, IComparer<T> comparer) => m_List.Sort(index, count, comparer);

        public void Reverse()                     => m_List.Reverse();
        public void Reverse(int index, int count) => m_List.Reverse(index, count);

        public bool TrueForAll(Predicate<T> match) => m_List.TrueForAll(match);

        public List<T>.Enumerator     GetEnumerator()                   => m_List.GetEnumerator();
        bool ICollection<T>.          IsReadOnly                        => false;
        void ICollection<T>.          CopyTo(T[] array, int arrayIndex) => m_List.CopyTo(array, arrayIndex);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    }
}