using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Models.Shop
{
    /// <summary>
    /// 商店定义类，用于描述一个游戏内商店的配置，并提供添加/移除商品的方法。
    /// 修改请求会在 Initialize 阶段收集，在 Inject 阶段统一应用到游戏 JSON。
    /// </summary>
    public class ShopDefinition(
        int shopId,
        object exGoodsId = null,
        int money = 0,
        int percent = 100,
        List<int> eventValue = null,
        string fuhao = ""
    ) {
        /// <summary>商店 ID，对应游戏内商店的 ShopID。</summary>
        public int ShopId { get; } = shopId;

        /// <summary>默认兑换物 ID（如门派贡献、灵石等），支持字符串引用。</summary>
        public object ExGoodsId { get; } = exGoodsId;

        /// <summary>
        /// 默认灵石价格（仅兑换商店有效）。
        /// 门派商店忽略此值，由 percent 计算。
        /// </summary>
        public int Money { get; } = money;

        /// <summary>
        /// 默认价格百分比（仅门派商店有效）。
        /// 实际价格 = 物品原价 / percent。不可为 0。
        /// 兑换商店忽略此值。
        /// </summary>
        public int Percent { get; } = percent;

        /// <summary>默认事件值，与 fuhao 组合用于剧情条件判定。</summary>
        public List<int> EventValue { get; } = eventValue;

        /// <summary>默认符号，与 eventValue 组合用于剧情条件判定。</summary>
        public string Fuhao { get; } = fuhao;

        /// <summary>待添加的商品列表，每个商品以 Dictionary 存储各字段。</summary>
        public List<Dictionary<string, object>> Adds { get; } = [];

        /// <summary>待移除的商品 ID 列表，支持字符串引用。</summary>
        public List<object> Removes { get; } = [];

        /// <summary>
        /// 向商店添加一个商品。
        /// </summary>
        /// <param name="goodsId">商品 ID，支持字符串引用。</param>
        /// <param name="exGoodsId">兑换物 ID，若不填则继承商店默认值。</param>
        /// <param name="money">直接价格（兑换商店用），若不填则继承商店默认值。</param>
        /// <param name="percent">价格百分比（门派商店用），若不填则继承商店默认值。</param>
        /// <param name="eventValue">事件值，若不填则继承商店默认值（空）。</param>
        /// <param name="fuhao">符号，若不填则继承商店默认值（空）。</param>
        public void Add(
            object goodsId,
            object exGoodsId = null,
            int? money = null,
            int? percent = null,
            List<int> eventValue = null,
            string fuhao = null
        ) {
            var data = new Dictionary<string, object>
            {
                ["goodsId"] = goodsId
            };

            if (exGoodsId != null) data["exGoodsId"] = exGoodsId;
            if (money.HasValue) data["money"] = money.Value;
            if (percent.HasValue) data["percent"] = percent.Value;
            if (eventValue != null) data["eventValue"] = eventValue;
            if (fuhao != null) data["fuhao"] = fuhao;

            Adds.Add(data);
        }

        /// <summary>
        /// 从商店移除一个商品。
        /// </summary>
        /// <param name="goodsId">商品 ID，支持字符串引用。</param>
        public void Remove(object goodsId)
        {
            Removes.Add(goodsId);
        }

        /// <summary>
        /// 清空当前商店的添加/移除列表（每次 Initialize 时调用，防止残留）。
        /// </summary>
        public void Clear()
        {
            Adds.Clear();
            Removes.Clear();
        }
    }
}