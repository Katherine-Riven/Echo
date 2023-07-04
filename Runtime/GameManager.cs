using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Echo.Asset;
using Echo.UI;
using UnityEngine;

// ReSharper disable UseArrayEmptyMethod
// ReSharper disable ForCanBeConvertedToForeach

namespace Echo
{
    [DefaultExecutionOrder(-1)]
    [AddComponentMenu(nameof(Echo) + "/" + nameof(GameManager))]
    public sealed class GameManager : MonoBehaviour
    {
        #region Fields

        private const int DefaultEntityCapacity = 256;

        private static GameManager s_Instance;

        [SerializeReference]
        private IUISystem m_UISystem;

        [SerializeReference]
        private IAssetSystem m_AssetSystem;

        [SerializeReference]
        private GameFeature[] m_Features = new GameFeature[0];

        [NonSerialized]
        private IGameSystem[] m_Systems = new IGameSystem[0];

        [NonSerialized]
        private Queue<GameStage> m_StageQueue = new Queue<GameStage>();

        [NonSerialized]
        private List<GameEntity> m_ActiveEntities = new List<GameEntity>(DefaultEntityCapacity);

        #endregion

        #region API

        public static IUISystem UISystem => s_Instance.m_UISystem;

        public static IAssetSystem AssetSystem => s_Instance.m_AssetSystem;

        public static GameEntityQuery<GameEntity> GetActiveEntities() => new GameEntityQuery<GameEntity>(s_Instance.m_ActiveEntities);

        public static GameEntityQuery<T> QueryActiveEntities<T>() where T : class, IGameEntity => new GameEntityQuery<T>(s_Instance.m_ActiveEntities);

        public static void LaunchStage(GameStage launchStage)
        {
            if (s_Instance.m_StageQueue.Count > 0)
            {
                throw new NotSupportedException("Game has already launch, can't launch again.");
            }

            launchStage.OnEnter();
            s_Instance.m_StageQueue.Enqueue(launchStage);
        }

        public static void GoToStage(GameStage targetStage, params GameStage[] dependencies)
        {
            s_Instance.m_StageQueue.Peek().OnExit();
            for (int i = 0; i < dependencies.Length; i++)
            {
                s_Instance.m_StageQueue.Enqueue(dependencies[i]);
            }

            targetStage.OnEnter();
            s_Instance.m_StageQueue.Enqueue(targetStage);
        }

        public static void GoBackToPreviousStage()
        {
            if (s_Instance.m_StageQueue.Count == 1)
            {
                throw new NotSupportedException("Can't go back to previous stage, because current is last one stage.");
            }

            GameStage currentStage = s_Instance.m_StageQueue.Dequeue();
            currentStage.OnExit();
            currentStage.OnDispose();
            s_Instance.m_StageQueue.Peek().OnEnter();
        }

        #endregion

        #region Internal

        internal static bool RegisterEntity(GameEntity entity)
        {
            if (s_Instance.m_ActiveEntities.Contains(entity))
            {
                return false;
            }

            s_Instance.m_ActiveEntities.Add(entity);
            return true;
        }

        internal static bool UnRegisterEntity(GameEntity entity)
        {
            return s_Instance.m_ActiveEntities.Remove(entity);
        }

        internal static bool IsEntityActive(GameEntity entity)
        {
            return s_Instance.m_ActiveEntities.Contains(entity);
        }

        #endregion

        #region Unity Event

        private void Awake()
        {
            if (s_Instance != null)
            {
                DestroyImmediate(gameObject);
                throw new Exception("There is more than one GameManager, make sure only one.");
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            m_Systems = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                 .Where(x => typeof(IGameSystem).IsAssignableFrom(x.FieldType))
                                 .Select(x => (IGameSystem) x.GetValue(this))
                                 .ToArray();

            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnInitialize();
            }

            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnInitialize();
            }
        }

        private void Start()
        {
            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnStart();
            }

            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnStart();
            }
        }

        private void Update()
        {
            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnUpdate();
            }

            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnFixedUpdate();
            }

            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnFixedUpdate();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnLateUpdate();
            }

            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnLateUpdate();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnDispose();
            }

            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnDispose();
            }
        }

        #endregion
    }
}