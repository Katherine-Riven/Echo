namespace Echo.Abilities
{
    public static class AbilityAPI
    {
        /// <summary>
        /// 启用能力
        /// </summary>
        /// <param name="driver">持有者</param>
        /// <param name="profile">能力描述符</param>
        public static Ability EnableAbility(this IAbilityDriver driver, AbilityProfile profile)
        {
            Ability ability = AbilitySerializer.FromProfile(profile);
            ability.OnInitialize(driver);
            driver.ToEnableAbilities.Add(ability);
            return ability;
        }

        /// <summary>
        /// 禁用能力
        /// </summary>
        /// <param name="driver">持有者</param>
        /// <param name="ability">能力实例</param>
        public static void DisableAbility(this IAbilityDriver driver, Ability ability)
        {
            if (driver.Abilities.Contains(ability) && driver.ToDisableAbilities.Contains(ability) == false)
            {
                driver.ToDisableAbilities.Add(ability);
            }
        }

        /// <summary>
        /// 添加修改器
        /// </summary>
        /// <param name="driver">持有者</param>
        /// <param name="modifier">修改器实例</param>
        public static void AddModifier(this IAbilityDriver driver, IAbilityModifier modifier)
        {
            driver.Modifiers.Add(modifier);
        }

        /// <summary>
        /// 移除修改器
        /// </summary>
        /// <param name="driver">持有者</param>
        /// <param name="modifier">修改器实例</param>
        public static void RemoveModifier(this IAbilityDriver driver, IAbilityModifier modifier)
        {
            driver.Modifiers.Remove(modifier);
        }
    }
}