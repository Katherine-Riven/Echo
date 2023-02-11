using System.Collections.Generic;
using UnityEngine;

namespace Lucifer
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public abstract class Manager : ScriptableObject
    {
        /// <summary>
        /// 当游戏初始化
        /// </summary>
        protected internal abstract void OnInitialize();

        /// <summary>
        /// 当游戏Update
        /// </summary>
        protected internal abstract void OnUpdate(IReadOnlyList<Entity> entities);

        /// <summary>
        /// 当游戏FixedUpdate
        /// </summary>
        protected internal abstract void OnFixedUpdate(IReadOnlyList<Entity> entities);

        /// <summary>
        /// 当游戏LateUpdate
        /// </summary>
        protected internal abstract void OnLateUpdate(IReadOnlyList<Entity> entities);

        /// <summary>
        /// 当游戏终止
        /// </summary>
        protected internal abstract void OnDispose();
    }
}