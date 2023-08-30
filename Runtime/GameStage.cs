namespace Echo
{
    public abstract class GameStage
    {
        protected internal abstract void OnEnter();
        protected internal abstract void OnUpdate();
        protected internal abstract void OnLateUpdate();
        protected internal abstract void OnFixedUpdate();
        protected internal abstract void OnExit();
    }
}