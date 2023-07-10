using System.Collections.Generic;

namespace Echo.Abilities
{
    public interface IAbilityOwner : IGameEntity
    {
        protected internal List<IAbility> Abilities { get; }
    }
}