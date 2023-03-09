using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    internal static class AbilityFactory
    {
        private static readonly List<AbilityBehaviour>      s_BehaviourCollector = new List<AbilityBehaviour>();
        private static readonly Dictionary<string, Factory> s_FactoryMap         = new Dictionary<string, Factory>();

        public static Ability CreateAbility(IAbilityReference reference)
        {
            Factory factory;
            if (s_FactoryMap.TryGetValue(reference.GUID, out factory) == false)
            {
                factory = new Factory(reference);
                s_FactoryMap.Add(reference.GUID, factory);
            }

            Ability ability = factory.CreateInstance();
            return ability;
        }

        internal static void ReleaseAbility(Ability ability)
        {
            Factory factory = s_FactoryMap[ability.GUID];
            factory.ReleaseInstance();
        }

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
                if (m_ReferenceCount >= 0)
                {
                    throw new Exception();
                }

                if (m_AssetHandle == null)
                {
                    throw new Exception();
                }

                GameManager.AssetSystem.Release(m_AssetHandle);
                m_AssetHandle = null;
                m_Json        = null;
            }
        }
    }
}