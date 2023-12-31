﻿namespace Echo.Control
{
    /// <summary>
    /// 带参状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStateWithArg<T>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        protected internal void OnEnter(in T arg);
    }

    /// <summary>
    /// 状态
    /// </summary>
    public abstract class State<TMachine, TState, TOwner>
        where TMachine : StateMachine<TMachine, TState, TOwner>
        where TState : State<TMachine, TState, TOwner>
    {
        /// <summary>
        /// 状态机
        /// </summary>
        public TMachine Machine { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public TOwner Owner => Machine.Owner;

        /// <summary>
        /// 当进入时
        /// </summary>
        protected internal abstract void OnEnter();

        /// <summary>
        /// 当更新时
        /// </summary>
        protected internal abstract void OnUpdate();

        /// <summary>
        /// 当退出时
        /// </summary>
        protected internal abstract void OnExit();
    }
}