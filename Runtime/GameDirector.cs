using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [AddComponentMenu("Lucifer/GameDirector")]
    public sealed class GameDirector : MonoBehaviour
    {
        #region Fields

        private const  int          DefaultEntityCapacity = 256;
        private static GameDirector s_Instance;

        [SerializeField] private Stage     m_LaunchStage = null;
        [SerializeField] private Manager[] m_Managers    = new Manager[0];

        [NonSerialized] private Stage        m_CurrentStage;
        [NonSerialized] private List<Entity> m_Entities = new List<Entity>(DefaultEntityCapacity);

        #endregion

        #region API

        /// <summary>
        /// 切换游戏阶段
        /// </summary>
        /// <param name="stage">目标阶段</param>
        /// <exception cref="ArgumentNullException">stage can't be null</exception>
        public static void GotoStage(Stage stage)
        {
            if (stage == null)
            {
                throw new ArgumentNullException(nameof(stage));
            }

            if (s_Instance.m_CurrentStage != null)
            {
                s_Instance.m_CurrentStage.OnExit();
            }

            s_Instance.m_CurrentStage = stage;
            s_Instance.m_CurrentStage.OnEnter();
        }

        /// <summary>
        /// 获取系统实例
        /// </summary>
        /// <typeparam name="T">目标系统类型</typeparam>
        /// <returns>返回系统实例</returns>
        /// <exception cref="ArgumentException">目标系统必须已创建</exception>
        public static T GetManager<T>() where T : Manager
        {
            for (int i = 0, length = s_Instance.m_Managers.Length; i < length; i++)
            {
                if (s_Instance.m_Managers[i] is T result)
                {
                    return result;
                }
            }

            throw new ArgumentException($"{typeof(T).Name} is not exists, please setup it on GameManager.");
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="order">对象订单</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        public static void CreateEntity(IEntityOrder order, Vector3 position, Quaternion rotation)
        {
            GameObject  gameObject = Instantiate(order.GetPrefab(), position, rotation);
            Component[] components = order.CreateComponents();
            Entity entity = new Entity()
            {
                m_GameObject       = gameObject,
                m_Components       = components,
                m_HasBeenDestroyed = false,
            };

            for (int i = 0; i < components.Length; i++)
            {
                components[i].OnInitialize(gameObject);
            }

            s_Instance.m_Entities.Add(entity);
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="entity">对象实例</param>
        public static void DestroyEntity(Entity entity)
        {
            if (entity.m_GameObject != null)
            {
                Destroy(entity.m_GameObject);
            }

            entity.m_GameObject       = null;
            entity.m_Components       = Array.Empty<Component>();
            entity.m_HasBeenDestroyed = true;
        }

        #endregion

        #region Unity Life

        private void Awake()
        {
            if (s_Instance != null)
            {
                DestroyImmediate(gameObject);
                throw new Exception("There is more than one GameManager, make sure only one.");
            }

            s_Instance = this;
            for (int i = 0, length = m_Managers.Length; i < length; i++)
            {
                m_Managers[i].OnInitialize();
            }
        }

        private void Start()
        {
            GotoStage(m_LaunchStage);
        }

        private void Update()
        {
            for (int i = 0, length = m_Managers.Length; i < length; i++)
            {
                m_Managers[i].OnUpdate(m_Entities);
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0, length = m_Managers.Length; i < length; i++)
            {
                m_Managers[i].OnFixedUpdate(m_Entities);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0, length = m_Managers.Length; i < length; i++)
            {
                m_Managers[i].OnLateUpdate(m_Entities);
            }

            for (int i = m_Entities.Count - 1; i >= 0; i--)
            {
                if (m_Entities[i].HasBeenDestroyed)
                {
                    m_Entities.RemoveAt(i);
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0, length = m_Managers.Length; i < length; i++)
            {
                m_Managers[i].OnDispose();
            }
        }

        #endregion
    }
}