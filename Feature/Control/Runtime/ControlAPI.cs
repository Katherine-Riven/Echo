namespace Echo.Control
{
    public static class ControlAPI
    {
        /// <summary>
        /// 启用
        /// </summary>
        public static void Enable(this IController controller)
        {
            if (ControlDriven.s_Controllers.Contains(controller)) return;
            ControlDriven.s_Controllers.Add(controller);
            controller.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public static void Disable(this IController controller)
        {
            if (ControlDriven.s_Controllers.Remove(controller))
            {
                controller.OnDisable();
            }
        }
    }
}