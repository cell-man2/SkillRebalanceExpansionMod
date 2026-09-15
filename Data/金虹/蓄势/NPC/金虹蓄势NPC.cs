using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.NPCLeiXing
{
    [DataBase(DataCategory.NPCLeiXing, "金虹蓄势", "常规NPC修改")]
    public static class 金虹蓄势NPC
    {
        private static readonly List<int> level = [9, 10, 11, 12, 13, 14, 15];
        public static List<NPCLeiXingData> Data = CreateData();

        private static List<NPCLeiXingData> CreateData()
        {
            List<NPCLeiXingData> result = [];

            NPCLeiXingData data = new()
            {
                realLiuPai = (int)LiuPai.金虹蓄势,
                level = level,

                skills = new TierValue<List<object>>
                {
                    [10] = ["@sId:蓄势_天阶", 16, 6, 28, 10, 31, 501, 504, 519],
                    [11] = ["@sId:蓄势_天阶", 16, 6, 28, 10, 31, 501, 504, 519],
                    [12] = ["@sId:蓄势_天阶", 16, 6, 28, 10, 31, 501, 504, 519],
                    [13] = ["@sId:蓄势_天阶", 16, 6, 28, 10, 31, 501, 504, 519],
                    [14] = ["@sId:蓄势_天阶", 16, 6, 28, 10, 31, 501, 504, 519],
                    [15] = ["@sId:蓄势_天阶", 16, 6, 28, 10, 31, 501, 504, 519],
                },

                staticSkills = new TierValue<List<object>>
                {
                    [9] = [33, 64, 112, 89, 19],
                    [11] = ["@staticSkill:重元轮转诀3", 65, 5245, 90, 34],
                    [12] = ["@staticSkill:重元轮转诀4", 65, 5245, 90, 34],
                    [13] = ["@staticSkill:重元轮转诀5", 65, 5245, 90, 35],
                    [14] = ["@staticSkill:重元轮转诀5", 65, 5245, 90, 35],
                    [15] = ["@staticSkill:重元轮转诀5", 65, 5245, 90, 35],
                },

                yuanYing = new TierValue<object>
                {
                    [11] = "@staticSkill:金虹剑诀_天阶3",
                    [12] = "@staticSkill:金虹剑诀_天阶4",
                    [13] = "@staticSkill:金虹剑诀_天阶5",
                    [14] = "@staticSkill:金虹剑诀_天阶5",
                    [15] = "@staticSkill:金虹剑诀_天阶5",
                },

                huaShenLingYu = new TierValue<HuaShenLingYu>
                {
                    [13] = HuaShenLingYu.以金入道,
                    [14] = HuaShenLingYu.以金入道,
                    [15] = HuaShenLingYu.以金入道,
                },
            };

            result.Add(data);
            return result;
        }
    }
}