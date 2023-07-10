using System;

namespace Echo.Abilities
{
    [Serializable]
    public sealed class AbilityFeature : GameFeature
    {
        protected override void OnInitialize()
        {
        }

        protected override void OnStart()
        {
        }

        protected override void OnDispose()
        {
        }

        protected override void OnUpdate()
        {
            foreach (IAbilityOwner abilityHandler in GameManager.QueryActiveEntities<IAbilityOwner>())
            {
                foreach (IAbility ability in abilityHandler.Abilities)
                {
                    ability.OnUpdate();
                }
            }
        }

        protected override void OnFixedUpdate()
        {
        }

        protected override void OnLateUpdate()
        {
        }
    }
}