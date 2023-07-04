namespace Echo
{
    public interface IGameSystem
    {
        protected internal void OnInitialize();
        protected internal void OnStart();
        protected internal void OnDispose();

        protected internal void OnUpdate();
        protected internal void OnFixedUpdate();
        protected internal void OnLateUpdate();
    }
}