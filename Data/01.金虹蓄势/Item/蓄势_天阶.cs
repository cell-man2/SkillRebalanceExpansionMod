using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "蓄势_天阶")]
    public static class 蓄势_天阶
    {
        public static List<ItemData> Data =
        [
            new ItemData
            {
                localId = 3,
                key = "蓄势_天阶",
                name = "蓄势",
                skillKey = "@bookInfo:蓄势_天阶",
                shopType = ShopType.秘市秘籍,
                itemFlag =
                [
                    ItemFlag.神通秘籍,
                    ItemFlag.天阶神通,
                    ItemFlag.天阶金系神通,
                    ItemFlag.天阶剑系神通
                ],
                desc2 = "金虹剑仙所创的天阶金系秘法，晦养厚积，藏锋敛锷，剑势凝若渊岳，只待一朝现兵。",
                wuDao =
                [
                    (DaoType.金, DaoLevel.融会贯通)
                ],
                seidData = new()
                {
                    {
                        1,
                        new()
                        {
                            { "value1", "@sId:蓄势_天阶" }
                        }
                    }
                },
            },
            new ItemData
            {
                localId = 3,
                key = "蓄势_天阶_请教",
                qingJiao = true,
                name = "蓄势",
                skillKey = "@bookInfo:蓄势_天阶",
                itemFlag =
                [
                    ItemFlag.神通秘籍,
                    ItemFlag.天阶神通,
                    ItemFlag.天阶金系神通,
                    ItemFlag.天阶剑系神通
                ],
                desc2 = "金虹剑仙所创的天阶金系秘法，晦养厚积，藏锋敛锷，剑势凝若渊岳，只待一朝现兵。",
                wuDao =
                [
                    (DaoType.金, DaoLevel.融会贯通)
                ],
                seidData = new()
                {
                    {
                        1,
                        new()
                        {
                            { "value1", "@sId:蓄势_天阶" }
                        }
                    }
                },
            }
        ];
    }
}