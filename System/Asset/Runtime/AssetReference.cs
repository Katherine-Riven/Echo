using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Echo.Asset
{
    public class AssetReference<T> : AssetReferenceT<T>, IAssetReference<T> where T : Object
    {
        public AssetReference(string guid) : base(guid)
        {
        }
    }
}