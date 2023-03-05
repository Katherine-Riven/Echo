using UnityEngine;

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

        public IAssetLoadHandle<T> LoadAsync<T>(IAssetReference<T> reference) where T : Object
        {
            return null;
        }

        public void Release(Object asset)
        {
        }

        public void Release<T>(IAssetLoadHandle<T> handle) where T : Object
        {
        }
    }
}