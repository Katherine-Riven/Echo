using System;

namespace Echo.Abilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InvokeByContextAttribute : Attribute
    {
        public readonly Type Type;

        public InvokeByContextAttribute(Type type)
        {
            Type = type;
        }
    }
}