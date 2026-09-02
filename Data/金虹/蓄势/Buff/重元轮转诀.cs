using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "重元轮转诀")]
    public static class 重元轮转诀
    {
        public static List<BuffData> Data =
        [
            new BuffData
            {
                key="重元轮转诀(获得蓄势护罩&展示)",
                realId = 166,
                trigger = Trigger.抽牌阶段结束时,
                descr = "每回合吸收灵气后，获得【蓄势】*（attack），【护罩】*（attack）。" +
                    "每次使用5点或以上灵气的技能，都会获得【蓄势】和【护罩】并使此效果提升。"
            },
            new BuffData
            {
                key="重元轮转诀(使用技能)",
                realId = 167,
                seidData = new()
                {
                    {
                        81, new() {{"value1", 5}}
                    },
                    {
                        5,
                        new()
                        {
                            {
                                "value1",
                                new List<object> {"@buff:重元轮转诀(获得蓄势护罩&展示)", 5, 38}
                            },
                            { "value2", new List<int> {1, 1, 1} }
                        }
                    }
                }
            },
        ];
    }
}