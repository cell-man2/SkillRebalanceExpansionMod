using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class BuffFactory
    {
        private const int baseId = 175240;
        private static readonly Dictionary<string, object> defaultData = new Dictionary<string, object>
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
        private static readonly Dictionary<int, HashSet<int>> removeBuffSeid = [];
        private static readonly List<BuffData> buffDatas = [];

        public static void Initialize()
        {
            buffDatas.Clear();

            List<BuffData> datas = DataManager.Scan<BuffData>(DataCategory.Buff);
            buffDatas.AddRange(datas);

            foreach (BuffData data in buffDatas)
            {
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

                if (data.realId.HasValue && data.seidData != null)
                {
                    RegisterRemoveBuffSeid(data);
                }
            }
        }

        public static void Inject()
        {
            RemoveOldBuffSeid();
            foreach (BuffData data in buffDatas)
            {
                InjectBuffData(data);
                InjectBuffSeid(data);
            }
        }

        private static int GenId(int localId)
        {
            return baseId + localId;
        }

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

        private static JSONObject BuildNewBuff(BuffData data)
        {
            // 新建buff JSONObject对象
            JSONObject buff = JSONObject.Create(JSONObject.Type.OBJECT);
            buff.AddField("buffid", data.realId.Value);

            // 应用默认值
            foreach (var kvp in defaultData)
            {
                JSONObject result = JSONObjectHelper.ToJSONObject(kvp.Value);
                buff.AddField(kvp.Key, result);
            }
            return buff;
        }

        private static void ApplyBuffData(JSONObject buff, BuffData data)
        {
            // buff图标
            if (data.buffIcon != null) buff.SetField("BuffIcon", data.buffIcon.Value);
            // buff名称
            if (!string.IsNullOrEmpty(data.name)) buff.SetField("name", data.name);
            // buff描述
            if (!string.IsNullOrEmpty(data.descr)) buff.SetField("descr", data.descr);
            // 触发时机
            if (data.trigger != null) buff.SetField("trigger", (int)data.trigger.Value);
            // 移除方式
            if (data.removeTrigger != null) buff.SetField("removeTrigger", (int)data.removeTrigger.Value);
            // buff类型
            if (!string.IsNullOrEmpty(data.script)) buff.SetField("script", data.script);
            // buff循环时间
            if (data.loopTime != null) buff.SetField("looptime", data.loopTime.Value);
            // buff持续时间
            if (data.totalTime != null) buff.SetField("totaltime", data.totalTime.Value);
            // buff叠加类型
            if (data.stackType != null) buff.SetField("BuffType", (int)data.stackType.Value);
            // 是否隐藏
            if (data.isHide != null) buff.SetField("isHide", data.isHide.Value ? 1 : 0);
            // 是否只显示一层
            if (data.showOnlyOne != null)  buff.SetField("ShowOnlyOne", data.showOnlyOne.Value ? 1 : 0);
            // buff类型
            if (data.buffType != null) buff.SetField("bufftype", (int)data.buffType.Value);
            // 技能特效
            if (data.skillEffect != null)
            {
                string effect = data.skillEffect.Value == 0
                    ? "fx_Summoner_o"
                    : data.skillEffect.Value.ToString();
                buff.SetField("skillEffect", effect);
            }
            // 词缀
            if (data.affix != null)
            {
                JSONObject affix = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (int id in data.affix)
                {
                    affix.Add(id);
                }
                buff.SetField("Affix", affix);
            }
            // buff特性id
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
                buff.SetField("seid", seid);
            }
        }

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