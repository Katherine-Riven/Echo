using System;
using System.Collections.Generic;

namespace Echo.Control
{
    /// <summary>
    /// 状态机
    /// </summary>
    public abstract class StateMachine<TMachine, TState, TOwner> : IController
        where TMachine : StateMachine<TMachine, TState, TOwner>
        where TState : State<TMachine, TState, TOwner>, new()
    {
        private static readonly List<TState> s_StatePool = new List<TState>();

        protected StateMachine(TOwner owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 持有者
        /// </summary>
        public TOwner Owner { get; }

        private TState m_CurrentState;

        /// <summary>
        /// 默认状态
        /// </summary>
        protected abstract TState DefaultState { get; }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState<T>() where T : TState
        {
            m_CurrentState.OnExit();
            m_CurrentState.Machine = null;
            s_StatePool.Add(m_CurrentState);

            m_CurrentState = null;
            for (int i = 0; i < s_StatePool.Count; i++)
            {
                if (s_StatePool[i].GetType() == typeof(T))
                {
                    m_CurrentState = s_StatePool[i];
                    s_StatePool.RemoveAt(i);
                    break;
                }
            }

            m_CurrentState         ??= new TState();
            m_CurrentState.Machine =   (TMachine) this;
            m_CurrentState.OnEnter();
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState<T, TArg>(in TArg arg) where T : TState, IStateWithArg<TArg>
        {
            m_CurrentState.OnExit();
            m_CurrentState.Machine = null;
            s_StatePool.Add(m_CurrentState);

            m_CurrentState = null;
            for (int i = 0; i < s_StatePool.Count; i++)
            {
                if (s_StatePool[i].GetType() == typeof(T))
                {
                    m_CurrentState = s_StatePool[i];
                    s_StatePool.RemoveAt(i);
                    break;
                }
            }

            m_CurrentState         ??= new TState();
            m_CurrentState.Machine =   (TMachine) this;
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IStateWithArg<TArg>) m_CurrentState).OnEnter(arg);
        }

        void IController.OnEnable()
        {
            m_CurrentState         = DefaultState ?? throw new NullReferenceException();
            m_CurrentState.Machine = (TMachine) this;
            m_CurrentState.OnEnter();
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