using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "重元无锋功")]
    public static class 重元无锋功
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "重元无锋功(人阶蓄势加成)",
                realId = 137,
                descr = "释放【蓄势】技能消耗提高至五点金系灵气，并使获得的【蓄势】层数+（attack）",
                isHide = true
            },
            new BuffData
            {
                key = "重元无锋功(天阶蓄势加成&展示)",
                localId = 6,
                buffType = BuffType.功法被动,
                descr = "释放【蓄势】技能消耗提高至五点金系灵气并额外获得【蓄势】*（attack），" +
                    "每因此增加一点灵气消耗则额外获得【蓄势】",
                name = "重元无锋功（天阶蓄势加成）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [76] = new()
                    {
                        ["value1"] = "@sId:蓄势_天阶"
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> { 38 },
                        ["value2"] = new List<object> { 1 }
                    }
                },
                trigger = Trigger.使用技能时,
                buffIcon = 137,
                showOnlyOne = true,
            }
        ];
    }
}