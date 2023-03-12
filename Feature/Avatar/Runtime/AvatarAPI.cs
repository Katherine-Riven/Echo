using UnityEngine;

namespace Echo.Avatar
{
    public static class AvatarAPI
    {
        public static void SetPart(this IAvatar avatar, AvatarPartType type, IAssetReference<Mesh> value)
        {
            if (avatar.PartMap.TryGetValue(type, out AvatarPart part))
            {
                if (part.Active != null)
                {
                    ResetPart(avatar, type);
                    Debug.LogError("For grace, reset before set.");
                }

                Mesh mesh = GameManager.AssetSystem.LoadAsset(value);
                part.Renderer.sharedMesh = mesh;
                part.Active              = mesh;
            }
        }

        public static void ResetPart(this IAvatar avatar, AvatarPartType type)
        {
            if (avatar.PartMap.TryGetValue(type, out AvatarPart part) && part.Active != null)
            {
                GameManager.AssetSystem.ReleaseAsset(part.Active);
                part.Renderer.sharedMesh = part.Default;
                part.Active              = null;
            }
        }

        public static void SetSlot(this IAvatar avatar, AvatarSlotType type, IAssetReference<GameObject> value)
        {
            if (avatar.SlotMap.TryGetValue(type, out AvatarSlot slot))
            {
                if (slot.Active != null)
                {
                    ResetSlot(avatar, type);
                    Debug.LogError("For grace, reset before set.");
                }

                if (slot.Default != null)
                {
                    slot.Default.SetActive(false);
                }

                avatar.Profile.GetOffset(value.AssetGUID, out Vector3 position, out Quaternion rotation);
                GameObject gameObject = GameManager.AssetSystem.Instantiate(value, position, rotation);
                gameObject.transform.SetParent(slot.Root, false);
                slot.Active = gameObject;
            }
        }

        public static void ResetSlot(this IAvatar avatar, AvatarSlotType type)
        {
            if (avatar.SlotMap.TryGetValue(type, out AvatarSlot slot) && slot.Active != null)
            {
                GameManager.AssetSystem.ReleaseAsset(slot.Active);
                if (slot.Default != null)
                {
                    slot.Default.SetActive(true);
                }
            }
        }

        public static void AddAccessory(this IAvatar avatar, IAssetReference<GameObject> accessory)
        {
            if (avatar.Accessory.Contains(accessory.AssetGUID))
            {
                Debug.LogError($"Already add accessory {accessory.AssetGUID}, can't add twice.");
                return;
            }

            avatar.Profile.GetOffset(accessory.AssetGUID, out Vector3 position, out Quaternion rotation);
            GameObject instance = GameManager.AssetSystem.Instantiate(accessory, position, rotation);
            instance.transform.SetParent(avatar.Accessory.Root, false);
            avatar.Accessory.Add(accessory.AssetGUID, instance);
        }

        public static void RemoveAccessory(this IAvatar avatar, IAssetReference<GameObject> accessory)
        {
            if (avatar.Accessory.Remove(accessory.AssetGUID, out GameObject instance))
            {
                GameManager.AssetSystem.ReleaseInstance(instance);
            }
        }
    }
}