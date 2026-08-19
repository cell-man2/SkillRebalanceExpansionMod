using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;

namespace SkillRebalanceExpansionMod.Data
{
    [DataBase(DataCategory.NPCLeiXing, "金虹蓄势", "金虹蓄势")]
    public static class 金虹蓄势
    {
        public static List<NPCLeiXingData> Data =
        [
            new NPCLeiXingData
            {
                key = "金虹蓄势",
                realLiuPai = 13,
                level = [8],
                staticSkills = TierValue<List<object>>.Create(
                    tier =>
                    [
                        "@staticSkill:金虹剑诀_天阶5"
                    ],
                    [8]
                )
            }
        ];
    }
}