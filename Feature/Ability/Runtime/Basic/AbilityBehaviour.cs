using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    [Serializable]
    public abstract class AbilityBehaviour : ISerializationCallbackReceiver
    {
        internal static List<AbilityBehaviour> s_Collector;

        [SerializeField]
        private string m_Name;

        public string Name => m_Name;

        public Ability Ability { get; internal set; }

        protected internal virtual void OnAbilityEnable()
        {
        }

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