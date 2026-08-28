using System.Collections.Generic;
using System;
using System.Reflection;

namespace SkillRebalanceExpansionMod.Models.Shop
{
    /// <summary>
    /// 商店集合类，提供各门派/势力商店的定义和访问入口。
    /// 每个商店通过静态字段暴露，供数据层的 Register() 方法调用 Add()/Remove()。
    /// </summary>
    public static class ShopCollection
    {
        /// <summary>
        /// 创建门派商店（贡献兑换）的工厂方法。
        /// 价格 = 物品原价 / percent，使用门派贡献货币兑换。
        /// </summary>
        private static ShopDefinition PercentShop(int id, object exGoodsId, int percent = 100) => new ShopDefinition(
            shopId: id, exGoodsId: exGoodsId, money: 0, percent: percent,
            eventValue: [], fuhao: "");

        /// <summary>
        /// 创建兑换商店（灵石购买）的工厂方法。
        /// 价格固定为 money 值，使用灵石兑换。
        /// </summary>
        private static ShopDefinition MoneyShop(int id, object exGoodsId) => new ShopDefinition(
            shopId: id, exGoodsId: exGoodsId, money: 1, percent: 0,
            eventValue: [], fuhao: "");

        /// <summary>
        /// 通过反射获取所有嵌套类中的静态 ShopDefinition 字段。
        /// 在 ShopFactory.Initialize 中调用，收集所有商店定义。
        /// </summary>
        public static List<ShopDefinition> GetDefinitions()
        {
            List<ShopDefinition> result = [];
            foreach (Type type in typeof(ShopCollection).GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field.FieldType != typeof(ShopDefinition)) continue;
                    if (field.GetValue(null) is ShopDefinition definition) result.Add(definition);
                }
            }
            return result;
        }


        public static class 金虹
        {
            public static ShopDefinition 灵核兑换 = MoneyShop(3, 10005);
            public static ShopDefinition 藏经阁法术 = PercentShop(31, 10011);
            public static ShopDefinition 藏经阁功法 = PercentShop(32, 10011);
            public static ShopDefinition 藏经阁杂文 = PercentShop(33, 10011);
            public static ShopDefinition 神兵阁武器 = PercentShop(34, 10011);
            public static ShopDefinition 神兵阁内甲 = PercentShop(35, 10011);
            public static ShopDefinition 神兵阁饰品 = PercentShop(36, 10011);
            /// <summary>一/二品草药售卖区</summary>
            public static ShopDefinition 药房草药1 = PercentShop(37, 10011);
            /// <summary>三品草药售卖区</summary>
            public static ShopDefinition 药房草药2 = PercentShop(38, 10011);
            /// <summary>四品草药售卖区</summary>
            public static ShopDefinition 药房草药3 = PercentShop(39, 10011);
            public static ShopDefinition 秘阁法术 = PercentShop(131, 10011);
            public static ShopDefinition 秘阁功法 = PercentShop(132, 10011);
            public static ShopDefinition 秘阁杂文 = PercentShop(133, 10011);
        }

        public static class 竹山
        {
            public static ShopDefinition 灵核兑换 = MoneyShop(1, 10010);
            public static ShopDefinition 藏经阁法术 = PercentShop(11, 10010);
            public static ShopDefinition 藏经阁功法 = PercentShop(12, 10010);
            public static ShopDefinition 藏经阁杂文 = PercentShop(13, 10010);
            public static ShopDefinition 神兵阁武器 = PercentShop(14, 10010);
            public static ShopDefinition 神兵阁内甲 = PercentShop(15, 10010);
            public static ShopDefinition 神兵阁饰品 = PercentShop(16, 10010);
            /// <summary>一/二品草药售卖区</summary>
            public static ShopDefinition 药房草药1 = PercentShop(17, 10010);
            /// <summary>三品草药售卖区</summary>
            public static ShopDefinition 药房草药2 = PercentShop(18, 10010);
            /// <summary>四品草药售卖区</summary>
            public static ShopDefinition 药房草药3 = PercentShop(19, 10010);
            public static ShopDefinition 秘阁法术 = PercentShop(111, 10010);
            public static ShopDefinition 秘阁功法 = PercentShop(112, 10010);
            public static ShopDefinition 秘阁杂文 = PercentShop(113, 10010);
        }

        public static class 星河
        {
            public static ShopDefinition 灵核兑换 = MoneyShop(4, 10012);
            public static ShopDefinition 藏经阁法术 = PercentShop(41, 10012);
            public static ShopDefinition 藏经阁功法 = PercentShop(42, 10012);
            public static ShopDefinition 藏经阁杂文 = PercentShop(43, 10012);
            public static ShopDefinition 神兵阁武器 = PercentShop(44, 10012);
            public static ShopDefinition 神兵阁内甲 = PercentShop(45, 10012);
            public static ShopDefinition 神兵阁饰品 = PercentShop(46, 10012);
            /// <summary>一/二品草药售卖区</summary>
            public static ShopDefinition 药房草药1 = PercentShop(47, 10012);
            /// <summary>三品草药售卖区</summary>
            public static ShopDefinition 药房草药2 = PercentShop(48, 10012);
            /// <summary>四品草药售卖区</summary>
            public static ShopDefinition 药房草药3 = PercentShop(49, 10012);
            public static ShopDefinition 秘阁法术 = PercentShop(141, 10012);
            public static ShopDefinition 秘阁功法 = PercentShop(142, 10012);
            public static ShopDefinition 秘阁杂文 = PercentShop(143, 10012);
        }

        public static class 离火
        {
            public static ShopDefinition 灵核兑换 = MoneyShop(2, 10013);
            public static ShopDefinition 藏经阁法术 = PercentShop(21, 10013);
            public static ShopDefinition 藏经阁功法 = PercentShop(22, 10013);
            public static ShopDefinition 藏经阁杂文 = PercentShop(23, 10013);
            public static ShopDefinition 神兵阁武器 = PercentShop(24, 10013);
            public static ShopDefinition 神兵阁内甲 = PercentShop(25, 10013);
            public static ShopDefinition 神兵阁饰品 = PercentShop(26, 10013);
            /// <summary>一/二品草药售卖区</summary>
            public static ShopDefinition 药房草药1 = PercentShop(27, 10013);
            /// <summary>三品草药售卖区</summary>
            public static ShopDefinition 药房草药2 = PercentShop(28, 10013);
            /// <summary>四品草药售卖区</summary>
            public static ShopDefinition 药房草药3 = PercentShop(29, 10013);
            public static ShopDefinition 秘阁法术 = PercentShop(121, 10013);
            public static ShopDefinition 秘阁功法 = PercentShop(122, 10013);
            public static ShopDefinition 秘阁杂文 = PercentShop(123, 10013);
        }

        public static class 化尘
        {
            public static ShopDefinition 灵核兑换 = MoneyShop(5, 10014);
            public static ShopDefinition 藏经阁法术 = PercentShop(51, 10014);
            public static ShopDefinition 藏经阁功法 = PercentShop(52, 10014);
            public static ShopDefinition 藏经阁杂文 = PercentShop(53, 10014);
            public static ShopDefinition 神兵阁武器 = PercentShop(54, 10014);
            public static ShopDefinition 神兵阁内甲 = PercentShop(55, 10014);
            public static ShopDefinition 神兵阁饰品 = PercentShop(56, 10014);
            /// <summary>一/二品草药售卖区</summary>
            public static ShopDefinition 药房草药1 = PercentShop(57, 10014);
            /// <summary>三品草药售卖区</summary>
            public static ShopDefinition 药房草药2 = PercentShop(58, 10014);
            /// <summary>四品草药售卖区</summary>
            public static ShopDefinition 药房草药3 = PercentShop(59, 10014);
            public static ShopDefinition 秘阁法术 = PercentShop(151, 10014);
            public static ShopDefinition 秘阁功法 = PercentShop(152, 10014);
            public static ShopDefinition 秘阁杂文 = PercentShop(153, 10014);
        }

        public static class 白帝
        {
            public static ShopDefinition 灵核兑换 = MoneyShop(6, 10005);
            public static ShopDefinition 外阁商店 = PercentShop(7, 10035, 1);
            public static ShopDefinition 内阁商店 = PercentShop(8, 10035, 1);
            public static ShopDefinition 内阁高境界 = PercentShop(9, 10035, 1);
            public static ShopDefinition 外阁高声望 = PercentShop(10, 10035, 1);
        }

        public static class 天机
        {
            public static ShopDefinition 五品材料 = PercentShop(1210, 10071, 10000);
            public static ShopDefinition 六品材料 = PercentShop(1211, 10071, 10000);
            public static ShopDefinition 秘籍 = PercentShop(1212, 10071, 10000);
        }

        public static class 海外
        {
            public static ShopDefinition 阴魂岛 = MoneyShop(98, 10072);
            public static ShopDefinition 采筠阁 = MoneyShop(99, 10058);
            /// <summary>第一次提交龙鳞</summary>
            public static ShopDefinition 龙宫1 = PercentShop(341, 10035, 1);
            /// <summary>未知情景</summary>
            public static ShopDefinition 龙宫2 = PercentShop(342, 10035, 1);
            /// <summary>第二次提交龙鳞</summary>
            public static ShopDefinition 龙宫3 = PercentShop(343, 10035, 1);
        }
    }
}