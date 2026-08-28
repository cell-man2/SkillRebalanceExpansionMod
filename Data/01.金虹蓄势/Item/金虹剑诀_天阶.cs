using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        public static List<ItemData> Data =
        [
            new ItemData
            {
                localId = 1,
                key = "金虹剑诀_天阶",
                name = "金虹剑诀",
                skillKey = "@bookInfo:金虹剑诀_天阶",
                shopType = ShopType.秘市秘籍,
                itemFlag =
                [
                    ItemFlag.功法秘籍,
                    ItemFlag.天阶功法,
                    ItemFlag.天阶功法杂学
                ],
                desc2 = "金虹剑派秘传天阶战斗类功法，修炼者须修以金虹之法，则可使蓄灵气为剑势，剑出则锐不可当。",
                wuDao =
                [
                    (DaoType.剑, DaoLevel.融会贯通),
                    (DaoType.金, DaoLevel.略有小成)
                ],
                seidData = new()
                {
                    {
                        2,
                        new()
                        {
                            { "value1", "@ssId:金虹剑诀_天阶" }
                        }
                    }
                },
            },
            new ItemData
            {
                qingJiao = true,
                localId = 1,
                key = "金虹剑诀_天阶_请教",
                name = "金虹剑诀",
                skillKey = "@bookInfo:金虹剑诀_天阶",
                itemFlag =
                [
                    ItemFlag.功法秘籍,
                    ItemFlag.天阶功法,
                    ItemFlag.天阶功法杂学
                ],
                desc2 = "金虹剑派秘传天阶战斗类功法，修炼者须修以金虹之法，则可使蓄灵气为剑势，剑出则锐不可当。",
                wuDao =
                [
                    (DaoType.剑, DaoLevel.融会贯通),
                    (DaoType.金, DaoLevel.略有小成)
                ],
                seidData = new()
                {
                    {
                        2,
                        new()
                        {
                            { "value1", "@ssId:金虹剑诀_天阶" }
                        }
                    }
                },
            }
        ];
    }
}