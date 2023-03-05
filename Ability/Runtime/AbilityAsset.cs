using UnityEngine;

namespace Echo.Abilities
{
    [CreateAssetMenu(menuName = nameof(Echo) + "/" + nameof(Ability))]
    public sealed class AbilityAsset : ScriptableObject
    {
        [SerializeField] private Ability m_Ability;
        
        public Ability Ability => m_Ability;
    }
}