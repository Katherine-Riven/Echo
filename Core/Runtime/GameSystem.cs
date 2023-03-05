using UnityEngine;

namespace Echo
{
    public interface IGameSystem
    {
    }

    /// <summary>
    /// 游戏系统
    /// </summary>
    public abstract class GameSystem : ScriptableObject
    {
        /// <summary>
        /// 当游戏初始化
        /// </summary>
        protected internal abstract void OnInitialize();

        /// <summary>
        /// 当游戏终止
        /// </summary>
        protected internal abstract void OnDispose();
    }
}