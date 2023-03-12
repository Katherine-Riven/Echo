using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    [Serializable]
    public abstract class AbilityBehaviour : ISerializationCallbackReceiver
    {
        [NonSerialized]
        internal static List<AbilityBehaviour> s_Collector;

        [SerializeField]
        private string m_Name;

        [NonSerialized]
        private IAbility m_Ability;

        /// <summary>
        /// 自定义名称
        /// </summary>
        public string Name => m_Name;

        /// <summary>
        /// 定义该Behaviour的能力
        /// </summary>
        public IAbility Ability
        {
            get => m_Ability;
            internal set => m_Ability = value;
        }

        /// <summary>
        /// 当定义该Behaviour的Ability启用时
        /// </summary>
        protected internal virtual void OnAbilityEnable()
        {
        }

        /// <summary>
        /// 当定义该Behaviour的Ability更新时
        /// </summary>
        protected internal virtual void OnAbilityUpdate()
        {
        }

        /// <summary>
        /// 当定义该Behaviour的Ability禁用时
        /// </summary>
        protected internal virtual void OnAbilityDisable()
        {
        }

        protected virtual void OnBeforeSerialize()
        {
        }

        protected virtual void OnAfterDeserialize()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            OnBeforeSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            s_Collector?.Add(this);
            OnAfterDeserialize();
        }
    }
}