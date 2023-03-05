using System;

namespace Echo
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public readonly string Name;

        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
}