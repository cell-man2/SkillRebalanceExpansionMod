using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.Shop;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class ShopFactory
    {
        private const int baseId = 1470;
        private static readonly List<ShopDefinition> shops = ShopCollection.GetDefinitions();

        public static void Initialize()
        {
            foreach (ShopDefinition shop in shops) shop.Clear();
            DataManager.Register(DataCategory.Shop);
        }

        public static void Inject()
        {
            List<(int goodsId, int shopId)> removes = [];
            List<(
                int goodsId,
                int shopId,
                int exGoodsId,
                int money,
                int percent,
                List<int> eventValue,
                string fuhao
            )> adds = [];
            JSONObject json = jsonData.instance.jiaoHuanShopGoods;

            foreach (ShopDefinition shop in shops)
            {
                foreach (object goodsIdObject in shop.Removes)
                {
                    if (Registry.Resolve(goodsIdObject) is int goodsId)
                    {
                        removes.Add((goodsId, shop.ShopId));
                    }
                    else
                    {
                        Main.Log.LogWarning($"[ShopFactory] 无法解析移除商品 ID: {goodsIdObject}，ShopId: {shop.ShopId}");
                    }
                }

                foreach (Dictionary<string, object> data in shop.Adds)
                {
                    object goodsIdObject = data["goodsId"];
                    object exGoodsIdObject = data.TryGetValue("exGoodsId", out object value) ? value : shop.ExGoodsId;

                    if (Registry.Resolve(goodsIdObject) is not int goodsId)
                    {
                        Main.Log.LogWarning($"[ShopFactory] 无法解析新增商品 ID: {goodsIdObject}，ShopId: {shop.ShopId}");
                        continue;
                    }
                    if (Registry.Resolve(exGoodsIdObject) is not int exGoodsId)
                    {
                        Main.Log.LogWarning($"[ShopFactory] 无法解析兑换物 ID: {exGoodsIdObject}，ShopId: {shop.ShopId}");
                        continue;
                    }
                    int money = data.TryGetValue("money", out value) ? (int)value : shop.Money;
                    int percent = data.TryGetValue("percent", out value) ? (int)value : shop.Percent;
                    List<int> eventValue = data.TryGetValue("eventValue", out value) ? (List<int>)value : shop.EventValue;
                    string fuhao = data.TryGetValue("fuhao", out value) ? (string)value : shop.Fuhao;

                    adds.Add((goodsId, shop.ShopId, exGoodsId, money, percent, eventValue, fuhao));
                }
            }

            List<string> removeFields = [];
            foreach (string field in json.keys)
            {
                JSONObject goods = json.GetField(field);
                int goodsId = goods["GoodsID"].I;
                int shopId = goods["ShopID"].I;

                if (removes.Contains((goodsId, shopId))) removeFields.Add(field);
            }
            foreach (string field in removeFields) json.RemoveField(field);

            int nextId = baseId;
            foreach (
                (
                    int goodsId,
                    int shopId,
                    int exGoodsId,
                    int money,
                    int percent,
                    List<int> eventValue,
                    string fuhao
                ) add in adds
            ) {
                nextId += 1;
                JSONObject goods = JSONObject.Create();

                goods.AddField("id", nextId);
                goods.AddField("EventValue", JSONObjectHelper.ToJSONObject(add.eventValue));
                goods.AddField("EXGoodsID", add.exGoodsId);
                goods.AddField("fuhao", add.fuhao);
                goods.AddField("GoodsID", add.goodsId);
                goods.AddField("Money", add.money);
                goods.AddField("percent", add.percent);
                goods.AddField("ShopID", add.shopId);

                json.AddField(nextId.ToString(), goods);
            }
        }
    }
}