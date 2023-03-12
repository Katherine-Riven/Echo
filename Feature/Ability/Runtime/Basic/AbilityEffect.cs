using System;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力效果撤销
    /// </summary>
    public delegate void AbilityEffectCancelHandle();

    /// <summary>
    /// 能力效果
    /// </summary>
    [Serializable]
    public abstract class AbilityEffect : AbilityBehaviour
    {
        /// <summary>
        /// 执行
        /// </summary>
        public abstract AbilityEffectCancelHandle Invoke(IAbilityContext context);
    }
}