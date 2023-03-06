using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力容器
    /// </summary>
    public interface IAbilityOwner : IGameFeature
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
        protected internal List<Ability> Abilities { get; }

        /// <summary>
        /// 当前修改器列表
        /// </summary>
        protected internal List<IAbilityModifier> Modifiers { get; }
    }
}