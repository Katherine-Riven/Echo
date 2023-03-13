using UnityEngine;
using UnityEngine.Pool;

namespace Echo.Abilities
{
    public abstract class AbilityProfile : ScriptableObject
    {
        [SerializeField]
        protected string m_Json;

        public string Json => m_Json;
    }

    public abstract class AbilityProfile<T> : AbilityProfile where T : Ability
    {
        /// <summary>
        /// 新建Ability实例
        /// </summary>
        public T NewAbility()
        {
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
    }
}