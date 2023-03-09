namespace Echo.Control
{
    public static class ControlExtension
    {
        /// <summary>
        /// 启用
        /// </summary>
        public static void Enable<T>(this IController<T> controller, T target) where T : IControllable
        {
            if (ControlDriver.s_Controllers.Contains(controller)) return;
            ControlDriver.s_Controllers.Add(controller);
            controller.Target = target;
            controller.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public static void Disable<T>(this IController<T> controller) where T : IControllable
        {
            if (ControlDriver.s_Controllers.Remove(controller))
            {
                controller.OnDisable();
                controller.Target = default;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public static void Execute<T, TCommand>(this IController<T> controller, in TCommand command) where T : IControllable where TCommand : struct, ICommand
        {
            if (controller.Target is IControllable<TCommand> controllable)
            {
                controllable.Execute(in command);
            }
        }
    }
}