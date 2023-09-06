using UnityEngine;

namespace Echo
{
    public interface IGameAvatarReference
    {
        GameObject InstantiateInstance(Vector3 position, Quaternion rotation);
        void       ReleaseInstance(GameObject  instance);
    }
}