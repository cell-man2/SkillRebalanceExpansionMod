using System;
using System.Collections.Generic;
using System.Linq;
using SkillRebalanceExpansionMod.Models.Item;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Factories
{
    /// <summary>
    /// 功法工厂，负责扫描所有标记为 DataCategory.StaticSkill 的数据类，
    /// 将其展开为各等级实例后注入到游戏的功法 JSON 数据中。
    /// </summary>
    public static class StaticSkillFactory
    {
        // ======================== 常量 ========================

        /// <summary>
        /// 新功法实例 ID 起始基数，用于分配 StaticSkillJsonData 中的 id。
        /// </summary>
        private const int baseId = 134700;

        /// <summary>
        /// 技能编号基数，localId 最终偏移为 baseSkillId + localId。
        /// 此 ID 是功法编号（Skill_ID），而非实例 ID（id）。
        /// </summary>
        private const int baseSkillId = 24530;

        // ======================== 静态字段 ========================

        /// <summary>
        /// 新建功法时的默认字段值。
        /// </summary>
        private static readonly Dictionary<string, object> defaultData = new()
        {
            { "qingjiaotype", 1 },
            { "icon", 0 },
            { "Skill_LV", 1 },
            { "typePinJie", 1 },
            { "DF", 0 },
            { "TuJianType", 0 }
        };

        /// <summary>
        /// 记录每个 seid 编号下挂载了哪些功法实例 ID。
        /// Key: seid 编号, Value: 使用该 seid 的功法实例 ID 集合。
        /// 用于注入前清理旧数据，防止残留。
        /// </summary>
        private static readonly Dictionary<int, HashSet<int>> removeStaticSkillSeid = [];

        /// <summary>
        /// 缓存所有展开后的功法实例数据，供 Inject 阶段使用。
        /// </summary>
        private static readonly List<StaticSkillInstanceData> staticSkillInstanceDatas = [];

        // ======================== 工具函数 ========================

        /// <summary>
        /// 计算阶乘形式的级数乘积：x * (x+y) * (x+2y) * ... * (x+t*y)
        /// 用于参悟时间计算。
        /// </summary>
        private static long ProductSeries(int x, int y, int t)
        {
            long result = 1;
            for (int i = 0; i <= t; i++)
            {
                result *= x + i * y;
            }
            return result;
        }

        /// <summary>
        /// 计算修炼速度（Skill_Speed）。
        /// 基于等级、品级、阶级、功法类型综合计算。
        /// </summary>
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

        /// <summary>
        /// 计算参悟时间（Skill_castTime）。
        /// 基于等级、品级、阶级综合计算，等级 1 固定为 1。
        /// </summary>
        private static int CalculateSkillCastTime(
            int skillLv,
            SkillPin skillPin,
            SkillJie skillJie
        ) {
            if (skillLv == 1) return 1;

            double part1 = ProductSeries(6, -1, (int)skillPin - 1);
            double part2 =
                ProductSeries(15, 1, (int)skillPin - 2) *
                (Math.Pow((int)skillJie, 2) + 5 * (int)skillJie - 14) /
                (Math.Pow(4, (int)skillPin - 1) * 5);

            return (int)(Math.Pow(2, skillLv - 2) * Math.Round(part1 + part2));
        }

        // ======================== 公开方法 ========================

        /// <summary>
        /// 初始化阶段：扫描所有功法包数据，展开为实例并注册到 Registry。
        /// </summary>
        public static void Initialize()
        {
            removeStaticSkillSeid.Clear();
            staticSkillInstanceDatas.Clear();

            // 构建 (Skill_ID, Skill_Lv) → id 索引，用于判断实例是否已存在
            BuildStaticSkillIndex();

            List<StaticSkillData> staticSkillDatas = DataManager.Scan<StaticSkillData>(DataCategory.StaticSkill);
            // 排序确保 realId 优先，保证 ID 稳定性
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

                // 展开为多个等级实例
                List<StaticSkillInstanceData> instances = ExpandStaticSkillData(data, ref currentId);

                foreach (StaticSkillInstanceData instance in instances)
                {
                    // 记录 skillId、jie、pin 用于注册 BookInfo
                    if (skillId == null) skillId = instance.skillId;
                    if (jie == null && instance.skillJie.HasValue) jie = (int)instance.skillJie;
                    if (pin == null && instance.skillPin.HasValue) pin = (int)instance.skillPin;

                    staticSkillInstanceDatas.Add(instance);

                    // 已存在的实例需要收集旧 seid 供清理
                    if (!instance.isNew && instance.seidData != null)
                    {
                        RegisterRemoveStaticSkillSeid(instance);
                    }

                    // 注册实例 key → id（用于 "@staticSkill:xxx" 引用）
                    if (string.IsNullOrEmpty(instance.key)) continue;
                    if (Registry.staticSkill.ContainsKey(instance.key))
                    {
                        Main.Log.LogWarning($"StaticSkill Key重复:{instance.key}, id:{instance.id}");
                    }

                    Registry.staticSkill.Add(instance.key, instance.id);
                }

                // 注册包数据 key → skillId（用于 "@ssId:xxx" 和 "@bookInfo:xxx" 引用）
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

        /// <summary>
        /// 注入阶段：将功法实例数据写入游戏 JSON。
        /// </summary>
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

        // ======================== 私有方法（按执行顺序排列） ========================

        /// <summary>
        /// 生成技能编号（Skill_ID）：baseSkillId + localId。
        /// </summary>
        private static int GenStaticSkillId(int localId)
        {
            return baseSkillId + localId;
        }

        /// <summary>
        /// 从游戏现有的 StaticSkillJsonData 中构建 (Skill_ID, Skill_Lv) → id 索引。
        /// 用于判断某个等级的功法是否已存在。
        /// </summary>
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

        /// <summary>
        /// 将 StaticSkillData 展开为多个 StaticSkillInstanceData。
        /// 每个 skillLv 对应一个实例。
        /// 新实例分配递增 id，已存在实例复用旧 id。
        /// </summary>
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
                    // 实例 key = 包 key + 等级数字
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

        /// <summary>
        /// 从游戏现有的 StaticSkillJsonData 中读取该功法实例已挂载的 seid 列表，
        /// 记录到 removeStaticSkillSeid 中供后续清理。
        /// </summary>
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

        /// <summary>
        /// 从各 seid 表中移除旧功法实例的 seid 数据。
        /// </summary>
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

        /// <summary>
        /// 构建一个带有默认值的新功法 JSON 对象。
        /// </summary>
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

        /// <summary>
        /// 初始化新功法实例：计算修炼速度、参悟时间，
        /// 以及从描述中提取词缀和图鉴描述（若未单独指定）。
        /// 仅在新建实例时调用。
        /// </summary>
        private static void InitNewStaticSkill(
            JSONObject staticSkill,
            StaticSkillInstanceData data
        ) {
            // 根据品级、阶级、类型自动计算修炼速度和参悟时间
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

            // 从描述中自动提取词缀和图鉴描述（若未单独指定）
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

        /// <summary>
        /// 将 StaticSkillInstanceData 中的非空字段应用到 JSON 对象上。
        /// </summary>
        private static void ApplyStaticSkillData(JSONObject staticSkill, StaticSkillInstanceData data)
        {
            // name → name
            if (!string.IsNullOrEmpty(data.name)) staticSkill.SetField("name", data.name);
            // qingJiaoType → qingjiaotype
            if (data.qingJiaoType.HasValue) staticSkill.SetField("qingjiaotype", (int)data.qingJiaoType.Value);
            // attackType → AttackType
            if (data.attackType.HasValue) staticSkill.SetField("AttackType", (int)data.attackType.Value);
            // icon → icon
            if (data.icon.HasValue) staticSkill.SetField("icon", data.icon.Value);
            // skillJie → Skill_LV
            if (data.skillJie.HasValue) staticSkill.SetField("Skill_LV", (int)data.skillJie.Value);
            // skillPin → typePinJie
            if (data.skillPin.HasValue) staticSkill.SetField("typePinJie", (int)data.skillPin.Value);
            // skillCastTime → Skill_castTime
            if (data.skillCastTime.HasValue) staticSkill.SetField("Skill_castTime", data.skillCastTime.Value);
            // skillSpeed → Skill_Speed
            if (data.skillSpeed.HasValue) staticSkill.SetField("Skill_Speed", data.skillSpeed.Value);
            // descr → descr
            if (!string.IsNullOrEmpty(data.descr)) staticSkill.SetField("descr", data.descr);
            // tuJianDescr → TuJiandescr
            if (!string.IsNullOrEmpty(data.tuJianDescr)) staticSkill.SetField("TuJiandescr", data.tuJianDescr);
            // tuJianType → TuJianType
            if (data.tuJianType.HasValue) staticSkill.SetField("TuJianType", (int)data.tuJianType.Value);
            // df → DF
            if (data.df.HasValue) staticSkill.SetField("DF", data.df.Value ? 1 : 0);
            // affix → Affix
            if (data.affix != null)
            {
                staticSkill.SetField(
                    "Affix",
                    JSONObjectHelper.ToJSONObject(data.affix)
                );
            }
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
                staticSkill.SetField("seid", seid);
            }
        }

        /// <summary>
        /// 将单个功法实例注入到 StaticSkillJsonData 中。
        /// 若 isNew 为 true 则新建，否则更新已有对象。
        /// </summary>
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

        /// <summary>
        /// 将功法实例的 seid 特性数据注入到 StaticSkillSeidJsonData 中。
        /// </summary>
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