using System;

namespace Echo.Asset
{
    public interface IAssetKey : IEquatable<IAssetKey>
    {
        string AssetKey { get; }

        bool IsValid();
    }
}