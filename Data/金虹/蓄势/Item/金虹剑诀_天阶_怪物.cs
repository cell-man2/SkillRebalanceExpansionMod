using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "金虹剑诀_天阶_怪物")]
    public static class 金虹剑诀_天阶_怪物
    {
        public static readonly List<ItemData> Data =
        [
            new ItemData
            {
                key = "金虹剑诀_天阶_怪物",
                localId = 2,
                name = "金虹剑诀",
                seidData = new()
                {
                    [2] = new()
                    {
                        ["value1"] = "@ssId:金虹剑诀_天阶_怪物"
                    }
                },
                skillKey = "@bookInfo:金虹剑诀_天阶_怪物",
                desc2 = "金虹剑派秘传天阶战斗类功法，修炼者须修以金虹之法，则可蓄灵气为剑势，剑出则锐不可当。",
                itemFlag =
                [
                    ItemFlag.功法秘籍,
                    ItemFlag.天阶功法,
                    ItemFlag.天阶功法杂学
                ],
                price = 1,
                shopType = ShopType.不投放,
                wuDao =
                [
                    (DaoType.剑, DaoLevel.融会贯通)
                ],
            }
        ];
    }
}