using Sirenix.OdinInspector.Editor;
using UnityEditor.Experimental.GraphView;

namespace Lucifer.DataDriven.Editor
{
    sealed class HeadPortView : Port
    {
        public HeadPortView(InspectorProperty property) :
            base(Orientation.Horizontal, Direction.Input, Capacity.Single, property.Info.TypeOfValue)
        {
        }
    }
}