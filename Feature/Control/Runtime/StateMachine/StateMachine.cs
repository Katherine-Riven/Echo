using System.Collections.Generic;

namespace Echo.Control
{
    /// <summary>
    /// 状态机
    /// </summary>
    public abstract class StateMachine<TMachine, TState, TOwner> : IController
        where TMachine : StateMachine<TMachine, TState, TOwner>
        where TState : State<TMachine, TState, TOwner>
    {
        #region State Pool

        private static readonly List<TState> s_StatePool = new List<TState>();

        /// <summary>
        /// 获取一个state
        /// </summary>
        private static T GetState<T>() where T : TState, new()
        {
            T state = null;
            for (int i = 0; i < s_StatePool.Count; i++)
            {
                if (s_StatePool[i].GetType() == typeof(T))
                {
                    state = (T) s_StatePool[i];
                    s_StatePool.RemoveAt(i);
                    break;
                }
            }

            state ??= new T();
            return state;
        }

        #endregion

        /// <summary>
        /// 拥有者
        /// </summary>
        public TOwner Owner { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        private TState m_CurrentState;

        /// <summary>
        /// 默认状态
        /// </summary>
        protected abstract void EnterDefaultState();

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>() where T : TState, new()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnExit();
                m_CurrentState.Machine = null;
                s_StatePool.Add(m_CurrentState);
            }

            m_CurrentState         = GetState<T>();
            m_CurrentState.Machine = (TMachine) this;
            m_CurrentState.OnEnter();
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="arg"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        public void ChangeState<T, TArg>(in TArg arg) where T : TState, IStateWithArg<TArg>, new()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnExit();
                m_CurrentState.Machine = null;
                s_StatePool.Add(m_CurrentState);
            }

            m_CurrentState         = GetState<T>();
            m_CurrentState.Machine = (TMachine) this;
            ((IStateWithArg<TArg>) m_CurrentState).OnEnter(in arg);
        }

        void IController.OnEnable()
        {
            EnterDefaultState();
        }

        void IController.OnUpdate()
        {
            m_CurrentState.OnUpdate();
        }

        void IController.OnDisable()
        {
            m_CurrentState.OnExit();
            m_CurrentState.Machine = null;
            s_StatePool.Add(m_CurrentState);
            m_CurrentState = null;
        }
    }
}