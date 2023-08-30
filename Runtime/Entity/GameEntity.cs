using UnityEngine;

namespace Echo
{
    public interface IGameEntity
    {
        string Name     { get; }
        bool   IsActive { get; }
        void   Active(Vector3 position, Quaternion rotation);
        void   Destroy();
    }

    public abstract class GameEntity : IGameEntity
    {
        protected GameEntity(string name, IGameEntityOrder order)
        {
            Name        = name;
            IsActive    = false;
            m_Reference = order.Reference;
        }

        private readonly IGameEntityReference m_Reference;

        private GameObject m_Instance;

        public string Name { get; }

        public bool IsActive { get; private set; }

        public void Active(Vector3 position, Quaternion rotation)
        {
            IsActive        = true;
            m_Instance      = m_Reference.InstantiateInstance(position, rotation);
            m_Instance.name = Name;
            OnActive(m_Instance);
        }

        public void Destroy()
        {
            IsActive = false;
            OnDestroy();
            m_Reference.ReleaseInstance(m_Instance);
            m_Instance = null;
        }

        protected abstract void OnActive(GameObject instance);
        protected abstract void OnDestroy();
    }
}