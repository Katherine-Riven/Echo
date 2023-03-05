using UnityEngine;

namespace Echo
{
    /// <summary>
    /// 游戏对象创建订单
    /// </summary>
    public interface IGameEntityOrder
    {
        /// <summary>
        /// 实例化Unity GameObject
        /// </summary>
        GameObject Instantiate(Vector3 position, Quaternion rotation);

        /// <summary>
        /// 释放Unity GameObject
        /// </summary>
        void Release(GameObject instance);
    }

    /// <summary>
    /// 游戏对象
    /// </summary>
    public abstract class GameEntity
    {
        protected GameEntity(string name, Vector3 position, Quaternion rotation, IGameEntityOrder order)
        {
            Order              = order;
            m_GameObject       = order.Instantiate(position, rotation);
            m_GameObject.name  = name;
            m_HasBeenDestroyed = false;
            GameManager.InternalAddEntity(this);
        }

        internal readonly IGameEntityOrder Order;

        private GameObject m_GameObject;
        private bool       m_HasBeenDestroyed;

        /// <summary>
        /// 释放已被销毁
        /// </summary>
        public bool HasBeenDestroyed => m_HasBeenDestroyed;

        /// <summary>
        /// Unity GameObject
        /// </summary>
        public GameObject GameObject => m_GameObject;

        /// <summary>
        /// Unity Transform
        /// </summary>
        public Transform Transform => m_GameObject.transform;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position => m_GameObject.transform.position;

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion Rotation => m_GameObject.transform.rotation;

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy()
        {
            if (m_HasBeenDestroyed)
            {
                return;
            }

            m_HasBeenDestroyed = true;
            Order.Release(m_GameObject);
            GameManager.InternalRemoveEntity(this);
        }

        /// <summary>
        /// 当启用时
        /// </summary>
        protected internal virtual void OnEnable()
        {
        }

        /// <summary>
        /// 当禁用时
        /// </summary>
        protected internal virtual void OnDisable()
        {
        }
    }
}