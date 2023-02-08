using UnityEngine;

namespace Lucifer
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public abstract class Stage : ScriptableObject
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