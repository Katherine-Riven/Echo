using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Echo
{
    [DefaultExecutionOrder(-1)]
    [AddComponentMenu(nameof(Echo) + "/" + nameof(GameManager))]
    public sealed class GameManager : MonoBehaviour
    {
        private static GameManager s_Instance;

        #region Systems

        [NonSerialized]
        private IGameSystem[] m_Systems;

        [SerializeReference]
        private IUISystem m_UISystem;

        public static IUISystem UISystem => s_Instance.m_UISystem;

        [SerializeReference]
        private IAssetSystem m_AssetSystem;

        public static IAssetSystem AssetSystem => s_Instance.m_AssetSystem;

        #endregion

        #region Stage

        [NonSerialized]
        private GameStage m_CurrentStage;

        public void GoToStage<TStage>() where TStage : GameStage, new()
        {
            m_CurrentStage?.OnExit();
            m_CurrentStage = new TStage();
            m_CurrentStage.OnEnter();
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
            name       = nameof(GameManager);
            DontDestroyOnLoad(gameObject);

            m_Systems = typeof(GameManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => typeof(IGameSystem).IsAssignableFrom(x.FieldType))
                .Select(x => (IGameSystem) x.GetValue(this))
                .ToArray();

            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnInitialize();
            }
        }

        private void Start()
        {
            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnStart();
            }
        }

        private void Update()
        {
            m_CurrentStage?.OnUpdate();
        }

        private void LateUpdate()
        {
            m_CurrentStage?.OnLateUpdate();
        }

        private void FixedUpdate()
        {
            m_CurrentStage?.OnFixedUpdate();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_Systems.Length; i++)
            {
                m_Systems[i].OnDispose();
            }
        }

        #endregion
    }
}