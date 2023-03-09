using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Echo.Asset
{
    internal sealed class AssetSystem : GameSystem, IAssetSystem
    {
        public float iud;

        #region Override

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }

        #endregion

        public IAssetLoadHandle<T> LoadAsync<T>(IAssetReference<T> reference) where T : Object
        {
            return new AssetLoadHandle<T>(Addressables.LoadAssetAsync<T>(reference));
        }

        public void Release(Object asset)
        {
            Addressables.Release(asset);
        }

        public void Release<T>(IAssetLoadHandle<T> handle) where T : Object
        {
            Addressables.Release(((AssetLoadHandle<T>) handle).Handle);
        }
    }
}