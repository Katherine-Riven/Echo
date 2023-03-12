using System.Collections.Generic;

namespace Echo.Avatar
{
    public interface IAvatar : IGameFeature
    {
        protected internal AvatarProfile                          Profile   { get; set; }
        protected internal Dictionary<AvatarPartType, AvatarPart> PartMap   { get; set; }
        protected internal Dictionary<AvatarSlotType, AvatarSlot> SlotMap   { get; set; }
        protected internal AvatarAccessory                        Accessory { get; set; }
    }
}