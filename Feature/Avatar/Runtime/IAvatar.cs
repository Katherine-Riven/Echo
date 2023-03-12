using System.Collections.Generic;
using UnityEngine;

namespace Echo.Avatar
{
    public interface IAvatar : IGameFeature
    {
        protected internal AvatarOffsetProfile                    OffsetProfile { get; set; }
        protected internal Dictionary<AvatarPartType, AvatarPart> PartMap       { get; set; }
        protected internal Dictionary<AvatarSlotType, AvatarSlot> SlotMap       { get; set; }
        protected internal AvatarAccessory                        Accessory     { get; set; }
    }

    public sealed class AvatarAccessory
    {
        public Transform Root { get; set; }

        private Dictionary<string, GameObject> m_Map = new Dictionary<string, GameObject>();

        public Dictionary<string, GameObject>.ValueCollection Instances => m_Map.Values;

        public bool Contains(string guid)
        {
            return m_Map.ContainsKey(guid);
        }

        public void Add(string guid, GameObject accessory)
        {
            m_Map.Add(guid, accessory);
        }

        public bool Remove(string guid, out GameObject accessory)
        {
            return m_Map.Remove(guid, out accessory);
        }
    }
}