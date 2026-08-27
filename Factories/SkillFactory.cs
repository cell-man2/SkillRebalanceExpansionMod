using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Skill;
using SkillRebalanceExpansionMod.Models.Item;
using SkillRebalanceExpansionMod.Utils;
using System.Linq;

namespace SkillRebalanceExpansionMod.Factories
{
    /// <summary>
    /// 神通工厂，负责扫描所有标记为 DataCategory.Skill 的数据类，
    /// 将其展开为各等级实例后注入到游戏的神通 JSON 数据中。
    /// 与功法工厂类似，但额外处理 AI 行为数据（敌人出手权）。
    /// </summary>
    public static class SkillFactory
    {
        // ======================== 常量 ========================

        /// <summary>
        /// 新神通实例 ID 起始基数，用于分配 _skillJsonData 中的 id。
        /// </summary>
        private const int baseId = 41750;

        /// <summary>
        /// 技能编号基数，localId 最终偏移为 baseSkillId + localId。
        /// 此 ID 是技能编号（Skill_ID），而非实例 ID（id）。
        /// </summary>
        private const int baseSkillId = 4370;

        // ======================== 静态字段 ========================

        /// <summary>
        /// 新建神通时的默认字段值。
        /// </summary>
        private static readonly Dictionary<string, object> defaultData = new()
        {
            { "Affix", new List<int>() },
            { "canUseDistMax", 30 },
            { "CD", 10000f },
            { "DF", 0 },
            { "HP", 0 },
            { "icon", 0 },
            { "qingjiaotype", 1 },
            { "Skill_castTime", 1 },
            { "Skill_DisplayType", 0 },
            { "Skill_LV", 1 },
            { "Skill_Open", 1 },
            { "Skill_Type", 20 },
            { "speed", 0 },
            { "TuJianType", 0 },
            { "typePinJie", 1 }
        };

        /// <summary>
        /// 记录每个 seid 编号下挂载了哪些神通实例 ID。
        /// Key: seid 编号, Value: 使用该 seid 的神通实例 ID 集合。
        /// 用于注入前清理旧数据，防止残留。
        /// </summary>
        private static readonly Dictionary<int, HashSet<int>> removeSkillSeid = [];

        /// <summary>
        /// 记录每个 AI 编号下挂载了哪些神通实例 ID。
        /// Key: AI 编号, Value: 挂载在该 AI 下的神通实例 ID 集合。
        /// 用于注入前清理旧 AI 数据（需全量遍历 AIJsonDate 收集）。
        /// </summary>
        private static readonly Dictionary<int, HashSet<int>> removeSkillAI = [];

        /// <summary>
        /// 缓存所有展开后的神通实例数据，供 Inject 阶段使用。
        /// </summary>
        private static readonly List<SkillInstanceData> skillInstanceDatas = [];

        // ======================== 公开方法 ========================

        /// <summary>
        /// 初始化阶段：扫描所有神通包数据，展开为实例并注册到 Registry。
        /// </summary>
        public static void Initialize()
        {
            removeSkillSeid.Clear();
            removeSkillAI.Clear();
            skillInstanceDatas.Clear();

            // 构建 (Skill_ID, Skill_Lv) → id 索引，用于判断实例是否已存在
            BuildSkillIndex();

            List<SkillData> skillDatas = DataManager.Scan<SkillData>(DataCategory.Skill);
            // 排序确保 realId 优先，保证 ID 稳定性
            skillDatas.Sort((a, b) =>
            {
                bool aReal = a.realId.HasValue;
                bool bReal = b.realId.HasValue;

                if (aReal != bReal) return aReal ? -1 : 1;
                if (aReal) return a.realId.Value.CompareTo(b.realId.Value);
                return (a.localId ?? int.MaxValue).CompareTo(b.localId ?? int.MaxValue);
            });

            int currentId = baseId;
            foreach (SkillData data in skillDatas)
            {
                int? skillId = null;
                int? jie = null;
                int? pin = null;

                // 展开为多个等级实例
                List<SkillInstanceData> instances = ExpandSkillData(data, ref currentId);

                foreach (SkillInstanceData instance in instances)
                {
                    // 记录 skillId、jie、pin 用于注册 BookInfo
                    if (skillId == null) skillId = instance.skillId;
                    if (jie == null && instance.skillJie.HasValue) jie = (int)instance.skillJie;
                    if (pin == null && instance.skillPin.HasValue) pin = (int)instance.skillPin;

                    skillInstanceDatas.Add(instance);

                    // 已存在的实例需要收集旧 seid 和旧 AI 数据供清理
                    if (!instance.isNew)
                    {
                        if (instance.seidData != null) RegisterRemoveSkillSeid(instance);
                        if (instance.aiData != null) RegisterRemoveSkillAI(instance);
                    }

                    // 注册实例 key → id（用于 "@skill:xxx" 引用）
                    if (string.IsNullOrEmpty(instance.key)) continue;
                    if (Registry.skill.ContainsKey(instance.key))
                    {
                        Main.Log.LogWarning($"Skill Key重复:{instance.key}, id:{instance.id}");
                        continue;
                    }

                    Registry.skill.Add(instance.key, instance.id);
                }

                // 注册包数据 key → skillId（用于 "@sId:xxx" 和 "@bookInfo:xxx" 引用）
                if (skillId.HasValue)
                {
                    if (Registry.sId.TryGetValue(data.key, out int oldId))
                    {
                        if (oldId != skillId.Value)
                        {
                            Main.Log.LogWarning($"BookInfo Key重复:{data.key}, 冲突id:{skillId.Value}与{oldId}");
                        }
                    }
                    else
                    {
                        Registry.sId.Add(data.key, skillId.Value);
                    }

                    if (Registry.bookInfo.TryGetValue(data.key, out BookInfo oldInfo))
                    {
                        if (oldInfo.skillId != skillId.Value)
                        {
                            Main.Log.LogWarning($"BookInfo Key重复:{data.key}, 冲突id:{skillId.Value}与{oldInfo.skillId}");
                        }
                    }
                    else
                    {
                        Registry.bookInfo.Add(
                            data.key,
                            new BookInfo
                            {
                                type = BookType.神通,
                                skillId = skillId.Value,
                                jie = jie,
                                pin = pin
                            }
                        );
                    }
                }
            }
        }

        /// <summary>
        /// 注入阶段：将神通实例数据写入游戏 JSON。
        /// </summary>
        public static void Inject()
        {
            RemoveOldSkillSeid();
            RemoveOldSkillAI();
            foreach (SkillInstanceData data in skillInstanceDatas)
            {
                InjectSkillData(data);
                InjectSkillSeid(data);
                InjectSkillAI(data);
            }
            skillInstanceDatas.Clear();
        }

        // ======================== 私有方法（按执行顺序排列） ========================

        /// <summary>
        /// 生成技能编号（Skill_ID）：baseSkillId + localId。
        /// </summary>
        private static int GenSkillId(int localId)
        {
            return baseSkillId + localId;
        }

        /// <summary>
        /// 从游戏现有的 _skillJsonData 中构建 (Skill_ID, Skill_Lv) → id 索引。
        /// 用于判断某个等级的神通是否已存在。
        /// </summary>
        private static void BuildSkillIndex()
        {
            foreach (JSONObject json in jsonData.instance._skillJsonData.list)
            {
                int skillId = json["Skill_ID"].I;
                int skillLv = json["Skill_Lv"].I;
                int id = json["id"].I;

                Registry.skillIndex[(skillId, skillLv)] = id;
            }
        }

        /// <summary>
        /// 将 SkillData 展开为多个 SkillInstanceData。
        /// 每个 skillLv 对应一个实例。
        /// 新实例分配递增 id，已存在实例复用旧 id。
        /// </summary>
        private static List<SkillInstanceData> ExpandSkillData(SkillData data, ref int curr)
        {
            List<SkillInstanceData> result = [];

            if (data.skillLv == null || data.skillLv.Count == 0) return result;

            int skillId;
            Dictionary<int, bool> isNew = data.skillLv.ToDictionary(lv => lv, lv => true);
            if (data.realId.HasValue)
            {
                skillId = data.realId.Value;
                foreach (int tier in data.skillLv)
                {
                    isNew[tier] = !Registry.skillIndex.ContainsKey((skillId, tier));
                }
            }
            else
            {
                skillId = GenSkillId(data.localId.Value);
            }

            foreach (int lv in data.skillLv)
            {
                SkillInstanceData parseData = new()
                {
                    isNew = isNew[lv],
                    // 实例 key = 包 key + 等级数字
                    key = string.IsNullOrEmpty(data.key)
                        ? string.Empty
                        : $"{data.key}{lv}",
                    id = isNew[lv]
                        ? ++curr
                        : Registry.skillIndex[(skillId, lv)],
                    skillId = skillId,
                    skillLv = lv,
                    name = data.name != null && data.name.TryGetValue(lv, out var nameVal)
                        ? nameVal : null,
                    qingJiaoType = data.qingJiaoType != null && data.qingJiaoType.TryGetValue(lv, out var qingJiaoVal)
                        ? qingJiaoVal : null,
                    skillEffect = data.skillEffect != null && data.skillEffect.TryGetValue(lv, out var skillEffectVal)
                        ? skillEffectVal : null,
                    skillType = data.skillType != null && data.skillType.TryGetValue(lv, out var skillTypeVal)
                        ? skillTypeVal : null,
                    seidData = data.seidData != null && data.seidData.TryGetValue(lv, out var seidVal)
                        ? seidVal : null,
                    aiData = data.aiData != null && data.aiData.TryGetValue(lv, out var aiDataVal)
                        ? aiDataVal : null,
                    affix = data.affix != null && data.affix.TryGetValue(lv, out var affixVal)
                        ? affixVal : null,
                    affix2 = data.affix2 != null && data.affix2.TryGetValue(lv, out var affix2Val)
                        ? affix2Val : null,
                    descr = data.descr != null && data.descr.TryGetValue(lv, out var descrVal)
                        ? descrVal : null,
                    tuJianDescr = data.tuJianDescr != null && data.tuJianDescr.TryGetValue(lv, out var tuJianVal)
                        ? tuJianVal : null,
                    attackType = data.attackType != null && data.attackType.TryGetValue(lv, out var attackVal)
                        ? attackVal : null,
                    script = data.script != null && data.script.TryGetValue(lv, out var scriptVal)
                        ? scriptVal : null,
                    hp = data.hp != null && data.hp.TryGetValue(lv, out var hpVal)
                        ? hpVal : null,
                    speed = data.speed != null && data.speed.TryGetValue(lv, out var speedVal)
                        ? speedVal : null,
                    icon = data.icon != null && data.icon.TryGetValue(lv, out var iconVal)
                        ? iconVal : null,
                    skillDisplayType = data.skillDisplayType != null && data.skillDisplayType.TryGetValue(lv, out var skillDisplayVal)
                        ? skillDisplayVal : null,
                    cost = data.cost != null && data.cost.TryGetValue(lv, out var costVal)
                        ? costVal : null,
                    skillJie = data.skillJie != null && data.skillJie.TryGetValue(lv, out var jieVal)
                        ? jieVal : null,
                    skillPin = data.skillPin != null && data.skillPin.TryGetValue(lv, out var pinVal)
                        ? pinVal : null,
                    tuJianType = data.tuJianType != null && data.tuJianType.TryGetValue(lv, out var tuJianTypeVal)
                        ? tuJianTypeVal : null,
                    df = data.df != null && data.df.TryGetValue(lv, out var dfVal)
                        ? dfVal : null,
                    skillOpen = data.skillOpen != null && data.skillOpen.TryGetValue(lv, out var skillOpenVal)
                        ? skillOpenVal : null,
                    skillCastTime = data.skillCastTime != null && data.skillCastTime.TryGetValue(lv, out var castVal)
                        ? castVal : null,
                    canUseDistMax = data.canUseDistMax != null && data.canUseDistMax.TryGetValue(lv, out var distMaxVal)
                        ? distMaxVal : null,
                    cd = data.cd != null && data.cd.TryGetValue(lv, out var cdVal)
                        ? cdVal : null
                };
                result.Add(parseData);
            }
            return result;
        }

        /// <summary>
        /// 从游戏现有的 _skillJsonData 中读取该神通实例已挂载的 seid 列表，
        /// 记录到 removeSkillSeid 中供后续清理。
        /// </summary>
        private static void RegisterRemoveSkillSeid(SkillInstanceData data)
        {
            JSONObject skill = jsonData.instance._skillJsonData.GetField(data.id.ToString());

            if (!skill.HasField("seid")) return;
            foreach (JSONObject seid in skill.GetField("seid").list)
            {
                if (!removeSkillSeid.TryGetValue(seid.I, out HashSet<int> ids))
                {
                    ids = [];
                    removeSkillSeid.Add(seid.I, ids);
                }
                ids.Add(data.id);
            }
        }

        /// <summary>
        /// 遍历所有 AIJsonDate，检查该神通实例 ID 是否挂载在某个 AI 下。
        /// 若有则记录到 removeSkillAI 中供后续清理。
        /// 由于 AI 数据不存储于技能本体，需全量遍历。
        /// </summary>
        private static void RegisterRemoveSkillAI(SkillInstanceData data)
        {
            Dictionary<int, JSONObject> aiJsonData = jsonData.instance.AIJsonDate;
            foreach (int ai in aiJsonData.Keys)
            {
                if (!aiJsonData[ai].HasField(data.id.ToString())) continue;

                if (!removeSkillAI.TryGetValue(ai, out HashSet<int> ids))
                {
                    ids = [];
                    removeSkillAI.Add(ai, ids);
                }
                ids.Add(data.id);
            }
        }

        /// <summary>
        /// 从各 seid 表中移除旧神通实例的 seid 数据。
        /// </summary>
        private static void RemoveOldSkillSeid()
        {
            foreach (var pair in removeSkillSeid)
            {
                JSONObject seidTable = jsonData.instance.SkillSeidJsonData[pair.Key];
                foreach (int id in pair.Value)
                {
                    seidTable.RemoveField(id.ToString());
                }
            }
            removeSkillSeid.Clear();
        }

        /// <summary>
        /// 从各 AI 表中移除旧神通实例的 AI 数据。
        /// </summary>
        private static void RemoveOldSkillAI()
        {
            foreach (var pair in removeSkillAI)
            {
                int ai = pair.Key;

                JSONObject aiTable = jsonData.instance.AIJsonDate[ai];
                foreach (int skillId in pair.Value)
                {
                    aiTable.RemoveField(skillId.ToString());
                }
            }
            removeSkillAI.Clear();
        }

        /// <summary>
        /// 构建一个带有默认值的新神通 JSON 对象。
        /// </summary>
        private static JSONObject BuildNewSkill(SkillInstanceData data)
        {
            JSONObject skill = JSONObject.Create(JSONObject.Type.OBJECT);

            skill.AddField("id", data.id);
            skill.AddField("Skill_ID", data.skillId);
            skill.AddField("Skill_Lv", data.skillLv);

            foreach (var kvp in defaultData)
            {
                JSONObject result = JSONObjectHelper.ToJSONObject(kvp.Value);
                skill.AddField(kvp.Key, result);
            }

            return skill;
        }

        /// <summary>
        /// 初始化新神通实例：从描述中提取词缀和图鉴描述（若未单独指定）。
        /// 仅在新建实例时调用。
        /// </summary>
        private static void InitNewSkill(JSONObject skill, SkillInstanceData data)
        {
            if (string.IsNullOrEmpty(data.descr)) return;

            // 从描述中自动提取词缀（写入 Affix2）
            if (data.affix2 == null)
            {
                skill.SetField(
                    "Affix2",
                    JSONObjectHelper.ToJSONObject(AffixProcessor.ExtractAffix(data.descr))
                );
            }

            // 从描述中自动生成图鉴描述
            if (string.IsNullOrEmpty(data.tuJianDescr))
            {
                skill.SetField(
                    "TuJiandescr",
                    AffixProcessor.FormatTuJian(data.descr)
                );
            }
        }

        /// <summary>
        /// 将 SkillInstanceData 中的非空字段应用到 JSON 对象上。
        /// </summary>
        private static void ApplySkillData(JSONObject skill, SkillInstanceData data)
        {
            // name → name
            if (!string.IsNullOrEmpty(data.name)) skill.SetField("name", data.name);
            // qingJiaoType → qingjiaotype
            if (data.qingJiaoType.HasValue) skill.SetField("qingjiaotype", (int)data.qingJiaoType.Value);
            // skillEffect → skillEffect
            if (!string.IsNullOrEmpty(data.skillEffect)) skill.SetField("skillEffect", data.skillEffect);
            // skillType → Skill_Type
            if (data.skillType.HasValue) skill.SetField("Skill_Type", (int)data.skillType.Value);
            // affix → Affix
            if (data.affix != null)
            {
                skill.SetField("Affix", JSONObjectHelper.ToJSONObject(data.affix));
            }
            // affix2 → Affix2
            if (data.affix2 != null)
            {
                skill.SetField("Affix2", JSONObjectHelper.ToJSONObject(data.affix2));
            }
            // descr → descr
            if (!string.IsNullOrEmpty(data.descr)) skill.SetField("descr", data.descr);
            // tuJianDescr → TuJiandescr
            if (!string.IsNullOrEmpty(data.tuJianDescr)) skill.SetField("TuJiandescr", data.tuJianDescr);
            // attackType → AttackType
            if (data.attackType != null)
            {
                skill.SetField(
                    "AttackType",
                    JSONObjectHelper.ToJSONObject(data.attackType.ConvertAll(type => (int)type))
                );
            }
            // script → script（"SkillSelf" / "SkillAttack"）
            if (data.script.HasValue) skill.SetField(
                "script",
                data.script.Value == Script.对自己 ? "SkillSelf" : "SkillAttack"
            );
            // hp → HP
            if (data.hp.HasValue) skill.SetField("HP", data.hp.Value);
            // speed → speed
            if (data.speed.HasValue) skill.SetField("speed", data.speed.Value);
            // icon → icon
            if (data.icon.HasValue) skill.SetField("icon", data.icon.Value);
            // skillDisplayType → Skill_DisplayType
            if (data.skillDisplayType.HasValue)
                skill.SetField(
                    "Skill_DisplayType",
                    (int)data.skillDisplayType.Value
                );
            // skillJie → Skill_LV
            if (data.skillJie.HasValue) skill.SetField("Skill_LV", (int)data.skillJie.Value);
            // skillPin → typePinJie
            if (data.skillPin.HasValue) skill.SetField("typePinJie", (int)data.skillPin.Value);
            // tuJianType → TuJianType
            if (data.tuJianType.HasValue) skill.SetField("TuJianType", (int)data.tuJianType.Value);
            // df → DF
            if (data.df.HasValue) skill.SetField("DF", data.df.Value ? 1 : 0);
            // skillOpen → Skill_Open
            if (data.skillOpen.HasValue) skill.SetField("Skill_Open", data.skillOpen.Value);
            // skillCastTime → Skill_castTime
            if (data.skillCastTime.HasValue) skill.SetField("Skill_castTime", data.skillCastTime.Value);
            // canUseDistMax → canUseDistMax
            if (data.canUseDistMax.HasValue) skill.SetField("canUseDistMax", data.canUseDistMax.Value);
            // cd → CD
            if (data.cd.HasValue) skill.SetField("CD", data.cd.Value);
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
                skill.SetField("seid", seid);
            }
            // cost → skill_SameCastNum + skill_CastType + skill_Cast
            // 同系灵气（CardType.同）写入 skill_SameCastNum，可重复添加（多组同系消耗）
            // 元素灵气按类型累加后写入 skill_CastType + skill_Cast
            if (data.cost != null)
            {
                JSONObject sameCastNum = JSONObject.Create(JSONObject.Type.ARRAY);
                JSONObject castType = JSONObject.Create(JSONObject.Type.ARRAY);
                JSONObject cast = JSONObject.Create(JSONObject.Type.ARRAY);
                Dictionary<int, int> costDict = [];
                foreach (var (type, amount) in data.cost)
                {
                    if (type == CardType.同)
                    {
                        sameCastNum.Add(amount);
                    }
                    else
                    {
                        if (!costDict.ContainsKey((int)type)) costDict[(int)type] = 0;
                        costDict[(int)type] += amount;
                    }
                }

                foreach (var castkvp in costDict)
                {
                    castType.Add(castkvp.Key);
                    cast.Add(castkvp.Value);
                }

                skill.SetField("skill_SameCastNum", sameCastNum);
                skill.SetField("skill_CastType", castType);
                skill.SetField("skill_Cast", cast);
            }
        }

        /// <summary>
        /// 将单个神通实例注入到 _skillJsonData 中。
        /// 若 isNew 为 true 则新建，否则更新已有对象。
        /// </summary>
        private static void InjectSkillData(SkillInstanceData data)
        {
            string id = data.id.ToString();

            JSONObject skill;
            if (data.isNew)
            {
                skill = BuildNewSkill(data);
                jsonData.instance._skillJsonData.AddField(id, skill);
                jsonData.instance.skillJsonData.Add(id, skill);

                InitNewSkill(skill, data);
            }
            else
            {
                skill = jsonData.instance._skillJsonData[id];
            }

            ApplySkillData(skill, data);
        }

        /// <summary>
        /// 将神通实例的 seid 特性数据注入到 SkillSeidJsonData 中。
        /// </summary>
        private static void InjectSkillSeid(SkillInstanceData data)
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

                if (seidId < 0 || seidId >= jsonData.instance.SkillSeidJsonData.Length)
                {
                    Main.Log.LogWarning($"SkillSeid索引越界:{seidId}");
                    continue;
                }

                JSONObject seidTable = jsonData.instance.SkillSeidJsonData[seidId];

                if (seidTable == null)
                {
                    Main.Log.LogWarning($"SkillSeid未初始化:{seidId}");
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

        /// <summary>
        /// 将神通实例的 AI 行为数据注入到 AIJsonDate 中。
        /// </summary>
        private static void InjectSkillAI(SkillInstanceData data)
        {
            if (data.aiData == null || data.aiData.Count == 0) return;

            foreach (var ai in data.aiData)
            {
                object result = Registry.Resolve(ai.Key);

                if (result is not int aiId)
                {
                    Main.Log.LogWarning($"AI引用无效:{ai.Key}");
                    continue;
                }

                if (!jsonData.instance.AIJsonDate.TryGetValue(aiId, out JSONObject aiTable))
                {
                    Main.Log.LogWarning($"AI不存在:{aiId}");
                    continue;
                }

                JSONObject aiJson = JSONObject.Create(JSONObject.Type.OBJECT);

                aiJson.AddField("id", data.id);
                foreach (var kvp in ai.Value)
                {
                    if (kvp.Key == "id") continue;

                    aiJson.AddField(
                        kvp.Key,
                        JSONObjectHelper.ToJSONObject(Registry.Resolve(kvp.Value))
                    );
                }

                aiTable.SetField(data.id.ToString(), aiJson);
            }
        }
    }
}