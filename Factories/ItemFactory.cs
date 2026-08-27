using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Factories
{
    /// <summary>
    /// 物品工厂，负责扫描所有标记为 DataCategory.Item 的数据类，
    /// 将其注入到游戏的物品 JSON 数据中。
    /// </summary>
    public static class ItemFactory
    {
        // ======================== 常量 ========================

        /// <summary>
        /// 物品 ID 基数，localId 最终偏移为 baseId + localId。
        /// </summary>
        private const int baseId = 28500;

        /// <summary>
        /// 请教版本物品 ID 偏移量，用于区分常规物品与请教版本。
        /// </summary>
        private const int qingJiaoOffset = 1000000000;

        // ======================== 静态字段 ========================

        /// <summary>
        /// 新建物品时的默认字段值。
        /// </summary>
        private static readonly Dictionary<string, object> defaultData = new()
        {
            { "ItemIcon", 0 },
            { "maxNum", 9999999 },
            { "ShopType", 99 },
            { "ItemFlag", new List<int>() },
            { "typePinJie", 1 },
            { "StuTime", 0 },
            { "CanSale", 0 },
            { "vagueType", 1 },
            { "price", 0 },
            { "wuDao", new List<int>() },
            { "TuJianType", 0 },
            { "FaBaoType", "" },
            { "Affix", new List<int>() },
            { "CanUse", 0 },
            { "DanDu", 0 },
            { "NPCCanUse", 0 },
            { "ShuaXin", 0 },
            { "ShuXingType", 0 },
            { "WuWeiType", 0 },
            { "yaoZhi1", 0 },
            { "yaoZhi2", 0 },
            { "yaoZhi3", 0 }
        };

        /// <summary>
        /// 缓存所有扫描到的 ItemData，供 Inject 阶段使用。
        /// </summary>
        private static readonly List<ItemData> itemDatas = [];

        /// <summary>
        /// 记录每个 seid 编号下挂载了哪些物品 ID。
        /// Key: seid 编号, Value: 使用该 seid 的物品 ID 集合。
        /// 用于注入前清理旧数据，防止残留。
        /// </summary>
        private static readonly Dictionary<int, HashSet<int>> removeItemSeid = [];

        // ======================== 工具函数 ========================

        /// <summary>
        /// 根据 BookInfo 计算物品价格。
        /// </summary>
        private static int CaculatePrice(BookInfo info)
        {
            if (!info.jie.HasValue || !info.pin.HasValue) return 0;
            if (info.type == BookType.功法)
            {
                return info.jie.Value switch
                {
                    1 => info.pin.Value switch
                    {
                        1 => 100,
                        2 => 150,
                        3 => 200,
                        _ => 0
                    },
                    2 => info.pin.Value switch
                    {
                        1 => 4000,
                        2 => 6000,
                        3 => 8000,
                        _ => 0
                    },
                    3 => info.pin.Value switch
                    {
                        1 => 480000,
                        2 => 800000,
                        3 => 1000000,
                        _ => 0
                    },
                    _ => 0
                };
            }

            if (info.type == BookType.神通)
            {
                return info.jie.Value switch
                {
                    1 => info.pin.Value switch
                    {
                        1 => 50,
                        2 => 100,
                        3 => 150,
                        _ => 0
                    },
                    2 => info.pin.Value switch
                    {
                        1 => 1200,
                        2 => 1600,
                        3 => 2000,
                        _ => 0
                    },
                    3 => info.pin.Value switch
                    {
                        1 => 36000,
                        2 => 42000,
                        3 => 48000,
                        _ => 0
                    },
                    _ => 0
                };
            }
            return 0;
        }

        /// <summary>
        /// 根据 BookInfo 计算领悟时间。
        /// </summary>
        private static int CaculateStuTime(BookInfo info)
        {
            if (!info.jie.HasValue || !info.pin.HasValue) return 0;
            if (info.type == BookType.功法)
            {
                return info.jie.Value switch
                {
                    1 => info.pin.Value switch
                    {
                        1 => 120,
                        2 => 152,
                        3 => 180,
                        _ => 0
                    },
                    2 => info.pin.Value switch
                    {
                        1 => 288,
                        2 => 320,
                        3 => 360,
                        _ => 0
                    },
                    3 => info.pin.Value switch
                    {
                        1 => 480,
                        2 => 600,
                        3 => 720,
                        _ => 0
                    },
                    _ => 0
                };
            }

            if (info.type == BookType.神通)
            {
                return info.jie.Value switch
                {
                    1 => info.pin.Value switch
                    {
                        1 => 12,
                        2 => 24,
                        3 => 48,
                        _ => 0
                    },
                    2 => info.pin.Value switch
                    {
                        1 => 72,
                        2 => 96,
                        3 => 120,
                        _ => 0
                    },
                    3 => info.pin.Value switch
                    {
                        1 => 144,
                        2 => 240,
                        3 => 240,
                        _ => 0
                    },
                    _ => 0
                };
            }
            return 0;
        }

        // ======================== 公开方法 ========================

        /// <summary>
        /// 初始化阶段：扫描所有物品数据并注册到 Registry。
        /// </summary>
        public static void Initialize()
        {
            itemDatas.Clear();
            removeItemSeid.Clear();

            List<ItemData> datas = DataManager.Scan<ItemData>(DataCategory.Item);
            itemDatas.AddRange(datas);

            foreach (ItemData data in itemDatas)
            {
                GenId(data);
                if (!string.IsNullOrEmpty(data.key))
                {
                    if (!Registry.item.ContainsKey(data.key))
                    {
                        Registry.item.Add(data.key, data.realId.Value);
                    }
                    else
                    {
                        Main.Log.LogWarning($"Item Key重复:{data.key}, 重复数据id:{data.realId.Value}");
                    }
                }

                if (data.realId.HasValue && data.seidData != null)
                {
                    RegisterRemoveItemSeid(data);
                }
            }
        }

        /// <summary>
        /// 注入阶段：将物品数据写入游戏 JSON。
        /// </summary>
        public static void Inject()
        {
            RemoveOldItemSeid();

            foreach (ItemData data in itemDatas)
            {
                InjectItemData(data);
                InjectItemSeid(data);
            }

            itemDatas.Clear();
        }

        // ======================== 私有方法（按执行顺序排列） ========================

        /// <summary>
        /// 生成实际 ID：baseId + localId，若 qingJiao 为 true 则额外加上 qingJiaoOffset。
        /// 若 realId 已存在且大于 qingJiaoOffset（即已偏移过），则跳过生成。
        /// </summary>
        private static void GenId(ItemData data)
        {
            if (data.realId.HasValue && data.realId.Value > qingJiaoOffset) return;

            data.realId ??= data.localId.Value + baseId;
            data.realId += data.qingJiao ? qingJiaoOffset : 0;
        }

        /// <summary>
        /// 从游戏现有的 _ItemJsonData 中读取该物品已挂载的 seid 列表，
        /// 记录到 removeItemSeid 中供后续清理。
        /// </summary>
        private static void RegisterRemoveItemSeid(ItemData data)
        {
            JSONObject json = jsonData.instance._ItemJsonData;
            if (!json.HasField(data.realId.Value.ToString())) return;

            JSONObject item = json.GetField(data.realId.Value.ToString());
            if (!item.HasField("seid")) return;

            int seidItemId = data.qingJiao
                ? data.realId.Value - qingJiaoOffset
                : data.realId.Value;

            foreach (JSONObject seid in item.GetField("seid").list)
            {
                if (!removeItemSeid.TryGetValue(seid.I, out HashSet<int> ids))
                {
                    ids = [];
                    removeItemSeid.Add(seid.I, ids);
                }
                ids.Add(seidItemId);
            }
        }

        /// <summary>
        /// 从各 seid 表中移除旧物品的 seid 数据。
        /// </summary>
        private static void RemoveOldItemSeid()
        {
            foreach (var pair in removeItemSeid)
            {
                JSONObject seidTable = jsonData.instance.ItemsSeidJsonData[pair.Key];
                foreach (int id in pair.Value)
                {
                    seidTable.RemoveField(id.ToString());
                }
            }
            removeItemSeid.Clear();
        }

        /// <summary>
        /// 构建一个带有默认值的新物品 JSON 对象。
        /// </summary>
        private static JSONObject BuildNewItem(ItemData data)
        {
            JSONObject item = JSONObject.Create(JSONObject.Type.OBJECT);
            item.AddField("id", data.realId.Value);

            foreach (var kvp in defaultData)
            {
                item.AddField(
                    kvp.Key,
                    JSONObjectHelper.ToJSONObject(kvp.Value)
                );
            }
            return item;
        }

        /// <summary>
        /// 初始化技能书/功法书类物品，从 BookInfo 中自动填充相关字段。
        /// 仅在新建物品时调用（ID 不存在时）。
        /// </summary>
        private static void InitNewItem(JSONObject item, ItemData data)
        {
            if (string.IsNullOrEmpty(data.skillKey)) return;

            if (Registry.Resolve(data.skillKey) is not BookInfo info)
            {
                Main.Log.LogWarning($"尝试索引的BookInfo不存在:{data.skillKey}");
                return;
            }

            int? jie = info.jie;
            int? pin = info.pin;

            if (jie.HasValue)
            {
                item.SetField("ItemIcon", 3000 + jie.Value);
                item.SetField("quality", jie.Value);
            }

            if (pin.HasValue)
            {
                item.SetField("typePinJie", pin.Value);
            }

            item.SetField("desc", info.skillId.ToString());
            item.SetField("maxNum", 1);
            item.SetField("TuJianType", 0);
            item.SetField("type", info.type == BookType.功法 ? 4 : 3);
            item.SetField("vagueType", 1);
            item.SetField("CanSale", 0);

            if (jie.HasValue && pin.HasValue)
            {
                item.SetField("StuTime", CaculateStuTime(info));
                item.SetField("price", CaculatePrice(info));
            }
        }

        /// <summary>
        /// 将 ItemData 中的非空字段应用到 JSON 对象上。
        /// </summary>
        private static void ApplyItemData(JSONObject item, ItemData data)
        {
            // affix → Affix
            if (data.affix != null)
            {
                JSONObject affix = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (int id in data.affix)
                {
                    affix.Add(id);
                }
                item.SetField("Affix", affix);
            }
            // canSale → CanSale（false = 可出售，true = 不可出售）
            if (data.canSale.HasValue) item.SetField("CanSale", data.canSale.Value ? 0 : 1);
            // canUse → CanUse
            if (data.canUse.HasValue) item.SetField("CanUse", data.canUse.Value);
            // danDu → DanDu
            if (data.danDu.HasValue) item.SetField("DanDu", data.danDu.Value);
            // desc → desc
            if (data.desc != null) item.SetField("desc", data.desc);
            // desc2 → desc2
            if (data.desc2 != null) item.SetField("desc2", data.desc2);
            // faBaoType → FaBaoType
            if (data.faBaoType != null) item.SetField("FaBaoType", data.faBaoType);
            // itemFlag → ItemFlag
            if (data.itemFlag != null)
            {
                JSONObject itemFlag = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (ItemFlag flag in data.itemFlag)
                {
                    itemFlag.Add((int)flag);
                }
                item.SetField("ItemFlag", itemFlag);
            }
            // itemIcon → ItemIcon
            if (data.itemIcon.HasValue) item.SetField("ItemIcon", data.itemIcon.Value);
            // maxNum → maxNum
            if (data.maxNum.HasValue) item.SetField("maxNum", data.maxNum.Value);
            // name → name
            if (data.name != null) item.SetField("name", data.name);
            // npcCanUse → NPCCanUse
            if (data.npcCanUse.HasValue) item.SetField("NPCCanUse", data.npcCanUse.Value ? 1 : 0);
            // price → price
            if (data.price.HasValue) item.SetField("price", data.price.Value);
            // quality → quality
            if (data.quality.HasValue) item.SetField("quality", data.quality.Value);
            // seidData → seid
            if (data.seidData != null)
            {
                JSONObject seid = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (object rawId in data.seidData.Keys)
                {
                    object result = Registry.Resolve(rawId);
                    if (result is not int id)
                    {
                        Main.Log.LogWarning($"Seid引用无效:{rawId}");
                        continue;
                    }
                    seid.Add(id);
                }
                item.SetField("seid", seid);
            }
            // shopType → ShopType
            if (data.shopType.HasValue) item.SetField("ShopType", (int)data.shopType.Value);
            // shuaXin → ShuaXin
            if (data.shuaXin.HasValue) item.SetField("ShuaXin", data.shuaXin.Value);
            // shuXingType → ShuXingType
            if (data.shuXingType.HasValue) item.SetField("ShuXingType", data.shuXingType.Value);
            // stuTime → StuTime
            if (data.stuTime.HasValue) item.SetField("StuTime", data.stuTime.Value);
            // tuJianType → TuJianType
            if (data.tuJianType.HasValue) item.SetField("TuJianType", (int)data.tuJianType.Value);
            // type → type
            if (data.type.HasValue) item.SetField("type", (int)data.type.Value);
            // typePinJie → typePinJie
            if (data.typePinJie.HasValue) item.SetField("typePinJie", data.typePinJie.Value);
            // vagueType → vagueType
            if (data.vagueType.HasValue) item.SetField("vagueType", data.vagueType.Value);
            // wuDao → wuDao
            if (data.wuDao != null)
            {
                JSONObject wuDao = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach ((DaoType type, DaoLevel level) in data.wuDao)
                {
                    wuDao.Add((int)type);
                    wuDao.Add((int)level);
                }
                item.SetField("wuDao", wuDao);
            }
            // wuWeiType → WuWeiType
            if (data.wuWeiType.HasValue) item.SetField("WuWeiType", data.wuWeiType.Value);
            // yaoZhi1 → yaoZhi1
            if (data.yaoZhi1.HasValue) item.SetField("yaoZhi1", data.yaoZhi1.Value);
            // yaoZhi2 → yaoZhi2
            if (data.yaoZhi2.HasValue) item.SetField("yaoZhi2", data.yaoZhi2.Value);
            // yaoZhi3 → yaoZhi3
            if (data.yaoZhi3.HasValue) item.SetField("yaoZhi3", data.yaoZhi3.Value);

            // 请教版本特殊处理：价格固定为 1，ShopType 固定为 99（不投放）
            if (data.qingJiao)
            {
                item.SetField("price", 1);
                item.SetField("ShopType", 99);

                if (!jsonData.instance.NpcQingJiaoItemData.HasField(data.realId.Value.ToString()))
                {
                    jsonData.instance.NpcQingJiaoItemData.AddField(data.realId.Value.ToString(), item);
                }
            }
        }

        /// <summary>
        /// 将单个 ItemData 注入到 _ItemJsonData 中。
        /// 若 ID 已存在则更新，否则新建并调用 InitNewItem 初始化技能书相关字段。
        /// </summary>
        private static void InjectItemData(ItemData data)
        {
            string id = data.realId.Value.ToString();

            JSONObject item;
            if (jsonData.instance._ItemJsonData.HasField(id))
            {
                item = jsonData.instance._ItemJsonData.GetField(id);
            }
            else
            {
                item = BuildNewItem(data);
                jsonData.instance._ItemJsonData.AddField(id, item);
                jsonData.instance.ItemJsonData.Add(id, item);

                InitNewItem(item, data);
            }

            ApplyItemData(item, data);
        }

        /// <summary>
        /// 将物品的 seid 特性数据注入到 ItemsSeidJsonData 中。
        /// </summary>
        private static void InjectItemSeid(ItemData data)
        {
            if (data.seidData == null || data.seidData.Count == 0) return;
            
            int seidItemId = data.qingJiao
                ? data.realId.Value - qingJiaoOffset
                : data.realId.Value;

            foreach (var seid in data.seidData)
            {
                if (seid.Value == null) continue;

                object result = Registry.Resolve(seid.Key);
                if (result is not int seidId)
                {
                    Main.Log.LogWarning($"Seid引用无效:{seid.Key}");
                    continue;
                }

                if (seidId < 0 || seidId >= jsonData.instance.ItemsSeidJsonData.Length)
                {
                    Main.Log.LogWarning($"ItemSeid索引越界:{seidId}");
                    continue;
                }

                JSONObject seidTable = jsonData.instance.ItemsSeidJsonData[seidId];
                if (seidTable == null)
                {
                    Main.Log.LogWarning($"ItemsSeid未初始化:{seidId}");
                    continue;
                }

                JSONObject seidJson = JSONObject.Create(JSONObject.Type.OBJECT);
                seidJson.AddField("id", seidItemId);
                foreach (var kpv in seid.Value)
                {
                    if (kpv.Key == "id") continue;

                    seidJson.AddField(
                        kpv.Key,
                        JSONObjectHelper.ToJSONObject(Registry.Resolve(kpv.Value))
                    );
                }

                seidTable.SetField(
                    seidItemId.ToString(),
                    seidJson
                );
            }
        }
    }
}