using System;
using Echo.Utility;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力功能
    /// </summary>
    [Serializable]
    public sealed class AbilityFeature : AbilityBehaviour
    {
        [SerializeField, SerializeReference]
        private AbilityCondition m_Condition;

        [SerializeField, SerializeReference]
        private AbilityTrigger[] m_Triggers;

        [SerializeField, SerializeReference]
        private AbilityEffect[] m_Effects;

        private PooledList<AbilityTrigger> m_ActiveTriggers;
        private PooledList<AbilityEffect>  m_ActiveEffects;
        private AbilityEffectCancelHandle  m_CancelHandle;

        /// <summary>
        /// 当前是否有效
        /// </summary>
        internal bool IsValid(IAbilityContext context) => m_Condition == null || m_Condition.Check(context);

        protected internal override void OnAbilityEnable()
        {
            base.OnAbilityEnable();
            m_ActiveTriggers = PooledList<AbilityTrigger>.Get();
            m_ActiveEffects  = PooledList<AbilityEffect>.Get();
            m_ActiveTriggers.AddRange(m_Triggers);
            m_ActiveEffects.AddRange(m_Effects);
        }

        protected internal override void OnAbilityDisable()
        {
            base.OnAbilityDisable();
            m_ActiveTriggers.Dispose();
            m_ActiveEffects.Dispose();
        }

        /// <summary>
        /// 当启用时
        /// </summary>
        internal void OnEnable()
        {
            for (int i = 0; i < m_Triggers.Length; i++)
            {
                m_Triggers[i].OnTrigger += OnTrigger;
            }

            for (int i = 0; i < m_Triggers.Length; i++)
            {
                m_Triggers[i].OnEnable();
            }
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        internal void OnUpdate()
        {
            for (int i = 0; i < m_Triggers.Length; i++)
            {
                m_Triggers[i].OnUpdate();
            }
        }

        /// <summary>
        /// 当禁用时
        /// </summary>
        internal void OnDisable()
        {
            for (int i = 0; i < m_Triggers.Length; i++)
            {
                m_Triggers[i].OnDisable();
            }

            for (int i = 0; i < m_ActiveTriggers.Count; i++)
            {
                m_ActiveTriggers[i].OnTrigger -= OnTrigger;
            }

            m_CancelHandle?.Invoke();
            m_CancelHandle = null;
        }

        /// <summary>
        /// 当触发时
        /// </summary>
        private void OnTrigger(IAbilityContext context)
        {
            for (int i = 0; i < m_Effects.Length; i++)
            {
                m_CancelHandle += m_Effects[i].Invoke(context);
            }
        }

        /// <summary>
        /// 新增触发器
        /// </summary>
        public void AddTrigger(AbilityTrigger trigger)
        {
            if (m_ActiveTriggers.Contains(trigger))
            {
                Debug.LogError($"Already contains trigger {trigger.GetType().Name}({trigger.Name}) in feature {Name}, can't add again.");
                return;
            }

            trigger.OnTrigger += OnTrigger;
            m_ActiveTriggers.Add(trigger);
        }

        /// <summary>
        /// 移除触发器
        /// </summary>
        public void RemoveTrigger(AbilityTrigger trigger)
        {
            if (m_ActiveTriggers.Remove(trigger))
            {
                trigger.OnTrigger -= OnTrigger;
            }
            else
            {
                Debug.LogError($"Can't remove trigger {trigger.GetType().Name}({trigger.Name}), it's not exist in feature {Name}");
            }
        }

        /// <summary>
        /// 启用触发器
        /// </summary>
        public void EnableTrigger(string triggerName)
        {
            if (string.IsNullOrEmpty(triggerName)) return;
            for (int i = 0; i < m_Triggers.Length; i++)
            {
                AbilityTrigger trigger = m_Triggers[i];
                if (trigger.Name == triggerName && m_ActiveTriggers.Contains(trigger) == false)
                {
                    trigger.OnTrigger += OnTrigger;
                    m_ActiveTriggers.Add(trigger);
                }
            }
        }

        /// <summary>
        /// 禁用触发器
        /// </summary>
        public void DisableTrigger(string triggerName)
        {
            if (string.IsNullOrEmpty(triggerName)) return;
            for (int i = 0; i < m_Triggers.Length; i++)
            {
                AbilityTrigger trigger = m_Triggers[i];
                if (trigger.Name == triggerName && m_ActiveTriggers.Remove(trigger))
                {
                    trigger.OnTrigger -= OnTrigger;
                }
            }
        }

        /// <summary>
        /// 新增效果
        /// </summary>
        public void AddEffect(AbilityEffect effect)
        {
            if (m_ActiveEffects.Contains(effect))
            {
                Debug.LogError($"Already contains effect {effect.GetType().Name}({effect.Name}) in feature {Name}, can't add again.");
                return;
            }

            m_ActiveEffects.Add(effect);
        }

        /// <summary>
        /// 移除效果
        /// </summary>
        public void RemoveEffect(AbilityEffect effect)
        {
            if (m_ActiveEffects.Remove(effect) == false)
            {
                Debug.LogError($"Can't remove effect {effect.GetType().Name}({effect.Name}), it's not exist in feature {Name}");
            }
        }

        /// <summary>
        /// 启用效果
        /// </summary>
        public void EnableEffect(string effectName)
        {
            if (string.IsNullOrEmpty(effectName)) return;
            for (int i = 0; i < m_Effects.Length; i++)
            {
                AbilityEffect effect = m_Effects[i];
                if (effect.Name == effectName && m_ActiveEffects.Contains(effect) == false)
                {
                    m_ActiveEffects.Add(effect);
                }
            }
        }

        /// <summary>
        /// 禁用效果
        /// </summary>
        public void DisableEffect(string effectName)
        {
            if (string.IsNullOrEmpty(effectName)) return;
            for (int i = 0; i < m_Effects.Length; i++)
            {
                AbilityEffect effect = m_Effects[i];
                if (effect.Name == effectName)
                {
                    m_ActiveEffects.Remove(effect);
                }
            }
        }
    }
}