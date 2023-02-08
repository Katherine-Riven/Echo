using UnityEngine;

namespace Lucifer
{
    /// <summary>
    /// 游戏组件
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// 当Entity初始化
        /// </summary>
        /// <param name="gameObject">unity游戏对象</param>
        protected internal abstract void OnInitialize(GameObject gameObject);
    }
}