using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Echo
{
    
    /// <summary>
    /// 加载成功事件
    /// </summary>
    public delegate void AssetLoadCompletedEvent<T>(T asset) where T : Object;
    
    /// <summary>
    /// 资源加载操作权柄
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAssetLoadHandle<T> : IEnumerator where T : Object
    {
        /// <summary>
        /// 是否加载完毕
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// 是否加载成功
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// 当前加载百分比
        /// </summary>
        float LoadPercent { get; }

        /// <summary>
        /// 加载结果
        /// </summary>
        T Result { get; }

        /// <summary>
        /// 加载任务
        /// </summary>
        Task<T> Task { get; }

        /// <summary>
        /// 等待加载完毕
        /// </summary>
        T WaitForCompletion();
        
        /// <summary>
        /// 当加载成功
        /// </summary>
        event AssetLoadCompletedEvent<T> OnCompleted;
    }
}