using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// ReSharper disable UseArrayEmptyMethod

namespace Echo.Abilities
{
    /// <summary>
    /// 能力配置
    /// </summary>
    [Serializable]
    public sealed class Ability
    {
        #region Field

        [SerializeField]     private AbilityTag           m_Tag           = new AbilityTag();
        [SerializeField]     private AbilityVariableTable m_VariableTable = new AbilityVariableTable();
        [SerializeReference] private AbilityFeature[]     m_Features      = new AbilityFeature[0];
        [SerializeReference] private AbilityEffect[]      m_Effects       = new AbilityEffect[0];

        [NonSerialized] internal string               GUID;
        [NonSerialized] internal AbilityBehaviour[]   Behaviours;
        [NonSerialized] private  AbilityTagSet        m_TagSet = new AbilityTagSet();
        [NonSerialized] private  List<AbilityFeature> m_ActiveFeatures;

        #endregion

        #region API

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// 持有者
        /// </summary>
        public IAbilityOwner Owner { get; private set; }

        /// <summary>
        /// 标签
        /// </summary>
        public AbilityTag Tag => m_TagSet.Tag;

        /// <summary>
        /// 添加标签
        /// </summary>
        public void AddTag(in AbilityTag tag)
        {
            m_TagSet.Add(tag);
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public void RemoveTag(in AbilityTag tag)
        {
            m_TagSet.Remove(tag);
        }

        /// <summary>
        /// 清空额外的标签
        /// </summary>
        public void ClearTag()
        {
            m_TagSet.Clear();
        }

        /// <summary>
        /// 查询修改器
        /// </summary>
        public AbilityModifierQuery<T> QueryModifier<T>() where T : IAbilityModifier
        {
            return new AbilityModifierQuery<T>(this, Owner.Modifiers);
        }

        #endregion

        #region Life Event

        /// <summary>
        /// 当获取时
        /// </summary>
        internal void OnEnable(IAbilityOwner owner, IAbilityInitializer initializer)
        {
            IsActive         = true;
            Owner            = owner;
            m_ActiveFeatures = ListPool<AbilityFeature>.Get();
            initializer?.InitializeVariables(m_VariableTable);
            foreach (AbilityBehaviour behaviour in Behaviours)
            {
                behaviour.Ability = this;
                behaviour.OnAbilityEnable();
            }

            m_TagSet.Reset(m_Tag);
            using AbilityContext context = AbilityContext.GetPooled(this);
            foreach (AbilityFeature feature in m_Features)
            {
                if (feature.IsValid(context))
                {
                    m_ActiveFeatures.Add(feature);
                    feature.OnEnable();
                }
            }
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        internal void OnUpdate()
        {
            using AbilityContext context = AbilityContext.GetPooled(this);
            foreach (AbilityFeature feature in m_Features)
            {
                bool isValid     = feature.IsValid(context);
                int  activeIndex = m_ActiveFeatures.IndexOf(feature);
                if (isValid == activeIndex >= 0)
                {
                    continue;
                }

                if (isValid)
                {
                    m_ActiveFeatures.Add(feature);
                    feature.OnEnable();
                }
                else
                {
                    m_ActiveFeatures.RemoveAt(activeIndex);
                    feature.OnDisable();
                }
            }

            foreach (AbilityFeature activeFeature in m_ActiveFeatures)
            {
                activeFeature.OnUpdate();
            }
        }

        /// <summary>
        /// 当失去时
        /// </summary>
        internal void OnDisable()
        {
            IsActive = false;
            foreach (AbilityFeature activeFeature in m_ActiveFeatures)
            {
                activeFeature.OnDisable();
            }

            ListPool<AbilityFeature>.Release(m_ActiveFeatures);
            m_ActiveFeatures = null;
            foreach (AbilityBehaviour behaviour in Behaviours)
            {
                behaviour.OnAbilityDisable();
            }
        }

        #endregion
    }
}