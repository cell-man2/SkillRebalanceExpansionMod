namespace SkillRebalanceExpansionMod.Models.Skill
{
    /// <summary>
    /// 神通请教类型，对应 JSON 字段 qingjiaotype。
    /// </summary>
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

    /// <summary>
    /// 神通类型（决定技能在 AI 中的释放优先级），对应 JSON 字段 Skill_Type。
    /// </summary>
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

    /// <summary>
    /// 攻击类型（决定技能伤害类型与克制关系），对应 JSON 字段 AttackType。
    /// </summary>
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

    /// <summary>
    /// 技能执行脚本，对应 JSON 字段 script（"SkillSelf" / "SkillAttack"）。
    /// </summary>
    public enum Script
    {
        对敌人,
        对自己
    }

    /// <summary>
    /// 技能释放方式，对应 JSON 字段 Skill_DisplayType。
    /// </summary>
    public enum SkillDisplayType
    {
        目标身上 = 0,
        我到目标 = 1
    }

    /// <summary>
    /// 灵气消耗类型，用于 cost 字段中的 type。
    /// 其中 同 = 999 表示同系灵气（任意非魔类型），写入 skill_SameCastNum。
    /// </summary>
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

    /// <summary>
    /// 神通阶级（人/地/天），对应 JSON 字段 Skill_LV。
    /// </summary>
    public enum SkillJie
    {
        人 = 1,
        地 = 2,
        天 = 3
    }

    /// <summary>
    /// 神通品级（下/中/上），对应 JSON 字段 typePinJie。
    /// </summary>
    public enum SkillPin
    {
        下 = 1,
        中 = 2,
        上 = 3
    }

    /// <summary>
    /// 图鉴类型，对应 JSON 字段 TuJianType。
    /// </summary>
    public enum TuJianType
    {
        无 = 0,
        神通 = 6,
        秘术 = 8,
        仙术 = 9
    }
}