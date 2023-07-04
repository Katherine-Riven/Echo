namespace Echo
{
    public abstract class GameStage
    {
        protected internal abstract void OnEnter();
        protected internal abstract void OnExit();
        protected internal abstract void OnDispose();
    }
}