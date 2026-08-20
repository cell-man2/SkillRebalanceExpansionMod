using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data
{
    [DataBase(DataCategory.StaticSkill, "金虹蓄势", "重元轮转诀")]
    public static class 重元轮转诀
    {
        private static readonly List<int> skillLv = [1,2,3,4,5];
        private static readonly List<int> shieldVal = [5,8,16,40,120];
        private static readonly List<int> growthVal = [1,2,5,12,36];

        public static List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "重元轮转诀",
                realId = 23,
                skillLv = skillLv,
                descr = TierValue<string>.Create(
                    tier => $"每回合吸收灵气后，获得【蓄势】*{shieldVal[tier-1]}，【护罩】*{shieldVal[tier-1]}。" +
                        $"每次使用5点或以上灵气的技能，都会使获得的层数额外+{growthVal[tier-1]}并获得等量的【蓄势】和【护罩】。",
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(
                        $"每回合吸收灵气后，获得【蓄势】*{shieldVal[tier-1]}，【护罩】*{shieldVal[tier-1]}。" +
                        $"每次使用5点或以上灵气的技能，都会使获得的层数额外+{growthVal[tier-1]}并获得等量的【蓄势】和【护罩】。"
                    ),
                    skillLv
                ),
            }
        ];
    }
}