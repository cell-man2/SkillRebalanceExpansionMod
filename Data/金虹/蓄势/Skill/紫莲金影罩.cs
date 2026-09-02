using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Skill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.Skill
{
    [DataBase(DataCategory.Skill, "金虹蓄势", "紫莲金影罩")]
    public static class 紫莲金影罩
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];
        private static readonly List<int> shieldVal = [14, 26, 28, 48, 94];
        private static readonly List<int> bonusShieldVal = [6, 9, 14, 24, 47];

        public static List<SkillData> Data =
        [
            new SkillData
            {
                key = "紫莲金影罩",
                realId = 16,
                skillLv = skillLv,
                descr = TierValue<string>.Create(
                    tier => $"获得【护罩】*{shieldVal[tier-1]}，吸收一点金系灵气，若此时灵气数" +
                        $"大于灵气上限，则本回合结束时额外获得【护罩】*{bonusShieldVal[tier-1]}。",
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(
                        $"获得【护罩】*{shieldVal[tier-1]}，吸收一点金系灵气，若此时灵气数" +
                        $"大于灵气上限，则本回合结束时额外获得【护罩】*{bonusShieldVal[tier-1]}。"
                    ),
                    skillLv
                ),
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new()
                    {
                        {
                            4,
                            new()
                            {
                                { "value1", new List<object> {5, "@buff:紫莲金影罩(灵气检测)"} },
                                { "value2", new List<int> { shieldVal[tier-1], bonusShieldVal[tier-1] } }
                            }
                        },
                        {
                            7,
                            new()
                            {
                                { "value1", new List<int> { 0 } },
                                { "value2", new List<int> { 1 } }
                            }
                        },
                        {
                            148,
                            new()
                            {
                                { "panduan", ">"},
                                { "target", 1},
                                { "value1", "@buff:紫莲金影罩(灵气检测)" },
                                { "value2", 0 }
                            }
                        },
                        {
                            3,
                            new()
                            {
                                { "value1", new List<string> { "@buff:紫莲金影罩(灵气检测)" } },
                                { "value2", new List<int> { 1 } }
                            }
                        },
                    },
                    skillLv
                )
            }
        ];
    }
}