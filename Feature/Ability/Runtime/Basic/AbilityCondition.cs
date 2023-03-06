using System;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力条件
    /// </summary>
    [Serializable]
    public abstract class AbilityCondition : AbilityBehaviour
    {
        /// <summary>
        /// 判断结果
        /// </summary>
        protected internal abstract bool Check(IAbilityContext context);
    }
}