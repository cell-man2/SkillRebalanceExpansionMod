using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        public static List<BuffData> Data =
        [
            new BuffData
            {
                key="金虹剑诀_天阶(伤害提升&展示)",
                localId=1,
                buffIcon=103,
                buffType=BuffType.功法被动,
                name="金虹剑诀",
                descr="每使用或消散一点金系灵气，获得【蓄势】*（2*(attack-5)/5）；每回合造成的第一次剑系伤害提升（attack）%。",
                trigger=Trigger.回合开始时,
                removeTrigger=RemoveTrigger.不主动移除,
                showOnlyOne=true,
                seidData=new()
                {
                    {
                        17,
                        new()
                        {
                            { "value1", "@buff:金虹剑诀_天阶(受伤提升)" },
                            { "value2", 1 }
                        }
                    }
                }
            },
            new BuffData
            {
                key="金虹剑诀_天阶(消散灵气)",
                localId=2,
                buffIcon=103,
                buffType=BuffType.功法被动,
                name="金虹剑诀（消散灵气）",
                descr="每消散1点金系灵气，获得【蓄势】*（attack）。",
                trigger=Trigger.弃牌时,
                removeTrigger=RemoveTrigger.不主动移除,
                isHide=true,
                seidData=new()
                {
                    {
                        195,
                        new()
                        {
                            { "value1", 1 },
                            { "value2", 0 },
                            { "value3", 38 },
                            { "value4", 1 }
                        }
                    }
                }
            },
            new BuffData
            {
                key="金虹剑诀_天阶(使用灵气)",
                localId=3,
                buffIcon=103,
                buffType=BuffType.功法被动,
                name="金虹剑诀（使用灵气）",
                descr="每使用1点金系灵气，获得【蓄势】*（attack）。",
                trigger=Trigger.使用卡牌时,
                removeTrigger=RemoveTrigger.不主动移除,
                isHide=true,
                seidData=new()
                {
                    {
                        39,
                        new()
                        {
                            { "value1", 1 },
                            { "value2", 38 },
                            { "value3", 1 },
                            { "value4", 0 }
                        }
                    }
                }
            },
            new BuffData
            {
                key="金虹剑诀_天阶(受伤提升)",
                localId=4,
                buffIcon=103,
                buffType=BuffType.功法被动,
                name="受到剑系伤害提升",
                descr="下一次受到剑系伤害提升（attack）%",
                trigger=Trigger.受到伤害时,
                removeTrigger=RemoveTrigger.不主动移除,
                isHide=true,
                seidData=new()
                {
                    {
                        216,
                        new()
                        {
                            { "value1", 7 }
                        }
                    },
                    {
                        192,
                        new()
                        {
                            { "value1", 0.01f },
                            { "value2", 7 }
                        }
                    },
                    {
                        111,
                        new()
                        {
                            { "value1", new List<string> {"@buff:金虹剑诀_天阶(受伤提升)"} }
                        }
                    }
                }
            }
        ];
    }
}