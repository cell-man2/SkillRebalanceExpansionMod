using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Skill;

namespace SkillRebalanceExpansionMod.Data.Skill
{
    [DataBase(DataCategory.Skill, "金虹剑诀", "蓄势_天阶")]
    public static class 蓄势_天阶
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];

        public static List<SkillData> Data =
        [
            new SkillData
            {
                key = "蓄势_天阶",
                realId = 2217,
                skillLv = skillLv,
                skillJie = SkillJie.天,
                skillPin = SkillPin.上,
                qingJiaoType = QingJiaoType.宁州不传,
                tuJianType = TuJianType.神通
            }
        ];
    }
}