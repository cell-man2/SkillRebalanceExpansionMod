using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class ItemFactory
    {
        private const int baseId = 28500;
        private const int qingJiaoOffset = 1000000000;
        private static readonly Dictionary<string, object> defaultData = new()
        {
            { "ItemIcon", 0 },
            { "maxNum", 9999999 },
            { "ShopType", 99 },
            { "ItemFlag", new List<int>() },
            { "typePinJie", 1 },
            { "StuTime", 0 },
            { "CanSale", 0 },
            { "seid", new List<int>() },
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
        private static readonly List<ItemData> itemDatas = [];
        private static readonly Dictionary<int, HashSet<int>> removeItemSeid = [];

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

        public static void Inject()
        {
            RemoveOldItemSeid();

            foreach (ItemData data in itemDatas)
            {
                InjectItemData(data);
                InjectItemSeid(data);
            }
        }

        private static void GenId(ItemData data)
        {
            if (data.realId.HasValue && data.realId.Value > qingJiaoOffset) return;

            data.realId ??= data.localId.Value + baseId;
            data.realId += data.qingJiao ? qingJiaoOffset : 0;
        }

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

        private static void ApplyItemData(JSONObject item, ItemData data)
        {
            if (data.name != null) item.SetField("name", data.name);
            if (data.itemIcon.HasValue) item.SetField("ItemIcon", data.itemIcon.Value);
            if (data.maxNum.HasValue) item.SetField("maxNum", data.maxNum.Value);
            if (data.type.HasValue) item.SetField("type", (int)data.type.Value);
            if (data.quality.HasValue) item.SetField("quality", data.quality.Value);
            if (data.typePinJie.HasValue) item.SetField("typePinJie", data.typePinJie.Value);
            if (data.tuJianType.HasValue) item.SetField("TuJianType", (int)data.tuJianType.Value);
            if (data.shopType.HasValue) item.SetField("ShopType", (int)data.shopType.Value);
            if (data.itemFlag != null)
            {
                JSONObject itemFlag = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (ItemFlag flag in data.itemFlag)
                {
                    itemFlag.Add((int)flag);
                }
                item.SetField("ItemFlag", itemFlag);
            }
            if (data.stuTime.HasValue) item.SetField("StuTime", data.stuTime.Value);
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
            if (data.desc != null) item.SetField("desc", data.desc);
            if (data.desc2 != null) item.SetField("desc2", data.desc2);
            if (data.price.HasValue) item.SetField("price", data.price.Value);
            if (data.canSale.HasValue) item.SetField("CanSale", data.canSale.Value ? 0 : 1);
            if (data.faBaoType != null) item.SetField("FaBaoType", data.faBaoType);
            if (data.affix != null)
            {
                JSONObject affix = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (int id in data.affix)
                {
                    affix.Add(id);
                }
                item.SetField("Affix", affix);
            }
            if (data.vagueType.HasValue) item.SetField("vagueType", data.vagueType.Value);
            if (data.canUse.HasValue) item.SetField("CanUse", data.canUse.Value);
            if (data.danDu.HasValue) item.SetField("DanDu", data.danDu.Value);
            if (data.npcCanUse.HasValue) item.SetField("NPCCanUse", data.npcCanUse.Value ? 1 : 0);
            if (data.shuXingType.HasValue) item.SetField("ShuXingType", data.shuXingType.Value);
            if (data.wuWeiType.HasValue) item.SetField("WuWeiType", data.wuWeiType.Value);
            if (data.shuaXin.HasValue) item.SetField("ShuaXin", data.shuaXin.Value);
            if (data.yaoZhi1.HasValue) item.SetField("yaoZhi1", data.yaoZhi1.Value);
            if (data.yaoZhi2.HasValue) item.SetField("yaoZhi2", data.yaoZhi2.Value);
            if (data.yaoZhi3.HasValue) item.SetField("yaoZhi3", data.yaoZhi3.Value);
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