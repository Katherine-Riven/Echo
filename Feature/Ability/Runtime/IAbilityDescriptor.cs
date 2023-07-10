namespace Echo.Abilities
{
    public interface IAbilityDescriptor
    {
        IAbility NewAbility();
    }

    public interface IAbilityDescriptor<out TAbility> : IAbilityDescriptor where TAbility : IAbility
    {
        new TAbility NewAbility();
    }
}