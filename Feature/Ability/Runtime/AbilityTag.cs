using System;
using System.Reflection;
using UnityEngine;

namespace Echo.Abilities
{
    [Serializable]
    public struct AbilityTag : IEquatable<AbilityTag>
    {
        public static readonly AbilityTag None = new AbilityTag(0);
        public static readonly AbilityTag All  = new AbilityTag(~0);

        [SerializeField]
        private int m_Value;
#if UNITY_EDITOR
        [SerializeField]
        private string m_Name;
#endif

        public AbilityTag(AbilityTag other)
        {
            m_Value = other.m_Value;
#if UNITY_EDITOR
            m_Name = other.m_Name;
#endif
        }

        public AbilityTag(int value)
        {
            m_Value = value;
#if UNITY_EDITOR
            m_Name = String.Empty;
#endif
        }

        public readonly bool HasAll(AbilityTag other)
        {
            return (m_Value & other.m_Value) == other.m_Value;
        }

        public readonly bool HasAny(AbilityTag other)
        {
            return (m_Value & other.m_Value) != 0;
        }

        public readonly bool Equals(AbilityTag other)
        {
            return m_Value == other.m_Value;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is AbilityTag other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return m_Value;
        }

        public static AbilityTag operator |(AbilityTag a, AbilityTag b)
        {
            return new AbilityTag(a.m_Value | b.m_Value);
        }

        public static AbilityTag operator &(AbilityTag a, AbilityTag b)
        {
            return new AbilityTag(a.m_Value & b.m_Value);
        }

        public static AbilityTag operator ~(AbilityTag a)
        {
            return new AbilityTag(~a.m_Value);
        }

        public static bool operator ==(AbilityTag a, AbilityTag b)
        {
            return a.m_Value == b.m_Value;
        }

        public static bool operator !=(AbilityTag a, AbilityTag b)
        {
            return a.m_Value != b.m_Value;
        }
    }

    public abstract class AbilityTagContainer : IEquatable<AbilityTagContainer>, IEquatable<AbilityTag>
    {
        private readonly AbilityTag m_Value;

        protected AbilityTagContainer()
        {
            AbilityTag tag = new AbilityTag(0);
            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.FieldType == typeof(AbilityTag))
                {
                    tag |= (AbilityTag) field.GetValue(this);
                }
                else if (field.FieldType.IsSubclassOf(typeof(AbilityTagContainer)))
                {
                    tag |= (AbilityTagContainer) field.GetValue(this);
                }
            }

            m_Value = tag;
        }

        public bool HasAll(AbilityTag other)
        {
            return m_Value.HasAll(other);
        }

        public bool HasAny(AbilityTag other)
        {
            return m_Value.HasAny(other);
        }

        public bool Equals(AbilityTagContainer other)
        {
            if (ReferenceEquals(other, null)) return false;
            return ReferenceEquals(this, other) || m_Value == other.m_Value;
        }

        public bool Equals(AbilityTag other)
        {
            return m_Value == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            switch (obj)
            {
                case AbilityTagContainer container:
                    return Equals(container);
                case AbilityTag tag:
                    return m_Value.Equals(tag);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        public static AbilityTag operator |(AbilityTagContainer a, AbilityTag b)
        {
            return a.m_Value | b;
        }

        public static AbilityTag operator |(AbilityTag a, AbilityTagContainer b)
        {
            return a | b.m_Value;
        }

        public static AbilityTag operator |(AbilityTagContainer a, AbilityTagContainer b)
        {
            return a.m_Value | b.m_Value;
        }

        public static AbilityTag operator &(AbilityTagContainer a, AbilityTag b)
        {
            return a.m_Value & b;
        }

        public static AbilityTag operator &(AbilityTag a, AbilityTagContainer b)
        {
            return a & b.m_Value;
        }

        public static AbilityTag operator &(AbilityTagContainer a, AbilityTagContainer b)
        {
            return a.m_Value & b.m_Value;
        }

        public static AbilityTag operator ~(AbilityTagContainer a)
        {
            return ~a.m_Value;
        }

        public static bool operator ==(AbilityTagContainer a, AbilityTag b)
        {
            return a != null && a.m_Value == b;
        }

        public static bool operator ==(AbilityTag a, AbilityTagContainer b)
        {
            return b != null && a == b.m_Value;
        }

        public static bool operator ==(AbilityTagContainer a, AbilityTagContainer b)
        {
            if (a == null)
            {
                return b == null;
            }

            if (b == null)
            {
                return false;
            }

            return a.m_Value == b.m_Value;
        }

        public static bool operator !=(AbilityTagContainer a, AbilityTag b)
        {
            return a == null || a.m_Value != b;
        }

        public static bool operator !=(AbilityTag a, AbilityTagContainer b)
        {
            return b == null || a != b.m_Value;
        }

        public static bool operator !=(AbilityTagContainer a, AbilityTagContainer b)
        {
            if (a == null)
            {
                return b != null;
            }

            if (b == null)
            {
                return true;
            }

            return a.m_Value != b.m_Value;
        }

        public static implicit operator AbilityTag(AbilityTagContainer a)
        {
            return a == null ? new AbilityTag() : a.m_Value;
        }
    }
}