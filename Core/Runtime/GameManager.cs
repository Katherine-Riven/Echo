using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [AddComponentMenu(nameof(Echo) + "/" + nameof(GameManager))]
    public sealed class GameManager : MonoBehaviour
    {
        #region Fields

        private const  int         DefaultEntityCapacity = 256;
        private static GameManager s_Instance;

        [SerializeField] private GameStage               m_LaunchStage = null;
        [SerializeField] private GameSystem[]            m_Systems     = new GameSystem[0];
        [SerializeField] private GameFeatureDriver[] m_Features    = new GameFeatureDriver[0];

        [NonSerialized] private GameStage        m_CurrentStage;
        [NonSerialized] private List<GameEntity> m_Entities         = new List<GameEntity>(DefaultEntityCapacity);
        [NonSerialized] private List<GameEntity> m_ToAddEntities    = new List<GameEntity>();
        [NonSerialized] private List<GameEntity> m_ToRemoveEntities = new List<GameEntity>();

        #endregion

        #region API

        /// <summary>
        /// 资源系统
        /// </summary>
        public static IAssetSystem AssetSystem { get; }

        /// <summary>
        /// UI系统
        /// </summary>
        public static IUISystem UISystem { get; }

        /// <summary>
        /// 切换游戏阶段
        /// </summary>
        /// <param name="stage">目标阶段</param>
        /// <exception cref="ArgumentNullException">stage can't be null</exception>
        public static void GotoStage(GameStage stage)
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
            stage.OnEnter();
        }

        /// <summary>
        /// 切换游戏阶段
        /// </summary>
        /// <param name="stageWithArg">目标阶段</param>
        /// <param name="arg">切换参数</param>
        /// <exception cref="ArgumentNullException">stage can't be null</exception>
        public static void GoToStage<T>(IGameStageWithArg<T> stageWithArg, in T arg)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            GameStage stage = stageWithArg as GameStage;
            if (stage == null)
            {
                throw new ArgumentNullException(nameof(stageWithArg));
            }

            if (s_Instance.m_CurrentStage != null)
            {
                s_Instance.m_CurrentStage.OnExit();
            }

            s_Instance.m_CurrentStage = stage;
            stageWithArg.Enter(arg);
        }

        #endregion

        #region Entity

        /// <summary>
        /// 添加对象
        /// </summary>
        internal static void InternalAddEntity(GameEntity entity)
        {
            s_Instance.m_ToAddEntities.Add(entity);
        }

        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="entity">对象实例</param>
        public static void InternalRemoveEntity(GameEntity entity)
        {
            s_Instance.m_ToRemoveEntities.Add(entity);
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
            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].OnInitialize();
            }

            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                m_Features[i].OnInitialize();
            }
        }

        private void Start()
        {
            GotoStage(m_LaunchStage);
        }

        private void Update()
        {
            GameEntityCollection collection = new GameEntityCollection(m_Entities);
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                m_Features[i].OnUpdate(collection);
            }
        }

        private void FixedUpdate()
        {
            GameEntityCollection collection = new GameEntityCollection(m_Entities);
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                m_Features[i].OnFixedUpdate(collection);
            }
        }

        private void LateUpdate()
        {
            GameEntityCollection collection = new GameEntityCollection(m_Entities);
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                m_Features[i].OnLateUpdate(collection);
            }

            foreach (GameEntity toAdd in m_ToAddEntities)
            {
                toAdd.OnEnable();
                for (int i = 0, length = m_Features.Length; i < length; i++)
                {
                    m_Features[i].OnEntityEnable(toAdd);
                }

                m_Entities.Add(toAdd);
            }

            m_ToAddEntities.Clear();
            foreach (GameEntity toRemove in m_ToRemoveEntities)
            {
                m_Entities.Remove(toRemove);
                toRemove.OnDisable();
                for (int i = 0, length = m_Features.Length; i < length; i++)
                {
                    m_Features[i].OnEntityDisable(toRemove);
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                m_Features[i].OnDispose();
            }

            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].OnDispose();
            }
        }

        #endregion
    }
}