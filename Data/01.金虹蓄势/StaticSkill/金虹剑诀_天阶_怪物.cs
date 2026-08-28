using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;


namespace SkillRebalanceExpansionMod.Data.StaticSkill
{
    [DataBase(DataCategory.StaticSkill, "金虹蓄势", "金虹剑诀_天阶_怪物")]
    public static class 金虹剑诀_天阶_怪物
    {
        private static readonly List<int> skillLv = [5];

        public static List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "金虹剑诀_天阶_怪物",
                realId = 994,
                skillLv = skillLv,
                descr = "每使用或消散一点金系灵气，获得【蓄势】*10；每回合造成的第一次剑系伤害提升30%。",
                affix = AffixProcessor.ExtractAffix(
                    "每使用或消散一点金系灵气，获得【蓄势】*10；每回合造成的第一次剑系伤害提升30%。"
                ),
                tuJianDescr = AffixProcessor.FormatTuJian(
                    "每使用或消散一点金系灵气，获得【蓄势】*10；每回合造成的第一次剑系伤害提升30%。"
                ),
                skillSpeed = 4320,
                skillPin = SkillPin.上,
                skillJie = SkillJie.天
            }
        ];
    }
}