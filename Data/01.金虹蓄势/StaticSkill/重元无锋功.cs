using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data
{
    [DataBase(DataCategory.StaticSkill, "金虹蓄势", "重元无锋功")]
    public static class 重元无锋功
    {
        private static readonly List<int> skillLv = [1,2,3,4,5];
        private static readonly List<int> buffNum = [14,22,39,65,130];
        private static readonly List<int> castTime = [1,38,75,150,300];

        public static List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "重元无锋功",
                realId = 18,
                skillLv = skillLv,
                descr = TierValue<string>.Create(
                    tier => $"释放【蓄势】技能消耗提高至五点金系灵气，并使获得的【蓄势】层数+{buffNum[tier-1]}",
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(
                        $"释放【蓄势】技能消耗提高至五点金系灵气，并使获得的【蓄势】层数+{buffNum[tier-1]}"
                    ),
                    skillLv
                ),
                skillJie = SkillJie.地,
                skillPin = SkillPin.上,
                skillCastTime = TierValue<int>.Create(tier => castTime[tier-1], skillLv),
                skillSpeed = TierValue<int>.Create(tier => 288 * tier, skillLv),
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new Dictionary<object, Dictionary<string, object>>
                    {
                        {
                            1,
                            new()
                            {
                                { "target", 1 },
                                {
                                    "value1",
                                    new List<object> {
                                        "@buff:重元无锋功(人阶蓄势加成&展示)",
                                        138,
                                        "@buff:重元无锋功(天阶蓄势加成)"
                                    }
                                },
                                {
                                    "value2",
                                    new List<int> {
                                        buffNum[tier-1],
                                        1,
                                        buffNum[tier-1]
                                    }
                                }
                            }
                        }
                    },
                    skillLv
                )
            }
        ];
    }
}