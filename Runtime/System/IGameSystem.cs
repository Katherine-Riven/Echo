namespace Echo
{
    public interface IGameSystem
    {
        protected internal void OnInitialize();
        protected internal void OnStart();
        protected internal void OnDispose();
        protected internal void OnStartGame();
        protected internal void OnStopGame();
    }
}