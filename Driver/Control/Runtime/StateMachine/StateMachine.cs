using System;
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
        public T GetState<T>() where T : TState, new()
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

            state         ??= new T();
            state.Machine =   (TMachine) this;
            return state;
        }

        /// <summary>
        /// 获取一个带参数的State
        /// </summary>
        public T GetState<T, TArg>(in TArg arg) where T : TState, IStateWithArg<TArg>, new()
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

            state         ??= new T();
            state.Machine =   (TMachine) this;
            state.SetUp(arg);
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
        protected abstract TState DefaultState { get; }

        void IController.OnEnable()
        {
            m_CurrentState         = DefaultState ?? throw new NullReferenceException("Default state is null, make sure it's not null.");
            m_CurrentState.Machine = (TMachine) this;
            m_CurrentState.OnEnter();
        }

        void IController.OnUpdate()
        {
            if (m_CurrentState.MoveNext(out TState nextState))
            {
                m_CurrentState.OnExit();
                m_CurrentState.Machine = null;
                s_StatePool.Add(m_CurrentState);
                m_CurrentState         = nextState ?? throw new NullReferenceException("Want to move next state, but next state is null. Make sure it's not null.");
                m_CurrentState.Machine = (TMachine) this;
                m_CurrentState.OnEnter();
            }

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