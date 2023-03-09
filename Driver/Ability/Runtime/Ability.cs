using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// ReSharper disable Unity.RedundantSerializeFieldAttribute
// ReSharper disable UnassignedField.Local

namespace Echo.Abilities
{
    /// <summary>
    /// 能力配置
    /// </summary>
    [Serializable]
    public sealed class Ability
    {
        #region Field

        [SerializeField]
        private AbilityTag m_Tag;

        [SerializeField, SerializeReference]
        private IAbilityVariable[] m_Variables;

        [SerializeField, SerializeReference]
        private AbilityFeature[] m_Features;

        internal string               GUID;
        internal AbilityBehaviour[]   Behaviours;
        private  AbilityTagSet        m_TagSet = new AbilityTagSet();
        private  List<AbilityFeature> m_ActiveFeatures;

        #endregion

        #region API

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// 持有者
        /// </summary>
        public IAbilityDriver Driver { get; private set; }

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
        /// 获取变量
        /// </summary>
        public AbilityVariable<T> GetVariable<T>(string variableName)
        {
            for (int i = 0; i < m_Variables.Length; i++)
            {
                if (m_Variables[i].Name == variableName && m_Variables[i] is AbilityVariable<T> result)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 查询修改器
        /// </summary>
        public AbilityModifierQuery<T> QueryModifier<T>() where T : IAbilityModifier
        {
            return new AbilityModifierQuery<T>(this, Driver.Modifiers);
        }

        #endregion

        #region Life Event

        /// <summary>
        /// 初始化
        /// </summary>
        internal void OnInitialize(IAbilityDriver driver)
        {
            Driver           = driver;
            m_ActiveFeatures = ListPool<AbilityFeature>.Get();
        }

        /// <summary>
        /// 当获取时
        /// </summary>
        internal void OnEnable()
        {
            IsActive = true;
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

            foreach (AbilityBehaviour behaviour in Behaviours)
            {
                behaviour.OnAbilityDisable();
            }

            ListPool<AbilityFeature>.Release(m_ActiveFeatures);
            m_ActiveFeatures = null;
        }

        #endregion
    }
}