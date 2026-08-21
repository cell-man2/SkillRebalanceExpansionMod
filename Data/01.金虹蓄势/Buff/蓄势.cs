using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "蓄势")]
    public static class 蓄势
    {
        public static List<BuffData> Data =
        [
            new BuffData
            {
                key = "蓄势",
                realId = 38,
                descr = "每受到一点伤害移除一层，回合开始时移除所有，造成层数点金剑系伤害。",
                seidData = new()
                {
                    {
                        25,
                        new()
                        {
                            { "value1", 1 },
                            { "value2", 8 }
                        }
                    },
                    {
                        111,
                        new()
                        {
                            { "value1", new List<string> {"@buff:蓄势"} }
                        }
                    }
                }
            },
        ];
    }
}