using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Echo.Asset
{
    internal sealed class AssetLoadHandle<T> : IAssetLoadHandle<T> where T : Object
    {
        internal AssetLoadHandle(AsyncOperationHandle<T> handle)
        {
            Handle = handle;
        }

        internal AsyncOperationHandle<T> Handle;

        private AssetLoadCompletedEvent<T> m_OnCompleted;

        public event AssetLoadCompletedEvent<T> OnCompleted
        {
            add
            {
                m_OnCompleted      += value;
                Handle.Completed -= HandleOnCompleted;
                Handle.Completed += HandleOnCompleted;
            }
            remove { m_OnCompleted -= value; }
        }

        public bool    IsDone      => Handle.IsDone;
        public bool    IsSuccess   => Handle.Status == AsyncOperationStatus.Succeeded;
        public float   LoadPercent => Handle.PercentComplete;
        public T       Result      => Handle.Result;
        public Task<T> Task        => Handle.Task;

        public T WaitForCompletion()
        {
            return Handle.WaitForCompletion();
        }

        private void HandleOnCompleted(AsyncOperationHandle<T> handle)
        {
            m_OnCompleted?.Invoke(handle.Result);
        }

        bool IEnumerator.MoveNext()
        {
            return !Handle.IsDone;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Handle.Result;
    }
}