using System;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力参数
    /// </summary>
    public interface IAbilityParameter
    {
    }

    /// <summary>
    /// 能力参数
    /// </summary>
    public interface IAbilityParameter<T> : IAbilityParameter
    {
        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="context"></param>
        T GetValue(IAbilityContext context);
    }

    /// <summary>
    /// 能力参数
    /// </summary>
    [Serializable]
    public sealed class AbilityParameter<T> : AbilityBehaviour, ISerializationCallbackReceiver
    {
        [SerializeReference]
        private IAbilityParameter m_Reference;

        [SerializeField]
        private string m_FromContext;

        [SerializeField]
        private T m_DirectValue;

        /// <summary>
        /// 取值
        /// </summary>
        public T GetValue(IAbilityContext context)
        {
            if (m_Reference != null)
            {
                return ((IAbilityParameter<T>) m_Reference).GetValue(context);
            }

            if (string.IsNullOrEmpty(m_FromContext) == false)
            {
                return context.GetValue<T>(m_FromContext);
            }

            return m_DirectValue;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_Reference is IAbilityParameter<T> == false)
            {
                m_Reference = null;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }
    }
}