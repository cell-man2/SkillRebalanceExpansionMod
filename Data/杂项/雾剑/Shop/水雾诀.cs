using SkillRebalanceExpansionMod.Models.Shop;

namespace SkillRebalanceExpansionMod.Data.Shop
{
    /// <summary>
    /// 【请替换为商店调整名称】商店数据。
    /// </summary>
    /// <remarks>
    /// 本类通过 Register() 方法向指定商店发送商品增删命令，由 ShopFactory 在初始化阶段收集并统一执行。
    /// 
    /// ShopCollection 结构：
    ///   ShopCollection.地区势力.具体商店
    ///   
    ///   示例：ShopCollection.金虹.藏经阁法术
    ///        ShopCollection.竹山.灵核兑换
    ///        ShopCollection.天机.秘籍
    /// 
    /// 操作方式：
    ///   添加商品：ShopCollection.XXX.商店.Add(goodsId, exGoodsId, money, percent, eventValue, fuhao)
    ///   移除商品：ShopCollection.XXX.商店.Remove(goodsId)
    /// 
    /// 参数说明（Add 方法，未传参数继承商店默认值）：
    ///   goodsId     - 商品 ID，必填，支持字符串引用（如 "@item:XXX"）
    ///   exGoodsId   - 兑换物 ID（即货币类型），支持字符串引用
    ///   money       - 直接价格（仅兑换商店生效），若不填则使用商店默认值（兑换商店默认价格为 1）
    ///   percent     - 价格百分比（仅门派商店生效，价格为原价（灵石） / percent），可自行调节但不能为 0 会报错
    ///   eventValue  - 事件值（剧情条件判定，参考真元玄典怎么写的）
    ///   fuhao       - 符号（剧情条件判定，参考真元玄典怎么写的）
    /// </remarks>
    [DataBase(DataCategory.Shop, "杂项雾剑", "水雾诀")]
    public static class 水雾诀
    {
        public static void Register()
        {
            ShopCollection.星河.藏经阁功法.Add("@item:水雾诀");
        }
    }
}