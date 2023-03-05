using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力标签
    /// </summary>
    [Serializable]
    public partial struct AbilityTag : IEquatable<AbilityTag>
    {
        #region MyRegion

        /// <summary>
        /// 空标签
        /// </summary>
        public static readonly AbilityTag None = new AbilityTag(0);

        /// <summary>
        /// 所有标签
        /// </summary>
        public static readonly AbilityTag All = new AbilityTag(~0);

        #endregion

        #region Parse

        static AbilityTag()
        {
            List<KeyValuePair<string, AbilityTag>> tagMap = new List<KeyValuePair<string, AbilityTag>>();
            foreach (FieldInfo field in typeof(AbilityTag).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name == nameof(None) || field.Name == nameof(All))
                {
                    continue;
                }

                MapTag(null, field);
            }

            s_Map = tagMap.ToArray();

            void MapTag(object target, FieldInfo field)
            {
                if (field.FieldType == typeof(AbilityTag))
                {
                    tagMap.Add(new KeyValuePair<string, AbilityTag>(field.Name, (AbilityTag) field.GetValue(target)));
                }
                else if (field.FieldType.IsSubclassOf(typeof(AbilityTagPack)))
                {
                    foreach (FieldInfo child in field.FieldType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        MapTag(field.GetValue(target), child);
                    }
                }
            }
        }

        private static readonly KeyValuePair<string, AbilityTag>[] s_Map;

        private static bool TryGetTag(ReadOnlySpan<char> key, out AbilityTag tag)
        {
            for (int i = 0; i < s_Map.Length; i++)
            {
                ReadOnlySpan<char> temp = s_Map[i].Key.AsSpan();
                if (key.Equals(temp, StringComparison.Ordinal))
                {
                    tag = s_Map[i].Value;
                    return true;
                }
            }

            tag = None;
            return false;
        }

        public static AbilityTag Parse(string value)
        {
            AbilityTag         result    = None;
            ReadOnlySpan<char> valueSpan = value.AsSpan();
            for (int splitIndex = valueSpan.IndexOf(','); splitIndex >= 0; valueSpan = valueSpan.Slice(splitIndex + 1), splitIndex = valueSpan.IndexOf(','))
            {
                if (TryGetTag(valueSpan.Slice(0, splitIndex), out AbilityTag tag))
                {
                    result |= tag;
                }
            }

            if (TryGetTag(valueSpan, out AbilityTag lastTag))
            {
                result |= lastTag;
            }

            return result;
        }

        #endregion

        #region Data

        public AbilityTag(int tagValue)
        {
            m_TagValue = tagValue;
        }

        [SerializeField] private int m_TagValue;

        #endregion

        #region API

        /// <summary>
        /// 是否完整包含另一标签
        /// </summary>
        public readonly bool HasAllTag(in AbilityTag other)
        {
            return (m_TagValue & other.m_TagValue) == other.m_TagValue;
        }

        /// <summary>
        /// 是否包含另一标签中的任意一个
        /// </summary>
        public readonly bool HasAnyTag(in AbilityTag other)
        {
            if (other.m_TagValue == 0)
            {
                return true;
            }

            return (m_TagValue & other.m_TagValue) != 0;
        }

        public static implicit operator int(in AbilityTag tag)
        {
            return tag.m_TagValue;
        }

        public static implicit operator AbilityTag(in int value)
        {
            return new AbilityTag(value);
        }

        public static AbilityTag operator |(in AbilityTag a, in AbilityTag b)
        {
            return new AbilityTag()
            {
                m_TagValue = a.m_TagValue | b.m_TagValue,
            };
        }

        public static AbilityTag operator &(in AbilityTag a, in AbilityTag b)
        {
            return new AbilityTag()
            {
                m_TagValue = a.m_TagValue & b.m_TagValue,
            };
        }

        public static bool operator ==(in AbilityTag a, in AbilityTag b)
        {
            return a.m_TagValue == b.m_TagValue;
        }

        public static bool operator !=(in AbilityTag a, in AbilityTag b)
        {
            return a.m_TagValue != b.m_TagValue;
        }

        #endregion

        #region Override

        public readonly bool Equals(AbilityTag other)
        {
            return m_TagValue == other.m_TagValue;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is AbilityTag other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return m_TagValue;
        }

        public readonly override string ToString()
        {
            if (m_TagValue == None.m_TagValue)
            {
                return nameof(None);
            }

            string str   = String.Empty;
            bool   isAll = true;
            for (int i = 0; i < s_Map.Length; i++)
            {
                if (HasAllTag(s_Map[i].Value))
                {
                    str = string.IsNullOrEmpty(str) ? s_Map[i].Key : $"{str},{s_Map[i].Key}";
                }
                else
                {
                    isAll = false;
                }
            }

            return isAll ? nameof(All) : str;
        }

        #endregion
    }

    /// <summary>
    /// 标签集，可处理重复添加标签移除标签问题，同一标签最多重复添加255次
    /// </summary>
    public sealed class AbilityTagSet
    {
        private const int AbilityTagSize = 32; // AbilityTag 为32位

        private AbilityTag m_DefaultTag;
        private AbilityTag m_CacheTag;

        private readonly byte[] m_Flags = new byte[AbilityTagSize];

        /// <summary>
        /// 当前标签
        /// </summary>
        public AbilityTag Tag => m_DefaultTag | m_CacheTag;

        /// <summary>
        /// 重置标签
        /// </summary>
        public void Reset(AbilityTag defaultTag)
        {
            m_DefaultTag = defaultTag;
            Clear();
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        public void Add(in AbilityTag tag)
        {
            int flag = tag;
            if (flag == 0) return;
            m_CacheTag |= tag;
            for (int i = 0; i < AbilityTagSize; i++)
            {
                if (((1 << i) & flag) != 0)
                {
                    if (m_Flags[i] == byte.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    m_Flags[i]++;
                }
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public void Remove(in AbilityTag tag)
        {
            int flag = tag;
            if (flag == 0) return;
            for (int i = 0; i < AbilityTagSize; i++)
            {
                int temp = 1 << i;
                if ((temp & flag) != 0 && m_Flags[i] > 0)
                {
                    m_Flags[i]--;
                    if (m_Flags[i] == 0)
                    {
                        m_CacheTag &= ~temp;
                    }
                }
            }
        }

        /// <summary>
        /// 清空标签
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < AbilityTagSize; i++)
            {
                m_Flags[i] = 0;
            }
        }
    }

    /// <summary>
    /// 多个标签，用于定义标签
    /// </summary>
    public abstract partial class AbilityTagPack : IEquatable<AbilityTagPack>, IEquatable<AbilityTag>
    {
        private readonly AbilityTag m_Tag;

        protected AbilityTagPack(int tag) => m_Tag = new AbilityTag(tag);

        /// <summary>
        /// 是否完整包含另一标签
        /// </summary>
        public bool HasAllTag(in AbilityTag other)
        {
            return m_Tag.HasAllTag(other);
        }

        /// <summary>
        /// 是否包含另一标签中的任意一个
        /// </summary>
        public bool HasAnyTag(in AbilityTag other)
        {
            return m_Tag.HasAnyTag(other);
        }

        public static implicit operator AbilityTag([NotNull] in AbilityTagPack pack)
        {
            return pack.m_Tag;
        }

        public static AbilityTag operator |([NotNull] in AbilityTagPack a, in AbilityTag b)
        {
            return a.m_Tag | b;
        }

        public static AbilityTag operator |(in AbilityTag a, [NotNull] in AbilityTagPack b)
        {
            return a | b.m_Tag;
        }

        public static AbilityTag operator |([NotNull] in AbilityTagPack a, [NotNull] in AbilityTagPack b)
        {
            return a.m_Tag | b.m_Tag;
        }

        public static AbilityTag operator &([NotNull] in AbilityTagPack a, in AbilityTag b)
        {
            return a.m_Tag & b;
        }

        public static AbilityTag operator &(in AbilityTag a, [NotNull] in AbilityTagPack b)
        {
            return a & b.m_Tag;
        }

        public static AbilityTag operator &([NotNull] in AbilityTagPack a, [NotNull] in AbilityTagPack b)
        {
            return a.m_Tag & b.m_Tag;
        }

        public static bool operator ==([NotNull] in AbilityTagPack a, in AbilityTag b)
        {
            return a.m_Tag == b;
        }

        public static bool operator ==(in AbilityTag a, [NotNull] in AbilityTagPack b)
        {
            return a == b.m_Tag;
        }

        public static bool operator ==([NotNull] in AbilityTagPack a, [NotNull] in AbilityTagPack b)
        {
            return a.m_Tag == b.m_Tag;
        }

        public static bool operator !=([NotNull] in AbilityTagPack a, in AbilityTag b)
        {
            return a.m_Tag != b;
        }

        public static bool operator !=(in AbilityTag a, [NotNull] in AbilityTagPack b)
        {
            return a != b.m_Tag;
        }

        public static bool operator !=([NotNull] in AbilityTagPack a, [NotNull] in AbilityTagPack b)
        {
            return a.m_Tag != b.m_Tag;
        }

        public bool Equals(AbilityTag other)
        {
            return m_Tag.Equals(other);
        }

        public bool Equals(AbilityTagPack other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_Tag.Equals(other.m_Tag);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AbilityTagPack) obj);
        }

        public override int GetHashCode()
        {
            return m_Tag.GetHashCode();
        }
    }
}