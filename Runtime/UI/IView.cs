using Echo.Asset;
using UnityEngine;

namespace Echo.UI
{
    public interface IView
    {
        IAssetKey     AssetKey      { get; }
        GameObject    GameObject    { get; }
        RectTransform RectTransform { get; }

        void HandleArgs(IArgs args);
        void OnUpdate();
    }
}