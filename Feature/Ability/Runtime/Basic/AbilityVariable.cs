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
        /// 取名称
        /// </summary>
        string GetName();
    }

    /// <summary>
    /// 能力变量表
    /// </summary>
    [Serializable]
    public sealed class AbilityVariableTable
    {
        [SerializeReference] private IAbilityVariable[] m_Variables;

        /// <summary>
        /// 初始化变量值
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="value">变量值</param>
        /// <typeparam name="T">变量类型</typeparam>
        public void InitializeVariable<T>(string name, in T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                if (m_Variables[i].GetName() == name)
                {
                    if (m_Variables[i] is AbilityVariable<T> variable)
                    {
                        variable.Initialize(value);
                    }
                    else
                    {
                        Debug.LogError($"Variable({name}) is not match type({typeof(T)}).");
                    }

                    return;
                }
            }

            Debug.LogError($"Can't find Variable({name}).");
        }
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
        public string GetName() => m_Name;

        /// <summary>
        /// 取值
        /// </summary>
        public T GetValue(IAbilityContext context) => m_Value;

        /// <summary>
        /// 初始化值
        /// </summary>
        public void Initialize(in T value) => m_Value = value;
    }
}