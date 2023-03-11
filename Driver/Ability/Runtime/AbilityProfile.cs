using System;
using UnityEngine;

namespace Echo.Abilities
{
    public abstract class AbilityProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private Ability m_Ability;

        [NonSerialized]
        private string m_Json;

        internal string AbilityData => m_Json;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_Json = JsonUtility.ToJson(m_Ability);
        }
    }
}