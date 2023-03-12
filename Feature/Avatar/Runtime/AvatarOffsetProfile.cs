using UnityEngine;

namespace Echo.Avatar
{
    [CreateAssetMenu(fileName = nameof(Echo) + "/" + nameof(AvatarOffsetProfile))]
    public sealed class AvatarOffsetProfile : ScriptableObject
    {
        public void GetOffset(string guid, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }
    }
}