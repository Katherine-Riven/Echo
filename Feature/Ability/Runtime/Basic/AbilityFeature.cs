using System;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力功能，用于执行Effect
    /// </summary>
    [Serializable]
    public abstract class AbilityFeature : AbilityBehaviour
    {
        [SerializeField] [SerializeReference] private AbilityCondition m_Condition;

        /// <summary>
        /// 当前是否有效
        /// </summary>
        internal bool IsValid(IAbilityContext context) => m_Condition == null || m_Condition.Check(context);

        /// <summary>
        /// 当启用时
        /// </summary>
        protected internal abstract void OnEnable();

        /// <summary>
        /// 当更新时
        /// </summary>
        protected internal abstract void OnUpdate();

        /// <summary>
        /// 当禁用时
        /// </summary>
        protected internal abstract void OnDisable();
    }
}