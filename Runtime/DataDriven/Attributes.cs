using System;

namespace Echo.DataDriven
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public sealed class InspectNameAttribute : Attribute
    {
        public readonly string Name;

        public InspectNameAttribute(string name)
        {
            Name = name;
        }
    }
}