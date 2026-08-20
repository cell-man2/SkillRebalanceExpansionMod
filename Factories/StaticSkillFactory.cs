using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SkillRebalanceExpansionMod.Models.Item;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class StaticSkillFactory
    {
        private const int baseId = 134700;
        private const int baseSkillId = 24530;
        private static readonly Dictionary<string, object> defaultData = new()
        {
            { "qingjiaotype", 1 },
            { "icon", 0 },
            { "Skill_LV", 1 },
            { "typePinJie", 1 },
            { "Skill_castTime", 0 },
            { "Skill_Speed", 0 },
            { "DF", 0 },
            { "TuJianType", 0 }
        };
        private static readonly Dictionary<int, HashSet<int>> removeStaticSkillSeid = [];
        private static readonly List<StaticSkillInstanceData> staticSkillInstanceDatas = [];

        private static long ProductSeries(int x, int y, int t)
        {
            long result = 1;
            for (int i = 0; i <= t; i++)
            {
                result *= x + i * y;
            }
            return result;
        }

        private static int CalculateSkillSpeed(
            int skillLv,
            SkillPin skillPin,
            SkillJie skillJie,
            SkillStyle skillStyle
        ) {
            return (int)(
                100 * skillLv * Math.Pow(3, (int)skillJie - 1) *
                (1 + 0.2 * ((int)skillPin - 2)) *
                (1 + 0.2 * ((int)skillStyle - 2))
            );
        }

        private static int CalculateSkillCastTime(
            int skillLv,
            SkillPin skillPin,
            SkillJie skillJie
        ) {
            if (skillLv == 1) return 1;

            double part1 = ProductSeries(6, -1, (int)skillPin - 1);
            double part2 =
                ProductSeries(15, 1, (int)skillPin - 2) *
                (Math.Pow((int)skillJie, 2) + 5 * (int)skillJie- 14) /
                (Math.Pow(4, (int)skillPin - 1) * 5);

            return (int)(Math.Pow(2, skillLv - 2) * Math.Round(part1 + part2));
        }

        public static void Initialize()
        {
            removeStaticSkillSeid.Clear();
            staticSkillInstanceDatas.Clear();

            BuildStaticSkillIndex();

            List<StaticSkillData> staticSkillDatas = DataManager.Scan<StaticSkillData>(DataCategory.StaticSkill);
            staticSkillDatas.Sort((a, b) =>
            {
                bool aReal = a.realId.HasValue;
                bool bReal = b.realId.HasValue;

                if (aReal != bReal) return aReal ? -1 : 1;
                if (aReal) return a.realId.Value.CompareTo(b.realId.Value);
                return (a.localId ?? int.MaxValue).CompareTo(b.localId ?? int.MaxValue);
            });

            int currentId = baseId;
            foreach (StaticSkillData data in staticSkillDatas)
            {
                int? skillId = null;
                int? jie = null;
                int? pin = null;

                List<StaticSkillInstanceData> instances = ExpandStaticSkillData(data, ref currentId);

                foreach (StaticSkillInstanceData instance in instances)
                {
                    if (skillId == null) skillId = instance.skillId;
                    if (jie == null && instance.skillJie.HasValue) jie = (int)instance.skillJie;
                    if (pin == null && instance.skillPin.HasValue) pin = (int)instance.skillPin;

                    staticSkillInstanceDatas.Add(instance);

                    if (!instance.isNew && instance.seidData != null)
                    {
                        RegisterRemoveStaticSkillSeid(instance);
                    }

                    if (string.IsNullOrEmpty(instance.key)) continue;
                    if (Registry.staticSkill.ContainsKey(instance.key))
                    {
                        Main.Log.LogWarning($"StaticSkill Key重复:{instance.key}, id:{instance.id}");
                    }

                    Registry.staticSkill.Add(instance.key, instance.id);
                }

                if (skillId.HasValue)
                {
                    if (Registry.ssId.TryGetValue(data.key, out int oldId))
                    {
                        if (oldId != skillId)
                        {
                            Main.Log.LogWarning($"BookInfo Key重复:{data.key}, 冲突id:{skillId}与{oldId}");
                        }
                    }
                    else
                    {
                        Registry.ssId.Add(data.key, skillId.Value);
                    }

                    if (Registry.bookInfo.TryGetValue(data.key, out BookInfo oldInfo))
                    {
                        if (oldInfo.skillId != skillId)
                        {
                            Main.Log.LogWarning($"BookInfo Key重复:{data.key}, 冲突id:{skillId}与{oldInfo.skillId}");
                        }
                    }
                    else
                    {
                        Registry.bookInfo.Add(data.key, new BookInfo
                        {
                            type = BookType.功法,
                            skillId = skillId.Value,
                            jie = jie,
                            pin = pin
                        });
                    }
                }
            }
        }

        public static void Inject()
        {
            RemoveOldStaticSkillSeid();
            foreach (StaticSkillInstanceData data in staticSkillInstanceDatas)
            {
                InjectStaticSkillData(data);
                InjectStaticSkillSeid(data);
            }
            staticSkillInstanceDatas.Clear();
        }

        private static int GenStaticSkillId(int localId)
        {
            return baseSkillId + localId;
        }

        private static void BuildStaticSkillIndex()
        {
            foreach (JSONObject json in jsonData.instance.StaticSkillJsonData.list)
            {
                int skillId = json["Skill_ID"].I;
                int skillLv = json["Skill_Lv"].I;
                int id = json["id"].I;

                Registry.staticSkillIndex[(skillId, skillLv)] = id;
            }
        }

        private static List<StaticSkillInstanceData> ExpandStaticSkillData(StaticSkillData data, ref int curr)
        {
            List<StaticSkillInstanceData> result = [];
            if (data.skillLv == null || data.skillLv.Count == 0) return result;

            int skillId;
            Dictionary<int, bool> isNew = data.skillLv.ToDictionary(lv => lv, lv => true);
            if (data.realId.HasValue)
            {
                skillId = data.realId.Value;
                foreach (int tier in data.skillLv)
                {
                    isNew[tier] = !Registry.staticSkillIndex.ContainsKey((skillId, tier));
                }
            }
            else
            {
                skillId = GenStaticSkillId(data.localId.Value);
            }

            foreach (int lv in data.skillLv)
            {
                StaticSkillInstanceData parseData = new()
                {
                    isNew = isNew[lv],
                    key = string.IsNullOrEmpty(data.key)
                        ? string.Empty
                        : $"{data.key}{lv}",
                    id = isNew[lv]
                        ? ++curr
                        : Registry.staticSkillIndex[(skillId, lv)],
                    skillId = skillId,
                    skillLv = lv,
                    name = data.name != null && data.name.TryGetValue(lv, out var nameVal)
                        ? nameVal : null,
                    qingJiaoType = data.qingJiaoType != null && data.qingJiaoType.TryGetValue(lv, out var qingJiaoVal)
                        ? qingJiaoVal : null,
                    affix = data.affix != null && data.affix.TryGetValue(lv, out var affixVal)
                        ? affixVal : null,
                    seidData = data.seidData != null && data.seidData.TryGetValue(lv, out var seidVal)
                        ? seidVal : null,
                    descr = data.descr != null && data.descr.TryGetValue(lv, out var descrVal)
                        ? descrVal : null,
                    attackType = data.attackType != null && data.attackType.TryGetValue(lv, out var attackVal)
                        ? attackVal : null,
                    icon = data.icon != null && data.icon.TryGetValue(lv, out var iconVal)
                        ? iconVal : null,
                    skillStyle = data.skillStyle != null && data.skillStyle.TryGetValue(lv, out var styleVal)
                        ? styleVal : null,
                    skillJie = data.skillJie != null && data.skillJie.TryGetValue(lv, out var jieVal)
                        ? jieVal : null,
                    skillPin = data.skillPin != null && data.skillPin.TryGetValue(lv, out var pinVal)
                        ? pinVal : null,
                    tuJianDescr = data.tuJianDescr != null && data.tuJianDescr.TryGetValue(lv, out var tuJianVal)
                        ? tuJianVal : null,
                    skillCastTime = data.skillCastTime != null && data.skillCastTime.TryGetValue(lv, out var castVal)
                        ? castVal : null,
                    skillSpeed = data.skillSpeed != null && data.skillSpeed.TryGetValue(lv, out var speedVal)
                        ? speedVal : null,
                    df = data.df != null && data.df.TryGetValue(lv, out var dfVal)
                        ? dfVal : null,
                    tuJianType = data.tuJianType != null && data.tuJianType.TryGetValue(lv, out var tuJianTypeVal)
                        ? tuJianTypeVal : null
                };
                result.Add(parseData);
            }
            return result;
        }

        private static void RegisterRemoveStaticSkillSeid(StaticSkillInstanceData data)
        {
            JSONObject staticSkill = jsonData.instance.StaticSkillJsonData.GetField(data.id.ToString());

            if (!staticSkill.HasField("seid")) return;
            foreach (JSONObject seid in staticSkill.GetField("seid").list)
            {
                if (!removeStaticSkillSeid.TryGetValue(seid.I, out HashSet<int> ids))
                {
                    ids = [];
                    removeStaticSkillSeid.Add(seid.I, ids);
                }
                ids.Add(data.id);
            }
        }

        private static void RemoveOldStaticSkillSeid()
        {
            foreach (var pair in removeStaticSkillSeid)
            {
                JSONObject seidTable = jsonData.instance.StaticSkillSeidJsonData[pair.Key];
                foreach (int id in pair.Value)
                {
                    seidTable.RemoveField(id.ToString());
                }
            }
            removeStaticSkillSeid.Clear();
        }

        private static JSONObject BuildNewStaticSkill(StaticSkillInstanceData data)
        {
            JSONObject staticSkill = JSONObject.Create(JSONObject.Type.OBJECT);

            staticSkill.AddField("id", data.id);
            staticSkill.AddField("Skill_ID", data.skillId);
            staticSkill.AddField("Skill_Lv", data.skillLv);

            foreach (var kvp in defaultData)
            {
                JSONObject result = JSONObjectHelper.ToJSONObject(kvp.Value);
                staticSkill.AddField(kvp.Key, result);
            }

            return staticSkill;
        }

        private static void InitNewStaticSkill(
            JSONObject staticSkill,
            StaticSkillInstanceData data
        ) {
            if (data.skillPin.HasValue &&
                data.skillJie.HasValue &&
                data.skillStyle.HasValue)
            {
                staticSkill.SetField(
                    "Skill_Speed",
                    CalculateSkillSpeed(
                        data.skillLv,
                        data.skillPin.Value,
                        data.skillJie.Value,
                        data.skillStyle.Value
                    )
                );

                staticSkill.SetField(
                    "Skill_castTime",
                    CalculateSkillCastTime(
                        data.skillLv,
                        data.skillPin.Value,
                        data.skillJie.Value
                    )
                );
            }

            if (!string.IsNullOrEmpty(data.descr))
            {
                if (data.affix == null)
                {
                    staticSkill.SetField(
                        "Affix",
                        JSONObjectHelper.ToJSONObject(AffixProcessor.ExtractAffix(data.descr))
                    );
                }

                if (string.IsNullOrEmpty(data.tuJianDescr))
                {
                    staticSkill.SetField(
                        "TuJiandescr",
                        AffixProcessor.FormatTuJian(data.descr)
                    );
                }
            }
        }

        private static void ApplyStaticSkillData(JSONObject staticSkill, StaticSkillInstanceData data)
        {
            // 功法名称
            if (!string.IsNullOrEmpty(data.name)) staticSkill.SetField("name", data.name);
            // 请教类型
            if (data.qingJiaoType.HasValue) staticSkill.SetField("qingjiaotype", (int)data.qingJiaoType.Value);
            // 功法属性
            if (data.attackType.HasValue) staticSkill.SetField("AttackType", (int)data.attackType.Value);
            // 功法图标
            if (data.icon.HasValue) staticSkill.SetField("icon", data.icon.Value);
            // 功法品级（上中下）
            if (data.skillJie.HasValue) staticSkill.SetField("Skill_LV", (int)data.skillJie.Value);
            // 功法阶级（天地人）
            if (data.skillPin.HasValue) staticSkill.SetField("typePinJie", (int)data.skillPin.Value);
            // 突破用时
            if (data.skillCastTime.HasValue) staticSkill.SetField("Skill_castTime", data.skillCastTime.Value);
            // 修炼速度
            if (data.skillSpeed.HasValue) staticSkill.SetField("Skill_Speed", data.skillSpeed.Value);
            // 功法描述
            if (!string.IsNullOrEmpty(data.descr)) staticSkill.SetField("descr", data.descr);
            // 功法图鉴描述
            if (!string.IsNullOrEmpty(data.tuJianDescr)) staticSkill.SetField("TuJiandescr", data.tuJianDescr);
            // 功法图鉴类型
            if (data.tuJianType.HasValue) staticSkill.SetField("TuJianType", (int)data.tuJianType.Value);
            // 神仙斗法
            if (data.df.HasValue) staticSkill.SetField("DF", data.df.Value ? 1 : 0);
            // 词缀
            if (data.affix != null)
            {
                staticSkill.SetField(
                    "Affix",
                    JSONObjectHelper.ToJSONObject(data.affix)
                );
            }
            // 功法特性seid
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
                staticSkill.SetField("seid", seid);
            }
        }

        private static void InjectStaticSkillData(StaticSkillInstanceData data)
        {
            string id = data.id.ToString();

            JSONObject staticSkill;
            if (data.isNew)
            {
                staticSkill = BuildNewStaticSkill(data);
                jsonData.instance.StaticSkillJsonData.AddField(id, staticSkill);

                InitNewStaticSkill(staticSkill, data);
            }
            else
            {
                staticSkill = jsonData.instance.StaticSkillJsonData[id];
            }

            ApplyStaticSkillData(staticSkill, data);
        }

        private static void InjectStaticSkillSeid(StaticSkillInstanceData data)
        {
            if (data.seidData == null || data.seidData.Count == 0) return;
            
            foreach (var seid in data.seidData)
            {
                object result = Registry.Resolve(seid.Key);

                if (result is not int seidId)
                {
                    Main.Log.LogWarning($"Seid引用无效:{seid.Key}");
                    continue;
                }

                if (seidId < 0 || seidId >= jsonData.instance.StaticSkillSeidJsonData.Length)
                {
                    Main.Log.LogWarning($"StaticSkillSeid索引越界:{seidId}");
                    continue;
                }

                JSONObject seidTable = jsonData.instance.StaticSkillSeidJsonData[seidId];
                if (seidTable == null)
                {
                    Main.Log.LogWarning($"StaticSkillSeid未初始化:{seidId}");
                    continue;
                }

                JSONObject seidJson = JSONObject.Create(JSONObject.Type.OBJECT);

                seidJson.AddField("skillid", data.id);
                foreach (var kpv in seid.Value)
                {
                    if (kpv.Key == "skillid") continue;

                    seidJson.AddField(
                        kpv.Key,
                        JSONObjectHelper.ToJSONObject(Registry.Resolve(kpv.Value))
                    );
                }

                seidTable.SetField(data.id.ToString(), seidJson);
            }
        }
    
    }
}