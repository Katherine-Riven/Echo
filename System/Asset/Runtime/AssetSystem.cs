using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Echo.Asset
{
    internal sealed class AssetSystem : GameSystem, IAssetSystem
    {
        #region Override

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }

        #endregion

        public T LoadAsset<T>(IAssetReference<T> reference) where T : Object
        {
            return Addressables.LoadAssetAsync<T>(reference).WaitForCompletion();
        }

        public Task<T> LoadAssetAsync<T>(IAssetReference<T> reference) where T : Object
        {
            return Addressables.LoadAssetAsync<T>(reference).Task;
        }

        public void ReleaseAsset(Object asset)
        {
            Addressables.Release(asset);
        }

        public GameObject Instantiate(IAssetReference<GameObject> reference, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Addressables.InstantiateAsync(reference, position, rotation, parent).WaitForCompletion();
        }

        public GameObject Instantiate(IAssetReference<GameObject> reference, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Addressables.InstantiateAsync(reference, parent, instantiateInWorldSpace).WaitForCompletion();
        }

        public Task<GameObject> InstantiateAsync(IAssetReference<GameObject> reference, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Addressables.InstantiateAsync(reference, position, rotation, parent).Task;
        }

        public Task<GameObject> InstantiateAsync(IAssetReference<GameObject> reference, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Addressables.InstantiateAsync(reference, parent, instantiateInWorldSpace).Task;
        }

        public void ReleaseInstance(GameObject instance)
        {
            Addressables.Release(instance);
        }
    }
}