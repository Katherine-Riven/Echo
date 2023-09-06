using System.Collections.Generic;

namespace Echo.Abilities
{
    public interface IAbilityOwner : IGameAvatar
    {
        protected internal List<IAbility> Abilities { get; }
    }
}