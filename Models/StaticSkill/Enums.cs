namespace SkillRebalanceExpansionMod.Models.StaticSkill
{
    // 功法请教类型，对应qingjiaotype
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

    // 功法属性，对应AttackType
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

    // 功法类型，无对应
    public enum SkillStyle
    {
        战斗 = 1,
        中庸 = 2,
        修炼 = 3
    }

    // 功法阶级，对应Skill_LV
    public enum SkillJie
    {
        人 = 1,
        地 = 2,
        天 = 3
    }

    // 功法品级，对应typePinJie
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
        功法 = 7,
    }
}