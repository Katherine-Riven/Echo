using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public abstract class GameAvatar : IGameAvatar
    {
        private static List<IGameAvatar> s_Avatars = new List<IGameAvatar>();

        public static GameAvatarQuery<T> Query<T>() where T : IGameAvatar => new GameAvatarQuery<T>(s_Avatars);

        protected GameAvatar(string name, Vector3 position, Quaternion rotation, IGameAvatarOrder order)
        {
            m_Reference     = order.Reference;
            Name            = name;
            GameObject      = order.Reference.InstantiateInstance(position, rotation);
            GameObject.name = name;
        }

        private readonly IGameAvatarReference m_Reference;

        public string     Name             { get; }
        public GameObject GameObject       { get; private set; }
        public Vector3    Position         => GameObject.transform.position;
        public Quaternion Rotation         => GameObject.transform.rotation;
        public bool       HasBeenDestroyed => GameObject == null;

        public void Destroy()
        {
            if (GameObject == null)
            {
                return;
            }

            OnDestroy();
            m_Reference.ReleaseInstance(GameObject);
            GameObject = null;
        }

        protected abstract void OnDestroy();
    }
}