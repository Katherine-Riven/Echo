using System.Collections;
using System.Collections.Generic;
using ListEnumerator = System.Collections.Generic.List<Echo.Abilities.IAbilityModifier>.Enumerator;

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
        internal AbilityModifierQuery(Ability ability, List<IAbilityModifier> modifiers)
        {
            m_Ability = ability;
            m_List    = modifiers;
        }

        private Ability                m_Ability;
        private List<IAbilityModifier> m_List;

        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_Ability, m_List.GetEnumerator());
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
            internal Enumerator(Ability ability, ListEnumerator modifiers)
            {
                m_Ability    = ability;
                m_Enumerator = modifiers;
                m_Current    = default;
            }

            private Ability        m_Ability;
            private ListEnumerator m_Enumerator;
            private T              m_Current;

            public bool MoveNext()
            {
                while (m_Enumerator.MoveNext())
                {
                    IAbilityModifier modifier = m_Enumerator.Current;
                    if (modifier is T temp && modifier.IsMatch(m_Ability))
                    {
                        m_Current = temp;
                        return true;
                    }
                }

                return false;
            }


            public T Current => m_Current;

            void IEnumerator.Reset()
            {
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                m_Enumerator.Dispose();
            }
        }
    }
}