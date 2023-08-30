using System;
using System.Threading.Tasks;

namespace Echo.Asset
{
    public enum AsyncOperationStatus
    {
        Loading,
        Succeeded,
        Failed,
    }

    public interface IAsyncOperationHandle : IEquatable<IAsyncOperationHandle>
    {
        AsyncOperationStatus Status          { get; }
        float                PercentComplete { get; }

        object       Result { get; }
        Task<object> Task   { get; }

        event Action<IAsyncOperationHandle> OnCompleted;
    }

    public interface IAsyncOperationHandle<T> : IAsyncOperationHandle
    {
        new T       Result { get; }
        new Task<T> Task   { get; }

        new event Action<IAsyncOperationHandle<T>> OnCompleted;
    }
}