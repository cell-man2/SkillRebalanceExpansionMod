using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.Skill;
using System.IO;

namespace SkillRebalanceExpansionMod.Data.Skill
{
    [DataBase(DataCategory.Skill, "金虹蓄势", "紫莲金影罩")]
    public static class 紫莲金影罩
    {
        private static readonly List<int> skillLv = [1,2,3,4,5];
        private static readonly List<int> shieldValue = [20, 28, 46, 85, 171];

        public static List<SkillData> Data =
        [
            new SkillData
            {
                key = "紫莲金影罩",
                realId = 16,
                skillLv = skillLv,
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    lv => new Dictionary<object, Dictionary<string, object>>
                    {
                        [4] = new Dictionary<string, object>
                        {
                            ["value1"] = new List<int> {5},
                            ["value2"] = new List<int> {shieldValue[lv - 1]}
                        },
                        [7] = new Dictionary<string, object>
                        {
                            ["value1"] = new List<int> {0},
                            ["value2"] = new List<int> {1}
                        }
                    },
                    skillLv
                ),
                descr = TierValue<string>.Create(
                    lv => $"获得【护罩】*{shieldValue[lv - 1]}，并吸收一点金系灵气",
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    lv => AffixProcessor.FormatTuJian(
                        $"获得【护罩】*{shieldValue[lv - 1]}，并吸收一点金系灵气"
                    ),
                    skillLv
                ),
                affix2 = AffixProcessor.ExtractAffix("获得【护罩】*1，并吸收一点金系灵气")
            }
        ];
    }
}
