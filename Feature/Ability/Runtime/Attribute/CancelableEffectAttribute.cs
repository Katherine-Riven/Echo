using System;

namespace Echo.Abilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class CancelableEffectAttribute : Attribute
    {
    }
}