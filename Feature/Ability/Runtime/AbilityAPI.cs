using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    public static class AbilityAPI
    {
        private static readonly List<AbilityBehaviour> s_BehaviourCollector = new List<AbilityBehaviour>();

        private class Factory : IDisposable
        {
            private string                         m_GUID;
            private IAssetLoadHandle<AbilityAsset> m_AssetHandle;
            private string                         m_Json;
            private int                            m_ReferenceCount;

            public int ReferenceCount => m_ReferenceCount;

            public Factory(IAbilityReference reference)
            {
                m_GUID           = reference.GUID;
                m_AssetHandle    = GameManager.AssetSystem.LoadAsync(reference);
                m_Json           = JsonUtility.ToJson(m_AssetHandle.WaitForCompletion().Ability);
                m_ReferenceCount = 0;
            }

            public Ability CreateInstance()
            {
                AbilityBehaviour.s_Collector = s_BehaviourCollector;
                Ability ability = JsonUtility.FromJson<Ability>(m_Json);
                ability.GUID       = m_GUID;
                ability.Behaviours = s_BehaviourCollector.ToArray();
                s_BehaviourCollector.Clear();
                AbilityBehaviour.s_Collector = null;
                m_ReferenceCount++;
                return ability;
            }

            public void ReleaseInstance()
            {
                if (m_ReferenceCount <= 0)
                {
                    throw new Exception();
                }

                m_ReferenceCount--;
            }

            public void Dispose()
            {
                if (m_AssetHandle == null) return;
                GameManager.AssetSystem.Release(m_AssetHandle);
                m_AssetHandle = null;
                m_Json        = null;
            }
        }

        private static readonly Dictionary<string, Factory> s_FactoryMap = new Dictionary<string, Factory>();

        /// <summary>
        /// 启用能力
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="reference">能力资源引用</param>
        /// <param name="initializer">能力初始化</param>
        public static Ability EnableAbility(this IAbilityOwner owner, IAbilityReference reference, IAbilityInitializer initializer = null)
        {
            Factory factory;
            if (s_FactoryMap.TryGetValue(reference.GUID, out factory) == false)
            {
                factory = new Factory(reference);
                s_FactoryMap.Add(reference.GUID, factory);
            }

            Ability ability = factory.CreateInstance();
            ability.OnEnable(owner, initializer);
            owner.Abilities.Add(ability);
            return ability;
        }

        /// <summary>
        /// 禁用能力
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="ability">能力实例</param>
        public static void DisableAbility(this IAbilityOwner owner, Ability ability)
        {
            if (owner.Abilities.Remove(ability))
            {
                ability.OnDisable();
                ReleaseAbility(ability);
            }
        }

        /// <summary>
        /// 添加修改器
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="modifier">修改器实例</param>
        public static void AddModifier(this IAbilityOwner owner, IAbilityModifier modifier)
        {
            owner.Modifiers.Add(modifier);
        }

        /// <summary>
        /// 移除修改器
        /// </summary>
        /// <param name="owner">持有者</param>
        /// <param name="modifier">修改器实例</param>
        public static void RemoveModifier(this IAbilityOwner owner, IAbilityModifier modifier)
        {
            owner.Modifiers.Remove(modifier);
        }

        internal static void ReleaseAbility(Ability ability)
        {
            Factory factory = s_FactoryMap[ability.GUID];
            factory.ReleaseInstance();
        }
    }
}