using UnityEngine;

namespace Echo
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public interface IAssetSystem : IGameSystem
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        IAssetLoadHandle<T> LoadAsync<T>(IAssetReference<T> reference) where T : Object;

        /// <summary>
        /// 释放资源
        /// </summary>
        void Release(Object asset);

        /// <summary>
        /// 释放资源
        /// </summary>
        void Release<T>(IAssetLoadHandle<T> handle) where T : Object;
    }
}