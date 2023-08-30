using UnityEngine;

namespace Echo
{
    public interface IGameEntityReference
    {
        GameObject InstantiateInstance(Vector3 position, Quaternion rotation);
        void       ReleaseInstance(GameObject  instance);
    }
}