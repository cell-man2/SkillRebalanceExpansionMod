using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "蓄势_天阶")]
    public static class 蓄势_天阶
    {
        public static readonly List<ItemData> Data =
        [
            new ItemData
            {
                key = "蓄势_天阶",
                localId = 3,
                name = "蓄势",
                seidData = new()
                {
                    [1] = new()
                    {
                        ["value1"] = "@sId:蓄势_天阶"
                    }
                },
                skillKey = "@bookInfo:蓄势_天阶",
                desc2 = "金虹剑仙所创的天阶金系秘法，晦养厚积，藏锋敛锷，剑势凝若渊岳，只待一朝现兵。",
                itemFlag =
                [
                    ItemFlag.神通秘籍,
                    ItemFlag.天阶神通,
                    ItemFlag.天阶金系神通,
                    ItemFlag.天阶剑系神通
                ],
                shopType = ShopType.秘市秘籍,
                wuDao =
                [
                    (DaoType.金, DaoLevel.融会贯通)
                ],
            },
            new ItemData
            {
                key = "蓄势_天阶(请教)",
                localId = 3,
                name = "蓄势",
                seidData = new()
                {
                    [1] = new()
                    {
                        ["value1"] = "@sId:蓄势_天阶"
                    }
                },
                skillKey = "@bookInfo:蓄势_天阶",
                qingJiao = true,
                desc2 = "金虹剑仙所创的天阶金系秘法，晦养厚积，藏锋敛锷，剑势凝若渊岳，只待一朝现兵。",
                itemFlag =
                [
                    ItemFlag.神通秘籍,
                    ItemFlag.天阶神通,
                    ItemFlag.天阶金系神通,
                    ItemFlag.天阶剑系神通
                ],
                wuDao =
                [
                    (DaoType.金, DaoLevel.融会贯通)
                ],
            }
        ];
    }
}