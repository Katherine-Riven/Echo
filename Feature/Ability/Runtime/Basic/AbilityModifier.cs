using System.Collections;
using System.Collections.Generic;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力修改器
    /// </summary>
    public interface IAbilityModifier
    {
        /// <summary>
        /// 该修改器是否适用于目标能力
        /// </summary>
        /// <param name="abilityInstance">目标能力</param>
        /// <returns>返回是否适用</returns>
        bool IsMatch(Ability abilityInstance);
    }

    /// <summary>
    /// 修改器查询结果
    /// </summary>
    public struct AbilityModifierQuery<T> : IEnumerable<T> where T : IAbilityModifier
    {
        private List<IAbilityModifier> m_List;

        internal AbilityModifierQuery(List<IAbilityModifier> modifiers)
        {
            m_List = modifiers;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_List);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 查询结果迭代器
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            internal Enumerator(List<IAbilityModifier> modifiers)
            {
                m_List  = modifiers;
                m_Index = -1;
            }

            private List<IAbilityModifier> m_List;
            private int                    m_Index;

            public bool MoveNext()
            {
                while (++m_Index < m_List.Count)
                {
                    if (m_List[m_Index] is T)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                m_Index = -1;
            }

            public T Current => (T) m_List[m_Index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                Reset();
            }
        }
    }
}