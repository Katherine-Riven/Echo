namespace Echo.Control
{
    /// <summary>
    /// 可控制的
    /// </summary>
    public interface IControllable
    {
    }

    /// <summary>
    /// 可被命令控制的
    /// </summary>
    public interface IControllable<T> : IControllable where T : struct, ICommand
    {
        void Execute(in T command);
    }
}