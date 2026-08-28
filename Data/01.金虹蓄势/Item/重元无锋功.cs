using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    [DataBase(DataCategory.Item, "金虹蓄势", "重元无锋功")]
    public static class 重元无锋功
    {
        public static List<ItemData> Data =
        [
            new ItemData
            {
                realId = 4018,
                key = "重元无锋功",
                quality = 2,
                typePinJie = 3,
                price = 8000,
                stuTime = 360,
                wuDao =
                [
                    (DaoType.金, DaoLevel.略有小成),
                    (DaoType.剑, DaoLevel.初窥门径)
                ]
            },
            new ItemData
            {
                qingJiao = true,
                realId = 4018,
                key = "重元无锋功_请教",
                quality = 2,
                typePinJie = 3,
                stuTime = 360,
                wuDao =
                [
                    (DaoType.金, DaoLevel.略有小成),
                    (DaoType.剑, DaoLevel.初窥门径)
                ]
            }
        ];
    }
}