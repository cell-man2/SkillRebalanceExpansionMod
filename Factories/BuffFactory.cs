using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Factories
{
    /// <summary>
    /// Buff 工厂，负责扫描所有标记为 DataCategory.Buff 的数据类，
    /// 将其注入到游戏的 Buff JSON 数据中。
    /// </summary>
    public static class BuffFactory
    {
        // ======================== 常量 ========================

        /// <summary>
        /// Buff ID 基数，localId 最终偏移为 baseId + localId。
        /// </summary>
        private const int baseId = 175240;

        // ======================== 静态字段 ========================

        /// <summary>
        /// 新建 Buff 时的默认字段值。
        /// </summary>
        private static readonly Dictionary<string, object> defaultData = new()
        {
            { "Affix", new List<int>() },
            { "BuffIcon", 0 },
            { "script", "Buff" },
            { "looptime", 1 },
            { "totaltime", 1 },
            { "BuffType", (int)StackType.叠加 },
            { "isHide", 0 },
            { "ShowOnlyOne", 0 },
            { "skillEffect", "fx_Summoner_o" }
        };

        /// <summary>
        /// 记录每个 seid 编号下挂载了哪些 Buff ID。
        /// Key: seid 编号, Value: 使用该 seid 的 Buff ID 集合。
        /// 用于注入前清理旧数据，防止残留。
        /// </summary>
        private static readonly Dictionary<int, HashSet<int>> removeBuffSeid = [];

        /// <summary>
        /// 缓存所有扫描到的 BuffData，供 Inject 阶段使用。
        /// </summary>
        private static readonly List<BuffData> buffDatas = [];

        // ======================== 公开方法 ========================

        /// <summary>
        /// 初始化阶段：扫描所有 Buff 数据并注册到 Registry。
        /// </summary>
        public static void Initialize()
        {
            buffDatas.Clear();

            // 扫描程序集中所有标记为 DataCategory.Buff 的数据类
            List<BuffData> datas = DataManager.Scan<BuffData>(DataCategory.Buff);
            buffDatas.AddRange(datas);

            foreach (BuffData data in buffDatas)
            {
                // 注册 Key → ID 映射
                if (!string.IsNullOrEmpty(data.key))
                {
                    data.realId ??= GenId(data.localId.Value);

                    if (!Registry.buff.ContainsKey(data.key))
                    {
                        Registry.buff.Add(data.key, data.realId.Value);
                    }
                    else
                    {
                        Main.Log.LogWarning($"Buff Key重复:{data.key}, 重复数据id:{data.realId.Value}");
                    }
                }

                // 收集该 Buff 当前使用的 seid，用于后续清理
                if (data.realId.HasValue && data.seidData != null)
                {
                    RegisterRemoveBuffSeid(data);
                }
            }
        }

        /// <summary>
        /// 注入阶段：将 Buff 数据写入游戏 JSON。
        /// </summary>
        public static void Inject()
        {
            // 先清理旧 seid 数据，避免残留
            RemoveOldBuffSeid();

            foreach (BuffData data in buffDatas)
            {
                InjectBuffData(data);
                InjectBuffSeid(data);
            }

            buffDatas.Clear();
        }

        // ======================== 私有方法（按执行顺序排列） ========================

        /// <summary>
        /// 生成实际 ID：baseId + localId。
        /// </summary>
        private static int GenId(int localId)
        {
            return baseId + localId;
        }

        /// <summary>
        /// 从游戏现有的 _BuffJsonData 中读取该 Buff 已挂载的 seid 列表，
        /// 记录到 removeBuffSeid 中供后续清理。
        /// </summary>
        private static void RegisterRemoveBuffSeid(BuffData data)
        {
            JSONObject json = jsonData.instance._BuffJsonData;
            if (!json.HasField(data.realId.Value.ToString())) return;
            
            JSONObject buff = json.GetField(data.realId.Value.ToString());
            if (!buff.HasField("seid")) return;

            foreach (JSONObject seid in buff.GetField("seid").list)
            {
                if (!removeBuffSeid.TryGetValue(seid.I, out HashSet<int> ids))
                {
                    ids = [];
                    removeBuffSeid.Add(seid.I, ids);
                }
                ids.Add(data.realId.Value);
            }
        }

        /// <summary>
        /// 从各 seid 表中移除旧 Buff 的 seid 数据。
        /// </summary>
        private static void RemoveOldBuffSeid()
        {
            foreach (var pair in removeBuffSeid)
            {
                JSONObject seidTable = jsonData.instance.BuffSeidJsonData[pair.Key];
                foreach (int id in pair.Value)
                {
                    seidTable.RemoveField(id.ToString());
                }
            }
            removeBuffSeid.Clear();
        }

        /// <summary>
        /// 构建一个带有默认值的新 Buff JSON 对象。
        /// </summary>
        private static JSONObject BuildNewBuff(BuffData data)
        {
            JSONObject buff = JSONObject.Create(JSONObject.Type.OBJECT);
            buff.AddField("buffid", data.realId.Value);

            foreach (var kvp in defaultData)
            {
                JSONObject result = JSONObjectHelper.ToJSONObject(kvp.Value);
                buff.AddField(kvp.Key, result);
            }
            return buff;
        }

        /// <summary>
        /// 将 BuffData 中的非空字段应用到 JSON 对象上。
        /// </summary>
        private static void ApplyBuffData(JSONObject buff, BuffData data)
        {
            // affix → Affix
            if (data.affix != null)
            {
                JSONObject affix = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (int id in data.affix)
                {
                    affix.Add(id);
                }
                buff.SetField("Affix", affix);
            }
            // buffIcon → BuffIcon
            if (data.buffIcon != null) buff.SetField("BuffIcon", data.buffIcon.Value);
            // buffType → bufftype
            if (data.buffType != null) buff.SetField("bufftype", (int)data.buffType.Value);
            // descr → descr
            if (!string.IsNullOrEmpty(data.descr)) buff.SetField("descr", data.descr);
            // isHide → isHide
            if (data.isHide != null) buff.SetField("isHide", data.isHide.Value ? 1 : 0);
            // loopTime → looptime
            if (data.loopTime != null) buff.SetField("looptime", data.loopTime.Value);
            // name → name
            if (!string.IsNullOrEmpty(data.name)) buff.SetField("name", data.name);
            // removeTrigger → removeTrigger
            if (data.removeTrigger != null) buff.SetField("removeTrigger", (int)data.removeTrigger.Value);
            // script → script
            if (!string.IsNullOrEmpty(data.script)) buff.SetField("script", data.script);
            // seidData → seid
            if (data.seidData != null)
            {
                JSONObject seid = JSONObject.Create(JSONObject.Type.ARRAY);

                foreach (var pair in data.seidData)
                {
                    if (
                        pair.Value != null &&
                        pair.Value.TryGetValue("__registerOnly", out object registerOnly) && 
                        registerOnly is bool flag && flag
                    ) {
                        continue;
                    }

                    object result = Registry.Resolve(pair.Key);
                    if (result is not int id)
                    {
                        Main.Log.LogWarning($"Seid引用无效:{pair.Key}");
                        continue;
                    }

                    seid.Add(id);
                }

                buff.SetField("seid", seid);
            }
            // showOnlyOne → ShowOnlyOne
            if (data.showOnlyOne != null) buff.SetField("ShowOnlyOne", data.showOnlyOne.Value ? 1 : 0);
            // skillEffect → skillEffect
            if (data.skillEffect != null)
            {
                string effect = data.skillEffect.Value == 0
                    ? "fx_Summoner_o"
                    : data.skillEffect.Value.ToString();
                buff.SetField("skillEffect", effect);
            }
            // stackType → BuffType
            if (data.stackType != null) buff.SetField("BuffType", (int)data.stackType.Value);
            // totalTime → totaltime
            if (data.totalTime != null) buff.SetField("totaltime", data.totalTime.Value);
            // trigger → trigger
            if (data.trigger != null) buff.SetField("trigger", (int)data.trigger.Value);
        }

        /// <summary>
        /// 将单个 BuffData 注入到 _BuffJsonData 中。
        /// 若 ID 已存在则更新，否则新建。
        /// </summary>
        private static void InjectBuffData(BuffData data)
        {
            string id = data.realId.Value.ToString();
            
            JSONObject buff;
            if (jsonData.instance._BuffJsonData.HasField(id))
            {
                buff = jsonData.instance._BuffJsonData.GetField(id);
            }
            else
            {
                buff = BuildNewBuff(data);
                jsonData.instance._BuffJsonData.AddField(id, buff);
                jsonData.instance.BuffJsonData.Add(id, buff);
            }

            ApplyBuffData(buff, data);
        }

        /// <summary>
        /// 将 Buff 的 seid 特性数据注入到 BuffSeidJsonData 中。
        /// </summary>
        private static void InjectBuffSeid(BuffData data)
        {
            if (data.seidData == null || data.seidData.Count == 0) return;

            foreach (var seid in data.seidData)
            {
                if (seid.Value == null) continue; 

                object result = Registry.Resolve(seid.Key);
                if (result is not int seidId)
                {
                    Main.Log.LogWarning($"Seid引用无效:{seid.Key}");
                    continue;
                }

                if (seidId < 0 || seidId >= jsonData.instance.BuffSeidJsonData.Length)
                {
                    Main.Log.LogWarning($"Seid索引越界:{seidId}");
                    continue;
                }

                JSONObject seidTable = jsonData.instance.BuffSeidJsonData[seidId];
                if (seidTable == null)
                {
                    Main.Log.LogWarning($"BuffSeid未初始化:{seidId}");
                    continue;
                }

                JSONObject seidJson = JSONObject.Create(JSONObject.Type.OBJECT);
                seidJson.AddField("id", data.realId.Value);
                foreach (var kpv in seid.Value)
                {
                    if (kpv.Key == "id") continue;
                    if (kpv.Key == "__registerOnly") continue;

                    seidJson.AddField(
                        kpv.Key,
                        JSONObjectHelper.ToJSONObject(Registry.Resolve(kpv.Value))
                    );
                }

                seidTable.SetField(
                    data.realId.Value.ToString(),
                    seidJson
                );
            }
        }
    }
}