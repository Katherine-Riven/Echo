using System;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Echo.Asset
{
    [Serializable]
    public class AssetReference<T> : AssetReferenceT<T>, IAssetReference<T> where T : Object
    {
        public AssetReference(string guid) : base(guid)
        {
        }
    }
}