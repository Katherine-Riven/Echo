using UnityEngine;

namespace Echo
{
    /// <summary>
    /// 资源引用
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    public interface IAssetReference<T> where T : Object
    {
    }
}