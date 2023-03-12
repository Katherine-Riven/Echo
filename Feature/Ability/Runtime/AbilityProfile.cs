using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Echo.Abilities
{
    public abstract class AbilityProfile : ScriptableObject
    {
    }

    public abstract class AbilityProfile<T> : AbilityProfile, ISerializationCallbackReceiver where T : Ability
    {
        [SerializeField]
        private string m_Json;

        [NonSerialized, ShowInInspector]
        private T m_Ability;

        /// <summary>
        /// 创建实例
        /// </summary>
        public T CreateInstance()
        {
            if (AbilityBehaviour.s_Collector != null)
            {
                throw new Exception("Can't create another ability instance when creating.");
            }

            AbilityBehaviour.s_Collector = ListPool<AbilityBehaviour>.Get();
            T ability = JsonUtility.FromJson<T>(m_Json);
            ability.Behaviours = AbilityBehaviour.s_Collector.ToArray();
            foreach (AbilityBehaviour behaviour in ability.Behaviours)
            {
                behaviour.Ability = ability;
            }

            ListPool<AbilityBehaviour>.Release(AbilityBehaviour.s_Collector);
            AbilityBehaviour.s_Collector = null;
            return ability;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            m_Ability ??= JsonUtility.FromJson<T>(string.IsNullOrEmpty(m_Json) ? "{}" : m_Json);
#endif
            m_Json = JsonUtility.ToJson(m_Ability);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITY_EDITOR
            m_Ability ??= JsonUtility.FromJson<T>(string.IsNullOrEmpty(m_Json) ? "{}" : m_Json);
#endif
        }
    }
}