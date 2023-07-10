using System;
using JetBrains.Annotations;

namespace Echo.Abilities
{
    public static class AbilityExtension
    {
        public static TAbility AcquireAbility<TAbility>([NotNull] this IAbilityOwner owner, [NotNull] IAbilityDescriptor<TAbility> descriptor, IAbilityVariableTable variableTable, object source = null) where TAbility : IAbility
        {
            if (owner == null || descriptor == null) throw new ArgumentNullException();
            TAbility ability = descriptor.NewAbility();
            ability.OnInitialize(owner, variableTable, source ?? owner);
            ability.OnAcquire();
            owner.Abilities.Add(ability);
            return ability;
        }

        public static bool DepriveAbility([NotNull] this IAbilityOwner owner, [NotNull] IAbility ability)
        {
            if (owner == null || ability == null) throw new ArgumentNullException();
            if (ability.Owner != owner || owner.Abilities.Remove(ability) == false)
            {
                return false;
            }

            ability.OnDeprive();
            return true;
        }
    }
}