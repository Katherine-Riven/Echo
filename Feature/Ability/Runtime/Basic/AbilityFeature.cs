using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力功能
    /// </summary>
    [Serializable]
    public abstract class AbilityFeature : AbilityElement
    {
        [SerializeField] [LabelText("生效标签")] private AbilityTag m_ActiveTag;

        /// <summary>
        /// 生效标签
        /// </summary>
        public AbilityTag ActiveTag => m_ActiveTag;

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public abstract string GetTooltip();

        /// <summary>
        /// 当功能启用
        /// </summary>
        protected internal abstract void OnEnable();

        /// <summary>
        /// 当功能更新
        /// </summary>
        protected internal abstract void OnUpdate();

        /// <summary>
        /// 当功能失效
        /// </summary>
        protected internal abstract void OnDisable();

        public sealed override string ToString() => GetTooltip();
    }
}