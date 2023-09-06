using System;
using System.Threading.Tasks;

namespace Echo.Asset
{
    public interface IAssetHandle
    {
        object Asset { get; }
    }

    public interface IAssetHandle<T> : IAssetHandle
    {
        new T Asset { get; }
    }

    public enum AsyncAssetStatus
    {
        Loading,
        Succeeded,
        Failed,
    }

    public delegate void AsyncAssetCallback<T>(T handle) where T : IAsyncAssetHandle;

    public interface IAsyncAssetHandle : IAssetHandle, IEquatable<IAsyncAssetHandle>
    {
        AsyncAssetStatus Status          { get; }
        float            CompletePercent { get; }

        Task<object> Task { get; }

        event AsyncAssetCallback<IAsyncAssetHandle> OnCompleted;
    }

    public interface IAsyncAssetHandle<T> : IAssetHandle<T>, IAsyncAssetHandle
    {
        new Task<T> Task { get; }

        new event AsyncAssetCallback<IAsyncAssetHandle<T>> OnCompleted;
    }
}