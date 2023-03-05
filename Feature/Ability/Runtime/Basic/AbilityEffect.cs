using System;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力效果
    /// </summary>
    [Serializable]
    [AbilityMenuItem("效果")]
    public abstract class AbilityEffect
    {
        /// <summary>
        /// 生效
        /// </summary>
        public abstract void Invoke(IAbilityContext context);
    }

    /// <summary>
    /// 能力可撤销效果
    /// </summary>
    [Serializable]
    [AbilityMenuItem("可撤销效果")]
    public abstract class AbilityCancelableEffect : AbilityEffect
    {
        /// <summary>
        /// 撤销
        /// </summary>
        public abstract void Cancel(IAbilityContext context);
    }

    /// <summary>
    /// 能力效果
    /// </summary>
    [Serializable]
    public sealed class AbilityEffects
    {
        [SerializeReference] private AbilityEffect[] m_Effects = new AbilityEffect[0];

        /// <summary>
        /// 生效
        /// </summary>
        public void Invoke(IAbilityContext context)
        {
            for (int i = 0, length = m_Effects.Length; i < length; i++)
            {
                m_Effects[i].Invoke(context);
            }
        }
    }

    /// <summary>
    /// 能力可撤销效果
    /// </summary>
    [Serializable]
    public sealed class AbilityCancelableEffects
    {
        [SerializeReference] private AbilityCancelableEffect[] m_Effects = new AbilityCancelableEffect[0];

        /// <summary>
        /// 生效
        /// </summary>
        public void Invoke(IAbilityContext context)
        {
            for (int i = 0, length = m_Effects.Length; i < length; i++)
            {
                m_Effects[i].Invoke(context);
            }
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Cancel(IAbilityContext context)
        {
            for (int i = 0, length = m_Effects.Length; i < length; i++)
            {
                m_Effects[i].Cancel(context);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}