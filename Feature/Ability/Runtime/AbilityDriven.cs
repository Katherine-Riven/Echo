using UnityEngine.Pool;

namespace Echo.Abilities
{
    internal sealed class AbilityDriven : GameFeatureDriven
    {
        protected override void OnInitialize()
        {
        }

        protected override void OnEntityEnable(GameEntity entity)
        {
            if (entity is IAbilityOwner owner)
            {
                owner.Abilities          = ListPool<Ability>.Get();
                owner.ToEnableAbilities  = ListPool<Ability>.Get();
                owner.ToDisableAbilities = ListPool<Ability>.Get();
            }
        }

        protected override void OnEntityDisable(GameEntity entity)
        {
            if (entity is IAbilityOwner owner)
            {
                foreach (Ability ability in owner.Abilities)
                {
                    ability.OnDisable();
                }

                ListPool<Ability>.Release(owner.Abilities);
                ListPool<Ability>.Release(owner.ToEnableAbilities);
                ListPool<Ability>.Release(owner.ToDisableAbilities);
                owner.Abilities          = null;
                owner.ToEnableAbilities  = null;
                owner.ToDisableAbilities = null;
            }
        }

        protected override void OnUpdate(GameEntityCollection entities)
        {
            foreach (GameEntity entity in entities)
            {
                if (entity is IAbilityOwner owner)
                {
                    foreach (Ability ability in owner.Abilities)
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
                if (entity is IAbilityOwner owner)
                {
                    foreach (Ability ability in owner.ToEnableAbilities)
                    {
                        ability.OnEnable();
                        owner.Abilities.Add(ability);
                    }

                    foreach (Ability ability in owner.ToDisableAbilities)
                    {
                        ability.OnDisable();
                        owner.Abilities.Remove(ability);
                    }

                    owner.ToEnableAbilities.Clear();
                    owner.ToDisableAbilities.Clear();
                }
            }
        }

        protected override void OnDispose()
        {
        }
    }
}