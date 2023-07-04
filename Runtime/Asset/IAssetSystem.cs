using UnityEngine;
using UnityEngine.SceneManagement;

namespace Echo.Asset
{
    public interface IAssetSystem : IGameSystem
    {
        Object                   LoadAsset(IAssetKey                key);
        T                        LoadAsset<T>(IAssetKey             key) where T : Object;
        IAsyncOperationHandle    LoadAssetAsync(IAssetKey           key);
        IAsyncOperationHandle<T> LoadAssetAsync<T>(IAssetKey        key) where T : Object;
        void                     ReleaseAsset(IAsyncOperationHandle handle);

        Scene                        LoadScene(IAssetKey                       key, LoadSceneMode loadMode = LoadSceneMode.Single);
        IAsyncOperationHandle<Scene> LoadSceneAsync(IAssetKey                  key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);
        void                         ReleaseScene(IAsyncOperationHandle<Scene> handle);

        GameObject                        Instantiate(IAssetKey                 key, Transform parent = null, bool       instantiateInWorldSpace    = false);
        GameObject                        Instantiate(IAssetKey                 key, Vector3   position,      Quaternion rotation, Transform parent = null);
        IAsyncOperationHandle<GameObject> InstantiateAsync(IAssetKey            key, Transform parent = null, bool       instantiateInWorldSpace    = false);
        IAsyncOperationHandle<GameObject> InstantiateAsync(IAssetKey            key, Vector3   position,      Quaternion rotation, Transform parent = null);
        bool                              ReleaseInstance(GameObject            instance);
        bool                              ReleaseInstance(IAsyncOperationHandle handle);
    }
}