using System;
using UnityEngine;

// ReSharper disable UseArrayEmptyMethod
// ReSharper disable Unity.RedundantSerializeFieldAttribute

namespace Echo.Abilities
{
    /// <summary>
    /// 能力配置
    /// </summary>
    [Serializable]
    public abstract class Ability : IAbility, ISerializationCallbackReceiver
    {
        [SerializeField]
        private AbilityTag m_Tag;

        [SerializeField, SerializeReference]
        private IAbilityVariable[] m_Variables;

        internal AbilityBehaviour[] Behaviours;
        private  AbilityTagSet      m_TagSet = new AbilityTagSet();

        /// <summary>
        /// 是否生效中
        /// </summary>
        public bool IsEnable { get; private set; }

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
        /// 查找变量
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

        protected abstract AbilityTag DefaultTag { get; }

        /// <summary>
        /// 查找功能
        /// </summary>
        public abstract AbilityFeature GetFeature(string featureName);

        protected abstract void OnEnable();
        protected abstract void OnUpdate();
        protected abstract void OnDisable();

        protected virtual void OnBeforeSerialize()
        {
        }

        protected virtual void OnAfterDeserialize()
        {
        }

        bool IAbility.IsEnable
        {
            get => IsEnable;
            set => IsEnable = value;
        }

        IAbilityOwner IAbility.Owner
        {
            get => Owner;
            set => Owner = value;
        }

        /// <summary>
        /// 当启用时
        /// </summary>
        void IAbility.OnEnable()
        {
            foreach (AbilityBehaviour behaviour in Behaviours)
            {
                behaviour.OnAbilityEnable();
            }

            m_TagSet.Reset(m_Tag | DefaultTag);
            OnEnable();
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        void IAbility.OnUpdate()
        {
            foreach (AbilityBehaviour behaviour in Behaviours)
            {
                behaviour.OnAbilityUpdate();
            }

            OnUpdate();
        }

        /// <summary>
        /// 当禁用时
        /// </summary>
        void IAbility.OnDisable()
        {
            OnDisable();
            foreach (AbilityBehaviour behaviour in Behaviours)
            {
                behaviour.OnAbilityDisable();
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_Variables ??= new IAbilityVariable[0];
            OnBeforeSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_Variables ??= new IAbilityVariable[0];
            OnAfterDeserialize();
        }
    }
}