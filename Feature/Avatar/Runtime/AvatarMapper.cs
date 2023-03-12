using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Echo.Avatar
{
    [AddComponentMenu(nameof(Echo) + "/" + nameof(AvatarMapper))]
    public sealed class AvatarMapper : MonoBehaviour
    {
        [Serializable]
        private sealed class Part
        {
            [SerializeField, HideLabel, HorizontalGroup]
            public AvatarPartType Type;

            [SerializeField, HideLabel, HorizontalGroup]
            public SkinnedMeshRenderer Renderer;
        }

        [Serializable]
        private sealed class Slot
        {
            [SerializeField, HideLabel, HorizontalGroup]
            public AvatarSlotType Type;

            [SerializeField, HideLabel, HorizontalGroup]
            public Transform Root;
        }

        [SerializeField]
        private Part[] m_Parts;

        [SerializeField]
        private Slot[] m_Slots;

        [SerializeField]
        private Transform m_AccessoryRoot;

        public SkinnedMeshRenderer GetPartRenderer(AvatarPartType type)
        {
            for (int i = 0; i < m_Parts.Length; i++)
            {
                if (m_Parts[i].Type == type)
                {
                    return m_Parts[i].Renderer;
                }
            }

            return null;
        }

        public Transform GetSlotRoot(AvatarSlotType type)
        {
            for (int i = 0; i < m_Slots.Length; i++)
            {
                if (m_Slots[i].Type == type)
                {
                    return m_Slots[i].Root;
                }
            }

            return null;
        }

        public Transform GetAccessoryRoot()
        {
            return m_AccessoryRoot;
        }
    }
}