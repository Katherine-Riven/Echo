using UnityEngine;

namespace Echo.Avatar
{
    [CreateAssetMenu(menuName = nameof(Echo) + "/" + nameof(AvatarProfile))]
    public sealed class AvatarProfile : ScriptableObject
    {
        public void GetOffset(string guid, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }
    }
}