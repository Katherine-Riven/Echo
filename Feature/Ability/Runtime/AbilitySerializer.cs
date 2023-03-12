using System.Collections.Generic;
using UnityEngine;

namespace Echo.Abilities
{
    internal static class AbilitySerializer
    {
        private static readonly List<AbilityBehaviour> s_BehaviourCollector = new List<AbilityBehaviour>();

        public static Ability FromProfile(AbilityProfile profile)
        {
            AbilityBehaviour.s_Collector = s_BehaviourCollector;
            Ability ability = JsonUtility.FromJson<Ability>(profile.AbilityData);
            ability.Behaviours           = s_BehaviourCollector.ToArray();
            AbilityBehaviour.s_Collector = null;
            s_BehaviourCollector.Clear();
            return ability;
        }
    }
}