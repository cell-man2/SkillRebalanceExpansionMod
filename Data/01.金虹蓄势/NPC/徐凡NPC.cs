using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.NPCLeiXing
{
    [DataBase(DataCategory.NPCLeiXing, "金虹蓄势", "固定NPC修改")]
    public static class 徐凡NPC
    {
        private static readonly List<int> level = [10, 11, 12, 13, 14, 15];
        public static List<NPCLeiXingData> Data = CreateData();

        private static List<NPCLeiXingData> CreateData()
        {
            List<NPCLeiXingData> result = [];

            NPCLeiXingData data = new()
            {
                realLiuPai = 14,
                level = level,

                skills = new TierValue<List<object>>
                {
                    [10] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [11] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [12] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [13] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [14] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [15] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                },

                staticSkills = new TierValue<List<object>>
                {
                    [11] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [12] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [13] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [14] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [15] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                },
            };

            result.Add(data);
            return result;
        }
    }
}