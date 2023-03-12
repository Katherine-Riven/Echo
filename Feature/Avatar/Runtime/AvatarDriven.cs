using UnityEngine;
using UnityEngine.Pool;

namespace Echo.Avatar
{
    internal sealed class AvatarDriven : GameFeatureDriven
    {
        [SerializeField]
        private AvatarProfile m_Profile;

        protected override void OnInitialize()
        {
        }

        protected override void OnEntityEnable(GameEntity entity)
        {
            if (entity is IAvatar avatar && entity.GameObject.TryGetComponent(out AvatarMapper profile))
            {
                avatar.Profile   = m_Profile;
                avatar.PartMap   = DictionaryPool<AvatarPartType, AvatarPart>.Get();
                avatar.SlotMap   = DictionaryPool<AvatarSlotType, AvatarSlot>.Get();
                avatar.Accessory = GenericPool<AvatarAccessory>.Get();

                avatar.PartMap.Add(AvatarPartType.Body,  new AvatarPart(profile.GetPartRenderer(AvatarPartType.Body)));
                avatar.PartMap.Add(AvatarPartType.Cloak, new AvatarPart(profile.GetPartRenderer(AvatarPartType.Body)));

                avatar.SlotMap.Add(AvatarSlotType.LeftHand,  new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.LeftHand)));
                avatar.SlotMap.Add(AvatarSlotType.RightHand, new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.RightHand)));
                avatar.SlotMap.Add(AvatarSlotType.Backpack,  new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Backpack)));
                avatar.SlotMap.Add(AvatarSlotType.Head,      new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Head)));
                avatar.SlotMap.Add(AvatarSlotType.Hat,       new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Hat)));
                avatar.SlotMap.Add(AvatarSlotType.Hair,      new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Hair)));
                avatar.SlotMap.Add(AvatarSlotType.Eyebrow,   new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Eyebrow)));
                avatar.SlotMap.Add(AvatarSlotType.Eye,       new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Eye)));
                avatar.SlotMap.Add(AvatarSlotType.Mouth,     new AvatarSlot(profile.GetSlotRoot(AvatarSlotType.Mouth)));

                avatar.Accessory.Root = profile.GetAccessoryRoot();
            }
        }

        protected override void OnEntityDisable(GameEntity entity)
        {
            if (entity is IAvatar avatar)
            {
                foreach (AvatarPart part in avatar.PartMap.Values)
                {
                    if (part.Active != null)
                    {
                        GameManager.AssetSystem.ReleaseAsset(part.Active);
                    }
                }

                foreach (AvatarSlot slot in avatar.SlotMap.Values)
                {
                    if (slot.Active != null)
                    {
                        GameManager.AssetSystem.ReleaseAsset(slot.Active);
                    }
                }

                foreach (AvatarPart part in avatar.PartMap.Values)
                {
                    if (part.Active != null)
                    {
                        GameManager.AssetSystem.ReleaseAsset(part.Active);
                    }
                }

                DictionaryPool<AvatarPartType, AvatarPart>.Release(avatar.PartMap);
                DictionaryPool<AvatarSlotType, AvatarSlot>.Release(avatar.SlotMap);
                GenericPool<AvatarAccessory>.Release(avatar.Accessory);
                avatar.Profile        = null;
                avatar.PartMap        = null;
                avatar.SlotMap        = null;
                avatar.Accessory.Root = null;
                avatar.Accessory      = null;
            }
        }

        protected override void OnUpdate(GameEntityCollection entities)
        {
        }

        protected override void OnFixedUpdate(GameEntityCollection entities)
        {
        }

        protected override void OnLateUpdate(GameEntityCollection entities)
        {
        }

        protected override void OnDispose()
        {
        }
    }
}