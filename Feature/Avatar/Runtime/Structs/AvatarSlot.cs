using UnityEngine;

namespace Echo.Avatar
{
    public enum AvatarSlotType
    {
        LeftHand,
        RightHand,
        Backpack,
        Head,
        Hat,
        Hair,
        Eyebrow,
        Eye,
        Mouth,
    }

    public sealed class AvatarSlot
    {
        public AvatarSlot(Transform root)
        {
            Root    = root;
            Default = root.childCount > 0 ? root.GetChild(0).gameObject : null;
        }

        public Transform  Root    { get; }
        public GameObject Default { get; }
        public GameObject Active  { get; set; }
    }
}