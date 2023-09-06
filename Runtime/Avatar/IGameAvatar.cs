using UnityEngine;

namespace Echo
{
    public interface IGameAvatar
    {
        string     Name             { get; }
        GameObject GameObject       { get; }
        Vector3    Position         { get; }
        Quaternion Rotation         { get; }
        bool       HasBeenDestroyed { get; }

        void Destroy();
    }
}