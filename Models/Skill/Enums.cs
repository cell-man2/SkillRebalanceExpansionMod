namespace SkillRebalanceExpansionMod.Models.Skill
{
    // 神通请教类型，对应qingjiaotype
    public enum QingJiaoType
    {
        普通 = 1,
        门派 = 2,
        宁州不传 = 3,
        海外 = 4,
        海外不传 = 5,
        魔门 = 6,
        不可请教 = 7
    }

    // 神通类型，对应 Skill_Type
    public enum SkillType
    {
        阵法 = 1,
        抽牌 = 2,
        buff = 3,
        伤害 = 4,
        防御 = 5,
        填充 = 6,
        废弃 = 20
    }

    // 神通攻击类型，对应 AttackType
    public enum AttackType
    {
        金 = 0,
        木 = 1,
        水 = 2,
        火 = 3,
        土 = 4,
        气 = 5,
        神 = 6,
        剑 = 7,
        阵法 = 8,
        禁制 = 9,
        隐匿 = 10,
        魔 = 11,
        结丹 = 12,
        秘术 = 13,
        碎丹秘术 = 14,
        化婴秘术 = 15,
        结婴运气秘术 = 16,
        筑基秘术 = 17,
        化神秘术 = 18,
        渡劫秘术 = 19,
        淬体秘术 = 20
    }

    // 技能执行脚本，对应 script
    public enum Script
    {
        对敌人,
        对自己
    }

    // 技能释放方式，对应 Skill_DisplayType
    public enum SkillDisplayType
    {
        目标身上 = 0,
        我到目标 = 1
    }

    // 神通灵气消耗类型，对应 skill_CastType
    public enum CardType
    {
        金 = 0,
        木 = 1,
        水 = 2,
        火 = 3,
        土 = 4,
        魔 = 5,
        同 = 999
    }

    // 神通阶级，对应Skill_LV
    public enum SkillJie
    {
        人 = 1,
        地 = 2,
        天 = 3
    }

    // 神通品级，对应typePinJie
    public enum SkillPin
    {
        下 = 1,
        中 = 2,
        上 = 3
    }

    // 图鉴类型，对应TuJianType
    public enum TuJianType
    {
        无 = 0,
        神通 = 6,
        秘术 = 8,
        仙术 = 9
    }
}
