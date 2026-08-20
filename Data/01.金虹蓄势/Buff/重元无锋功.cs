using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "重元无锋功")]
    public static class 重元无锋功
    {
        public static List<BuffData> Data =
        [
            new BuffData
            {
                key = "重元无锋功(人阶蓄势加成&展示)",
                realId = 137,
                descr = "释放【蓄势】技能消耗提高至五点金系灵气，并使获得的【蓄势】层数+（attack）"
            },
            new BuffData
            {
                key = "重元无锋功(天阶蓄势加成)",
                localId = 6,
                name = "重元无锋功（天阶蓄势加成）",
                descr = "释放【蓄势】技能将额外获得【蓄势】*（attack）",
                trigger = Trigger.使用技能时,
                removeTrigger = RemoveTrigger.不主动移除,
                buffIcon = 137,
                buffType = BuffType.功法被动,
                isHide = true,
                seidData = new()
                {
                    { 76, new() {{"value1", "@sId:蓄势_天阶"}} },
                    {
                        5,
                        new()
                        {
                            { "value1", new List<int> {38} },
                            { "value2", new List<int> {1} }
                        }
                    }
                },
            }
        ];
    }
}