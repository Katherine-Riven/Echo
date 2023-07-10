using System;
using UnityEngine;

namespace Echo.Abilities
{
    public abstract class AbilityDescriptor<T> : ScriptableObject, ISerializationCallbackReceiver, IAbilityDescriptor<T> where T : Ability
    {
        [SerializeField]
        private T m_Ability;

        [NonSerialized]
        private string m_JsonCache;

        public T NewAbility()
        {
            return JsonUtility.FromJson<T>(m_JsonCache);
        }

        IAbility IAbilityDescriptor.NewAbility() => NewAbility();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_JsonCache = JsonUtility.ToJson(m_Ability);
        }
    }
}