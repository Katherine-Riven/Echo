using UnityEngine;

namespace Echo
{
    /// <summary>
    /// 带参数的游戏阶段
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public interface IGameStageWithArg<T>
    {
        void Enter(in T arg);
    }

    /// <summary>
    /// 游戏阶段
    /// </summary>
    public abstract class GameStage : ScriptableObject
    {
        /// <summary>
        /// 当进入阶段
        /// </summary>
        protected internal abstract void OnEnter();

        /// <summary>
        /// 当退出阶段
        /// </summary>
        protected internal abstract void OnExit();
    }
}