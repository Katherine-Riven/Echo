using System;

namespace Echo.Abilities
{
    /// <summary>
    /// 触发器事件
    /// </summary>
    public delegate void AbilityTriggerEvent(IAbilityContext context);

    /// <summary>
    /// 能力触发器
    /// </summary>
    [Serializable]
    public abstract class AbilityTrigger : AbilityBehaviour
    {
        /// <summary>
        /// 当触发时
        /// </summary>
        protected internal abstract event AbilityTriggerEvent OnTrigger;

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