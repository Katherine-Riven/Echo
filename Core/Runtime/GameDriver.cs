using UnityEngine;

namespace Echo
{
    /// <summary>
    /// Entity具备的游戏驱动接口
    /// </summary>
    public interface IGameDrive
    {
    }

    /// <summary>
    /// 游戏驱动
    /// </summary>
    public abstract class GameDriver : ScriptableObject
    {
        /// <summary>
        /// 当游戏初始化
        /// </summary>
        protected internal abstract void OnInitialize();

        /// <summary>
        /// 当有对象激活
        /// </summary>
        protected internal abstract void OnEntityEnable(GameEntity entity);

        /// <summary>
        /// 当有对象禁用
        /// </summary>
        protected internal abstract void OnEntityDisable(GameEntity entity);

        /// <summary>
        /// 当游戏Update
        /// </summary>
        protected internal abstract void OnUpdate(GameEntityCollection entities);

        /// <summary>
        /// 当游戏FixedUpdate
        /// </summary>
        protected internal abstract void OnFixedUpdate(GameEntityCollection entities);

        /// <summary>
        /// 当游戏LateUpdate
        /// </summary>
        protected internal abstract void OnLateUpdate(GameEntityCollection entities);

        /// <summary>
        /// 当游戏终止
        /// </summary>
        protected internal abstract void OnDispose();
    }
}