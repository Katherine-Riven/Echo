using UnityEngine;
using UnityEngine.EventSystems;

namespace Echo.UI
{
    public interface IUISystem : IGameSystem
    {
        float ViewExpireTime { get; set; }

        Camera      GetCamera();
        Canvas      GetCanvas();
        EventSystem GetEventSystem();
        void        EnableInteraction();
        void        DisableInteraction();

        void  PopTip(string    text);
        IView Show<TView>(IArgs args = null) where TView : class, IView, new();
        void  Hide(IView       view);

        void ReleaseAllUnusedView();
    }
}