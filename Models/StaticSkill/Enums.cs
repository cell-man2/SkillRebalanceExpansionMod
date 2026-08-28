namespace SkillRebalanceExpansionMod.Models.StaticSkill
{
    /// <summary>
    /// 功法请教类型，对应 JSON 字段 qingjiaotype。
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
    /// 功法属性，对应 JSON 字段 AttackType。
    /// </summary>
    public enum AttackType
    {
        金 = 0,
        木 = 1,
        水 = 2,
        火 = 3,
        土 = 4,
        气 = 5,
        遁术 = 6,
        神 = 7,
        剑 = 8,
        体 = 9
    }

    /// <summary>
    /// 功法类型标识（战斗/中庸/修炼），用于自动计算修炼速度，不直接写入 JSON。
    /// </summary>
    public enum SkillStyle
    {
        战斗 = 1,
        中庸 = 2,
        修炼 = 3
    }

    /// <summary>
    /// 功法阶级（人/地/天），对应 JSON 字段 Skill_LV。
    /// </summary>
    public enum SkillJie
    {
        人 = 1,
        地 = 2,
        天 = 3
    }

    /// <summary>
    /// 功法品级（下/中/上），对应 JSON 字段 typePinJie。
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
        功法 = 7,
    }
}