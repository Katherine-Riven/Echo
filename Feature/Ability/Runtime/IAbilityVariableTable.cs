namespace Echo.Abilities
{
    public interface IAbilityVariableTable
    {
        T GetVariable<T>(string variableName);
    }
}