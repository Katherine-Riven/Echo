using System.Threading.Tasks;
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
        T LoadAsset<T>(IAssetReference<T> reference) where T : Object;

        /// <summary>
        /// 异步加载资源
        /// </summary>
        Task<T> LoadAssetAsync<T>(IAssetReference<T> reference) where T : Object;

        /// <summary>
        /// 释放资源
        /// </summary>
        void ReleaseAsset(Object asset);

        /// <summary>
        /// 实例化
        /// </summary>
        GameObject Instantiate(IAssetReference<GameObject> reference, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <summary>
        /// 实例化
        /// </summary>
        GameObject Instantiate(IAssetReference<GameObject> reference, Transform parent = null, bool instantiateInWorldSpace = false);

        /// <summary>
        /// 异步实例化
        /// </summary>
        Task<GameObject> InstantiateAsync(IAssetReference<GameObject> reference, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <summary>
        /// 异步实例化
        /// </summary>
        Task<GameObject> InstantiateAsync(IAssetReference<GameObject> reference, Transform parent = null, bool instantiateInWorldSpace = false);

        /// <summary>
        /// 释放实例
        /// </summary>
        void ReleaseInstance(GameObject instance);
    }
}