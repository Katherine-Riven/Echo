using System;

namespace Echo
{
    [Serializable]
    public abstract class GameFeature
    {
        protected internal abstract void OnInitialize();
        protected internal abstract void OnStart();
        protected internal abstract void OnDispose();
        protected internal abstract void OnUpdate();
        protected internal abstract void OnFixedUpdate();
        protected internal abstract void OnLateUpdate();
    }
}