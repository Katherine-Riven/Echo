using System;

namespace Echo.Abilities
{
    [Serializable]
    public abstract class AbilityElement
    {
        /// <summary>
        /// 所属能力
        /// </summary>
        protected internal Ability Ability { get; internal set; }

        /// <summary>
        /// 当能力启用时
        /// </summary>
        public virtual void OnAbilityEnable()
        {
        }

        /// <summary>
        /// 当能力被禁用时
        /// </summary>
        public virtual void OnAbilityDisable()
        {
        }
    }
}