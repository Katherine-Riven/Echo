using System;
using UnityEngine;

namespace Echo.Abilities
{
    [Serializable]
    public abstract class Ability : IAbility
    {
        [SerializeField]
        private AbilityTag m_Tag;

        public IAbilityOwner         Owner         { get; private set; }
        public IAbilityVariableTable VariableTable { get; private set; }
        public object                Source        { get; private set; }
        public AbilityTag            Tag           => m_Tag;

        public void AddTag(AbilityTag tag)
        {
            m_Tag |= tag;
        }

        public void RemoveTag(AbilityTag tag)
        {
            m_Tag &= ~tag;
        }

        void IAbility.OnInitialize(IAbilityOwner owner, IAbilityVariableTable variableTable, object source)
        {
            Owner         = owner;
            VariableTable = variableTable;
            Source        = source;
        }

        void IAbility.OnAcquire()
        {
        }

        void IAbility.OnDeprive()
        {
        }

        void IAbility.OnUpdate()
        {
        }
    }
}