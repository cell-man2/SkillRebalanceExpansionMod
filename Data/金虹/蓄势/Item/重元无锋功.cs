using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "重元无锋功")]
    public static class 重元无锋功
    {
        public static readonly List<ItemData> Data =
        [
            new ItemData
            {
                key = "重元无锋功",
                realId = 4018,
                price = 8000,
                quality = 2,
                stuTime = 360,
                typePinJie = 3,
                wuDao =
                [
                    (DaoType.金, DaoLevel.略有小成),
                    (DaoType.剑, DaoLevel.初窥门径)
                ]
            },
            new ItemData
            {
                key = "重元无锋功(请教)",
                realId = 4018,
                qingJiao = true,
                quality = 2,
                stuTime = 360,
                typePinJie = 3,
                wuDao =
                [
                    (DaoType.金, DaoLevel.略有小成),
                    (DaoType.剑, DaoLevel.初窥门径)
                ]
            }
        ];
    }
}