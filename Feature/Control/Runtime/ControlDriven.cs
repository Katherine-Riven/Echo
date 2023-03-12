using System.Collections.Generic;

namespace Echo.Control
{
    internal sealed class ControlDriven : GameFeatureDriven
    {
        internal static readonly List<IController> s_Controllers = new List<IController>();
        internal static readonly List<IController> s_Execution   = new List<IController>();

        protected override void OnInitialize()
        {
        }

        protected override void OnEntityEnable(GameEntity entity)
        {
        }

        protected override void OnEntityDisable(GameEntity entity)
        {
        }

        protected override void OnUpdate(GameEntityCollection entities)
        {
            s_Execution.Clear();
            s_Execution.AddRange(s_Controllers);
            foreach (IController controller in s_Execution)
            {
                controller.OnUpdate();
            }
        }

        protected override void OnFixedUpdate(GameEntityCollection entities)
        {
        }

        protected override void OnLateUpdate(GameEntityCollection entities)
        {
        }

        protected override void OnDispose()
        {
        }
    }
}