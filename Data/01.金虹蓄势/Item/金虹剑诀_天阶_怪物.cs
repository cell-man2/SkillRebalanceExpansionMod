using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "金虹剑诀_天阶_怪物")]
    public static class 金虹剑诀_天阶_怪物
    {
        public static List<ItemData> Data =
        [
            new ItemData
            {
                localId = 2,
                key = "金虹剑诀_天阶_怪物",
                name = "金虹剑诀",
                skillKey = "@bookInfo:金虹剑诀_天阶_怪物",
                shopType = ShopType.不投放,
                price = 1,
                itemFlag =
                [
                    ItemFlag.功法秘籍,
                    ItemFlag.天阶功法,
                    ItemFlag.天阶功法杂学
                ],
                desc2 = "金虹剑派秘传天阶战斗类功法，修炼者须修以金虹之法，则可蓄灵气为剑势，剑出则锐不可当。",
                wuDao =
                [
                    (DaoType.剑, DaoLevel.融会贯通)
                ],
                seidData = new()
                {
                    {
                        2,
                        new()
                        {
                            { "value1", "@ssId:金虹剑诀_天阶_怪物" }
                        }
                    }
                },
            },
        ];
    }
}