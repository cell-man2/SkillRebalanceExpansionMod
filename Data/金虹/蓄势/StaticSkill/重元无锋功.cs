using System;
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.StaticSkill
{
    [DataBase(DataCategory.StaticSkill, "金虹蓄势", "重元无锋功")]
    public static class 重元无锋功
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];
        private static readonly List<int> buffNum = [14, 22, 39, 65, 130];
        private static readonly List<int> buffNum1 = [8, 14, 23, 39, 78];
        private static readonly List<int> buffNum2 = [3, 4, 8, 13, 26];
        private static readonly List<int> castTime = [1, 38, 75, 150, 300];
        private static readonly Func<int, string> descrFunc = tier =>
            $"释放【蓄势】技能消耗提高至五点金系灵气并额外获得【蓄势】*{buffNum1[tier - 1]}，" +
            $"每因此增加一点灵气消耗则额外获得【蓄势】*{buffNum2[tier - 1]}。";

        public static readonly List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "重元无锋功",
                realId = 18,
                skillLv = skillLv,
                descr = TierValue<string>.Create(descrFunc, skillLv),
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new Dictionary<object, Dictionary<string, object>>
                    {
                        [1] = new()
                        {
                            ["target"] = 1,
                            ["value1"] = new List<object>
                            {
                                "@buff:重元无锋功(天阶蓄势加成&展示)",
                                "@buff:重元无锋功(人阶蓄势加成)",
                                138
                            },
                            ["value2"] = new List<object>
                            {
                                buffNum1[tier - 1],
                                buffNum[tier - 1],
                                1
                            }
                        }
                    },
                    skillLv
                ),
                skillCastTime = TierValue<int>.Create(tier => castTime[tier - 1], skillLv),
                skillJie = SkillJie.地,
                skillPin = SkillPin.上,
                skillSpeed = TierValue<int>.Create(tier => 288 * tier, skillLv),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(descrFunc(tier)),
                    skillLv
                ),
            }
        ];
    }
}