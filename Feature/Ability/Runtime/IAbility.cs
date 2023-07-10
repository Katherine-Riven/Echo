namespace Echo.Abilities
{
    public interface IAbility
    {
        IAbilityOwner         Owner         { get; }
        IAbilityVariableTable VariableTable { get; }
        object                Source        { get; }
        AbilityTag            Tag           { get; }

        void AddTag(AbilityTag    tag);
        void RemoveTag(AbilityTag tag);

        protected internal void OnInitialize(IAbilityOwner owner, IAbilityVariableTable variableTable, object source);
        protected internal void OnAcquire();
        protected internal void OnDeprive();
        protected internal void OnUpdate();
    }
}