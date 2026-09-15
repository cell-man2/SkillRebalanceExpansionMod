using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "金虹剑诀_天阶(伤害提升&展示)",
                localId = 1,
                buffType = BuffType.功法被动,
                descr = "每使用或消散一点金系灵气，获得【蓄势】*（2*(attack-5)/5）；每回合造成的第一次剑系伤害提升（attack）%。",
                name = "金虹剑诀",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [17] = new()
                    {
                        ["value1"] = "@buff:金虹剑诀_天阶(受伤提升)",
                        ["value2"] = 1
                    }
                },
                trigger = Trigger.回合开始时,
                buffIcon = 103,
                showOnlyOne = true,
            },
            new BuffData
            {
                key = "金虹剑诀_天阶(消散灵气)",
                localId = 2,
                buffType = BuffType.功法被动,
                descr = "每消散1点金系灵气，获得【蓄势】*（attack）。",
                name = "金虹剑诀（消散灵气）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [195] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = 0,
                        ["value3"] = 38,
                        ["value4"] = 1
                    }
                },
                trigger = Trigger.弃牌时,
                buffIcon = 103,
                isHide = true,
            },
            new BuffData
            {
                key = "金虹剑诀_天阶(使用灵气)",
                localId = 3,
                buffType = BuffType.功法被动,
                descr = "每使用1点金系灵气，获得【蓄势】*（attack）。",
                name = "金虹剑诀（使用灵气）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [39] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = 38,
                        ["value3"] = 1,
                        ["value4"] = 0
                    }
                },
                trigger = Trigger.使用卡牌时,
                buffIcon = 103,
                isHide = true,
            },
            new BuffData
            {
                key = "金虹剑诀_天阶(受伤提升)",
                localId = 4,
                buffType = BuffType.功法被动,
                descr = "下一次受到剑系伤害提升（attack）%",
                name = "受到剑系伤害提升",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [216] = new()
                    {
                        ["value1"] = 7
                    },
                    [192] = new()
                    {
                        ["value1"] = 0.01f,
                        ["value2"] = 7
                    },
                    [111] = new()
                    {
                        ["value1"] = new List<object> { "@buff:金虹剑诀_天阶(受伤提升)" }
                    }
                },
                trigger = Trigger.受到伤害时,
                buffIcon = 103,
                isHide = true,
            }
        ];
    }
}