using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力容器
    /// </summary>
    public interface IAbilityDriver : IGameDrive
    {
        /// <summary>
        /// 位置
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// 旋转
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// 当前能力列表
        /// </summary>
        protected internal List<Ability> Abilities { get; set; }

        /// <summary>
        /// 待启用能力列表
        /// </summary>
        protected internal List<Ability> ToEnableAbilities { get; set; }
        
        /// <summary>
        /// 待禁用能力列表
        /// </summary>
        protected internal List<Ability> ToDisableAbilities { get; set; }
        
        /// <summary>
        /// 当前修改器列表
        /// </summary>
        protected internal List<IAbilityModifier> Modifiers { get; set; }
    }
}