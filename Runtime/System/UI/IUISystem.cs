using Echo.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Echo
{
    public interface IUISystem : IGameSystem
    {
        Camera      GetCamera();
        Canvas      GetCanvas();
        EventSystem GetEventSystem();
        void        EnableInteraction();
        void        DisableInteraction();

        IView Show<TView>(IArgs args = null) where TView : class, IView, new();
        void  Hide(IView        view);

        void ReleaseAllUnusedView();
    }
}