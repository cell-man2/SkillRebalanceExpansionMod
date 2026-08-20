using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "紫莲金影罩")]
    public static class 紫莲金影罩
    {
        public static List<BuffData> Data =
        [
            new BuffData
            {
                key = "紫莲金影罩(获得护罩)",
                realId = 4256,
                descr = "本回合结束时，获得【护罩】*（attack）",
                trigger = Trigger.回合结束时,
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    {
                        5,
                        new()
                        {
                            { "value1", new List<int> {5} },
                            { "value2", new List<int> {1} }
                        }
                    }
                }
            },
            new BuffData
            {
                key = "紫莲金影罩(灵气检测)",
                localId = 5,
                name = "紫莲金影罩（灵气检测）",
                descr = "若灵气大于灵气上限，本回合结束时获得【护罩】*（attack）",
                trigger = Trigger.不主动触发,
                removeTrigger = RemoveTrigger.触发后移除所有,
                buffIcon = 4256,
                buffType = BuffType.指示物,
                isHide = true,
                seidData = new()
                {
                    { 161, new() {{"panduan", ">"}} },
                    {
                        5,
                        new()
                        {
                            { "value1", new List<string> {"@buff:紫莲金影罩(获得护罩)"} },
                            { "value2", new List<int> {1} }
                        }
                    }
                },
            },
        ];
    }
}