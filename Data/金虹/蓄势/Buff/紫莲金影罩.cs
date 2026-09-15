using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "紫莲金影罩")]
    public static class 紫莲金影罩
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "紫莲金影罩(获得护罩)",
                realId = 4256,
                descr = "本回合结束时，获得【护罩】*（attack）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> { 5 },
                        ["value2"] = new List<object> { 1 }
                    }
                },
                trigger = Trigger.回合结束时,
            },
            new BuffData
            {
                key = "紫莲金影罩(灵气检测)",
                localId = 5,
                buffType = BuffType.指示物,
                descr = "若灵气大于灵气上限，本回合结束时获得【护罩】*（attack）",
                name = "紫莲金影罩（灵气检测）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [161] = new()
                    {
                        ["panduan"] = ">"
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> { "@buff:紫莲金影罩(获得护罩)" },
                        ["value2"] = new List<object> { 1 }
                    }
                },
                trigger = Trigger.不主动触发,
                buffIcon = 4256,
                isHide = true,
            }
        ];
    }
}