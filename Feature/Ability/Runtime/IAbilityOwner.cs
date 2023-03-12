using System.Collections.Generic;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力容器
    /// </summary>
    public interface IAbilityOwner : IGameEntityFeature
    {
        /// <summary>
        /// 当前能力列表
        /// </summary>
        protected internal List<IAbility> Abilities { get; set; }

        /// <summary>
        /// 待启用能力列表
        /// </summary>
        protected internal List<IAbility> ToEnableAbilities { get; set; }

        /// <summary>
        /// 待禁用能力列表
        /// </summary>
        protected internal List<IAbility> ToDisableAbilities { get; set; }

        /// <summary>
        /// 当前修改器列表
        /// </summary>
        protected internal List<IAbilityModifier> Modifiers { get; set; }
    }
}