using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    [Serializable]
    public abstract class AbilityBehaviour : ISerializationCallbackReceiver
    {
        internal static List<AbilityBehaviour> s_Collector;

        public Ability Ability { get; internal set; }

        protected internal virtual void OnAbilityEnable()
        {
        }

        protected internal virtual void OnAbilityDisable()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            s_Collector?.Add(this);
        }
    }
}