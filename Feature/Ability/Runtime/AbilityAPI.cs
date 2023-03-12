using System;

namespace Echo.Abilities
{
    public static class AbilityAPI
    {
        /// <summary>
        /// 启用能力
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="ability">能力实例</param>
        public static void EnableAbility(this IAbilityOwner owner, IAbility ability)
        {
            if (ability.IsEnable)
            {
                throw new ArgumentException("Ability has already enabled.", nameof(ability));
            }

            ability.IsEnable = true;
            ability.Owner    = owner;
            owner.ToEnableAbilities.Add(ability);
        }

        /// <summary>
        /// 禁用能力
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="ability">能力实例</param>
        public static void DisableAbility(this IAbilityOwner owner, IAbility ability)
        {
            if (owner.Abilities.Contains(ability) && owner.ToDisableAbilities.Contains(ability) == false)
            {
                ability.IsEnable = false;
                owner.ToDisableAbilities.Add(ability);
            }
        }

        /// <summary>
        /// 添加修改器
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="modifier">修改器实例</param>
        public static void AddModifier(this IAbilityOwner owner, IAbilityModifier modifier)
        {
            owner.Modifiers.Add(modifier);
        }

        /// <summary>
        /// 移除修改器
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="modifier">修改器实例</param>
        public static void RemoveModifier(this IAbilityOwner owner, IAbilityModifier modifier)
        {
            owner.Modifiers.Remove(modifier);
        }
    }
}