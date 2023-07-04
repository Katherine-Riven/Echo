using System.Collections.Generic;
using Echo;

namespace Feature.Ability
{
    public interface IAbilityHandler : IGameEntity
    {
        protected internal List<IAbility> Abilities { get; }

        IAbility Acquire(IAbilityDescriptor descriptor)
        {
            IAbility ability = descriptor.NewAbility();
            ability.Handler = this;
            ability.OnAcquire();
            Abilities.Add(ability);
            return ability;
        }

        void Deprive(IAbility ability);
    }

    public interface IAbilityDescriptor
    {
        IAbility NewAbility();
    }

    public interface IAbility
    {
        protected internal IAbilityHandler Handler { get; set; }

        protected internal void OnAcquire();
    }
}