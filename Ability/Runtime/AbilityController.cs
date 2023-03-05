using System.Collections.Generic;

namespace Echo.Abilities
{
    internal sealed class AbilityController : GameFeatureController
    {
        private List<Ability> m_UpdatingAbilities = new List<Ability>();

        protected override void OnInitialize()
        {
        }

        protected override void OnEntityEnable(GameEntity entity, IGameEntityOrder order)
        {
        }

        protected override void OnEntityDisable(GameEntity entity)
        {
            if (entity is IAbilityOwner abilityOwner)
            {
                foreach (Ability ability in abilityOwner.Abilities)
                {
                    //ReleaseAbility(ability);
                }

                abilityOwner.Abilities.Clear();
            }
        }

        protected override void OnUpdate(IReadOnlyList<GameEntity> entities)
        {
            foreach (GameEntity entity in entities)
            {
                if (entity is IAbilityOwner abilityOwner)
                {
                    m_UpdatingAbilities.Clear();
                    m_UpdatingAbilities.AddRange(abilityOwner.Abilities);
                    foreach (Ability ability in m_UpdatingAbilities)
                    {
                        ability.OnUpdate();
                    }
                }
            }
        }

        protected override void OnFixedUpdate(IReadOnlyList<GameEntity> entities)
        {
        }

        protected override void OnLateUpdate(IReadOnlyList<GameEntity> entities)
        {
        }

        protected override void OnDispose()
        {
        }
    }
}