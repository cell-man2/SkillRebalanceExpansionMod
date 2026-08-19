using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Models.Shop
{
    public class ShopDefinition(
        int shopId,
        object exGoodsId = null,
        int money = 0,
        int percent = 100,
        List<int> eventValue = null,
        string fuhao = ""
    ) {
        public int ShopId { get; } = shopId;
        public object ExGoodsId { get; } = exGoodsId;
        public int Money { get; } = money;
        public int Percent { get; } = percent;
        public List<int> EventValue { get; } = eventValue;
        public string Fuhao { get; } = fuhao;

        public List<Dictionary<string, object>> Adds { get; } = [];
        public List<object> Removes { get; } = [];

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

        public void Remove(object goodsId)
        {
            Removes.Add(goodsId);
        }
    
        public void Clear()
        {
            Adds.Clear();
            Removes.Clear();
        }
    }
}