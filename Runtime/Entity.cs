using UnityEngine;

namespace Echo
{
    /// <summary>
    /// 游戏对象创建订单
    /// </summary>
    public interface IEntityOrder
    {
        /// <summary>
        /// Unity预制体
        /// </summary>
        /// <returns></returns>
        GameObject GetPrefab();

        /// <summary>
        /// 创建该对象的组件集合
        /// </summary>
        Component[] CreateComponents();
    }

    /// <summary>
    /// 游戏对象
    /// </summary>
    public sealed class Entity
    {
        internal Entity()
        {
        }

        internal GameObject  m_GameObject;
        internal Component[] m_Components;
        internal bool        m_HasBeenDestroyed;

        /// <summary>
        /// 释放已被销毁
        /// </summary>
        public bool HasBeenDestroyed => m_HasBeenDestroyed || m_GameObject == null;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position
        {
            get => m_GameObject.transform.position;
            set => m_GameObject.transform.position = value;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion Rotation
        {
            get => m_GameObject.transform.rotation;
            set => m_GameObject.transform.rotation = value;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="component">组件实例</param>
        /// <typeparam name="TComponent">目标组件类型</typeparam>
        /// <returns>返回是否找到组件</returns>
        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : Component
        {
            for (int i = 0, length = m_Components.Length; i < length; i++)
            {
                component = m_Components[i] as TComponent;
                if (component != null)
                {
                    return true;
                }
            }

            component = null;
            return false;
        }
    }
}