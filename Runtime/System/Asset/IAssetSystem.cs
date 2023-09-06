using Echo.Asset;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Echo
{
    public interface IAssetSystem : IGameSystem
    {
        IAssetHandle         LoadAsset(IAssetKey         key);
        IAssetHandle<T>      LoadAsset<T>(IAssetKey      key) where T : Object;
        IAsyncAssetHandle    LoadAssetAsync(IAssetKey    key);
        IAsyncAssetHandle<T> LoadAssetAsync<T>(IAssetKey key) where T : Object;
        void                 ReleaseAsset(IAssetHandle   handle);

        IAssetHandle<Scene>      LoadScene(IAssetKey              key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);
        IAsyncAssetHandle<Scene> LoadSceneAsync(IAssetKey         key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);
        void                     ReleaseScene(IAssetHandle<Scene> handle);

        IAssetHandle<GameObject>      Instantiate(IAssetKey                    key, Transform parent = null, bool       instantiateInWorldSpace    = false);
        IAssetHandle<GameObject>      Instantiate(IAssetKey                    key, Vector3   position,      Quaternion rotation, Transform parent = null);
        IAsyncAssetHandle<GameObject> InstantiateAsync(IAssetKey               key, Transform parent = null, bool       instantiateInWorldSpace    = false);
        IAsyncAssetHandle<GameObject> InstantiateAsync(IAssetKey               key, Vector3   position,      Quaternion rotation, Transform parent = null);
        void                          ReleaseInstance(IAssetHandle<GameObject> handle);
    }
}