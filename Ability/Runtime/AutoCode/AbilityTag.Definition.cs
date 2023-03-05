using UnityEngine;

namespace Echo.Abilities
{
    public partial struct AbilityTag
    {
        [InspectorName("神恩")]
        public static readonly AbilityTagPack.GraceOfGod GraceOfGod = new (0b00000000000000000000000000000011);
        [InspectorName("天赋")]
        public static readonly AbilityTagPack.Talent Talent = new (0b00000000000000000011111111111100);
        [InspectorName("装备")]
        public static readonly AbilityTag Equipment = new (0b00000000000000000100000000000000);
        [InspectorName("技能")]
        public static readonly AbilityTag Skill = new (0b00000000000000001000000000000000);
    }
    public partial class AbilityTagPack
    {
        public class GraceOfGod : AbilityTagPack
        {
            public GraceOfGod(int tag) : base(tag) { }
            [InspectorName("美德")]
            public readonly AbilityTag Blessing = new (0b00000000000000000000000000000001);
            [InspectorName("七宗罪")]
            public readonly AbilityTag Curse = new (0b00000000000000000000000000000010);
        }
        public class Talent : AbilityTagPack
        {
            public Talent(int tag) : base(tag) { }
            [InspectorName("人类")]
            public readonly Human Human = new (0b00000000000000000000000001111100);
            [InspectorName("血统")]
            public readonly Bloodline Bloodline = new (0b00000000000000000011111110000000);
        }
        public class Human : AbilityTagPack
        {
            public Human(int tag) : base(tag) { }
            [InspectorName("骑士")]
            public readonly AbilityTag Knight = new (0b00000000000000000000000000000100);
            [InspectorName("牧师")]
            public readonly AbilityTag Pastor = new (0b00000000000000000000000000001000);
            [InspectorName("巫师")]
            public readonly AbilityTag Wizard = new (0b00000000000000000000000000010000);
            [InspectorName("猎人")]
            public readonly AbilityTag Archer = new (0b00000000000000000000000000100000);
            [InspectorName("刺客")]
            public readonly AbilityTag Assassin = new (0b00000000000000000000000001000000);
        }
        public class Bloodline : AbilityTagPack
        {
            public Bloodline(int tag) : base(tag) { }
            [InspectorName("兽人")]
            public readonly AbilityTag Orc = new (0b00000000000000000000000010000000);
            [InspectorName("元素")]
            public readonly AbilityTag Element = new (0b00000000000000000000000100000000);
            [InspectorName("不死族")]
            public readonly AbilityTag Undead = new (0b00000000000000000000001000000000);
            [InspectorName("恶魔")]
            public readonly AbilityTag Demon = new (0b00000000000000000000010000000000);
            [InspectorName("木乃伊")]
            public readonly AbilityTag Mummy = new (0b00000000000000000000100000000000);
            [InspectorName("冰怪")]
            public readonly AbilityTag Snowman = new (0b00000000000000000001000000000000);
            [InspectorName("堕落人类")]
            public readonly AbilityTag FallenHuman = new (0b00000000000000000010000000000000);
        }
    }
}
