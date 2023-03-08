using System;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力变量
    /// </summary>
    public interface IAbilityVariable
    {
        /// <summary>
        /// 变量名
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// 能力变量
    /// </summary>
    [Serializable]
    public abstract class AbilityVariable<T> : IAbilityVariable, IAbilityParameter<T>
    {
        [SerializeField] private string m_Name;
        [SerializeField] private T      m_Value;

        /// <summary>
        /// 取名称
        /// </summary>
        public string Name => m_Name;

        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        T IAbilityParameter<T>.GetValue(IAbilityContext context) => m_Value;
    }
}