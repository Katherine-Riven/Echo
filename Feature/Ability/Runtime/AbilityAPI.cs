namespace Echo.Abilities
{
    public static class AbilityAPI
    {
        /// <summary>
        /// 启用能力
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="profile">能力描述符</param>
        public static Ability EnableAbility(this IAbilityOwner owner, AbilityProfile profile)
        {
            Ability ability = AbilitySerializer.FromProfile(profile);
            ability.OnInitialize(owner);
            owner.ToEnableAbilities.Add(ability);
            return ability;
        }

        /// <summary>
        /// 禁用能力
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="ability">能力实例</param>
        public static void DisableAbility(this IAbilityOwner owner, Ability ability)
        {
            if (owner.Abilities.Contains(ability) && owner.ToDisableAbilities.Contains(ability) == false)
            {
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