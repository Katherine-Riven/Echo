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
                owner.Abilities          = ListPool<IAbility>.Get();
                owner.ToEnableAbilities  = ListPool<IAbility>.Get();
                owner.ToDisableAbilities = ListPool<IAbility>.Get();
            }
        }

        protected override void OnEntityDisable(GameEntity entity)
        {
            if (entity is IAbilityOwner owner)
            {
                foreach (IAbility ability in owner.Abilities)
                {
                    ability.OnDisable();
                }

                ListPool<IAbility>.Release(owner.Abilities);
                ListPool<IAbility>.Release(owner.ToEnableAbilities);
                ListPool<IAbility>.Release(owner.ToDisableAbilities);
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
                    foreach (IAbility ability in owner.Abilities)
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
                    foreach (IAbility ability in owner.ToEnableAbilities)
                    {
                        ability.OnEnable();
                        owner.Abilities.Add(ability);
                    }

                    foreach (IAbility ability in owner.ToDisableAbilities)
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