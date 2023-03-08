using UnityEngine.Pool;

namespace Echo.Abilities
{
    internal sealed class AbilityDriver : GameFeatureDriver
    {
        protected override void OnInitialize()
        {
        }

        protected override void OnEntityEnable(GameEntity entity)
        {
            if (entity is IAbilityOwner abilityOwner)
            {
                abilityOwner.Abilities          = ListPool<Ability>.Get();
                abilityOwner.ToEnableAbilities  = ListPool<Ability>.Get();
                abilityOwner.ToDisableAbilities = ListPool<Ability>.Get();
            }
        }

        protected override void OnEntityDisable(GameEntity entity)
        {
            if (entity is IAbilityOwner abilityOwner)
            {
                foreach (Ability ability in abilityOwner.Abilities)
                {
                    ability.OnDisable();
                    AbilityAPI.ReleaseAbility(ability);
                }

                ListPool<Ability>.Release(abilityOwner.Abilities);
                ListPool<Ability>.Release(abilityOwner.ToEnableAbilities);
                ListPool<Ability>.Release(abilityOwner.ToDisableAbilities);
                abilityOwner.Abilities          = null;
                abilityOwner.ToEnableAbilities  = null;
                abilityOwner.ToDisableAbilities = null;
            }
        }

        protected override void OnUpdate(GameEntityCollection entities)
        {
            foreach (GameEntity entity in entities)
            {
                if (entity is IAbilityOwner abilityOwner)
                {
                    foreach (Ability ability in abilityOwner.Abilities)
                    {
                        ability.OnUpdate();
                    }
                }
            }
        }

        protected override void OnFixedUpdate(GameEntityCollection entities)
        {
        }

        protected override void OnLateUpdate(GameEntityCollection entities)
        {
            foreach (GameEntity entity in entities)
            {
                if (entity is IAbilityOwner abilityOwner)
                {
                    foreach (Ability ability in abilityOwner.ToEnableAbilities)
                    {
                        ability.OnEnable();
                        abilityOwner.Abilities.Add(ability);
                    }

                    foreach (Ability ability in abilityOwner.ToDisableAbilities)
                    {
                        ability.OnDisable();
                        abilityOwner.Abilities.Remove(ability);
                    }

                    abilityOwner.ToEnableAbilities.Clear();
                    abilityOwner.ToDisableAbilities.Clear();
                }
            }
        }

        protected override void OnDispose()
        {
        }
    }
}