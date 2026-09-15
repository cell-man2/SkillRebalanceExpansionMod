using SkillRebalanceExpansionMod.Models.Shop;

namespace SkillRebalanceExpansionMod.Data.Shop
{
    [DataBase(DataCategory.Shop, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        public static void Register()
        {
            ShopCollection.金虹.秘阁功法.Add("@item:金虹剑诀_天阶");
        }
    }
}

