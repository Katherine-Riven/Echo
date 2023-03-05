using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

        [SerializeField] [LabelText("标签")] 
        private AbilityTag m_Tag = new AbilityTag();

        [SerializeField] [LabelText("变量表")] 
        private AbilityVariableTable m_VariableTable = new AbilityVariableTable();

        [SerializeField] [LabelText("当启用时")] 
        private AbilityEffects m_OnEnable = new AbilityEffects();

        [SerializeField] [LabelText("当禁用时")] 
        private AbilityEffects m_OnDisable = new AbilityEffects();

        [SerializeReference] [LabelText("功能列表")]
        private AbilityFeature[] m_Features = new AbilityFeature[0];

        [HideInInspector] [SerializeReference] 
        private AbilityElement[] m_Elements = new AbilityElement[0];

        [NonSerialized] private  List<AbilityFeature> m_ActiveFeatures;
        [NonSerialized] private  AbilityTagSet        m_TagSet;
        [NonSerialized] internal string               GUID;

        #endregion

        #region API

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool IsActive => Owner != null && Owner.Abilities.Contains(this);

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
            UpdateActiveFeatures();
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public void RemoveTag(in AbilityTag tag)
        {
            m_TagSet.Remove(tag);
            UpdateActiveFeatures();
        }

        /// <summary>
        /// 清空额外的标签
        /// </summary>
        public void ClearTag()
        {
            m_TagSet.Clear();
            UpdateActiveFeatures();
        }

        /// <summary>
        /// 查询修改器
        /// </summary>
        public AbilityModifierQuery<T> QueryModifier<T>() where T : IAbilityModifier
        {
            return new AbilityModifierQuery<T>(Owner.Modifiers);
        }

        #endregion

        #region Life Event

        /// <summary>
        /// 当获取时
        /// </summary>
        internal void OnEnable(IAbilityOwner owner, IAbilityInitializer initializer)
        {
            Owner            = owner;
            m_ActiveFeatures = ListPool<AbilityFeature>.Get();
            m_TagSet         = GenericPool<AbilityTagSet>.Get();
            initializer?.InitializeVariables(m_VariableTable);
            foreach (AbilityElement element in m_Elements)
            {
                element.Ability = this;
                element.OnAbilityEnable();
            }

            m_TagSet.Reset(m_Tag);
            using AbilityContext context = AbilityContext.GetPooled(this);
            m_OnEnable.Invoke(context);
            UpdateActiveFeatures();
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        internal void OnUpdate()
        {
            for (int i = 0, length = m_ActiveFeatures.Count; i < length; i++)
            {
                m_ActiveFeatures[i].OnUpdate();
            }
        }

        /// <summary>
        /// 当失去时
        /// </summary>
        internal void OnDisable()
        {
            using AbilityContext context = AbilityContext.GetPooled(this);
            m_OnDisable.Invoke(context);

            foreach (AbilityFeature activeFeature in m_ActiveFeatures)
            {
                activeFeature.OnDisable();
            }

            ListPool<AbilityFeature>.Release(m_ActiveFeatures);

            m_TagSet.Clear();
            GenericPool<AbilityTagSet>.Release(m_TagSet);
            Owner            = null;
            m_ActiveFeatures = null;
            m_TagSet         = null;
            foreach (AbilityElement element in m_Elements)
            {
                element.OnAbilityDisable();
                element.Ability = null;
            }
        }

        #endregion

        #region Utility

        private void UpdateActiveFeatures()
        {
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                AbilityFeature feature     = m_Features[i];
                int            activeIndex = m_ActiveFeatures.IndexOf(feature);
                if (activeIndex >= 0)
                {
                    if (Tag.HasAllTag(feature.ActiveTag) == false)
                    {
                        feature.OnDisable();
                        m_ActiveFeatures.RemoveAt(activeIndex);
                    }
                }
                else
                {
                    if (Tag.HasAllTag(feature.ActiveTag))
                    {
                        feature.OnEnable();
                        m_ActiveFeatures.Add(feature);
                    }
                }
            }
        }

        #endregion
    }
}