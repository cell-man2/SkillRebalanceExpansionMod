using SkillRebalanceExpansionMod.Models.Shop;

namespace SkillRebalanceExpansionMod.Data.Shop
{
    [DataBase(DataCategory.Shop, "金虹蓄势", "蓄势_天阶")]
    public static class 蓄势_天阶
    {
        public static void Register()
        {
            ShopCollection.金虹.秘阁法术.Add("@item:蓄势_天阶");
        }
    }
}

