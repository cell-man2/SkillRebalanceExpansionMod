using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Skill;
using SkillRebalanceExpansionMod.Models.Item;
using SkillRebalanceExpansionMod.Utils;
using System.Linq;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class SkillFactory
    {
        private const int baseId = 41750;
        private const int baseSkillId = 4370;
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
        private static readonly Dictionary<int, HashSet<int>> removeSkillSeid = [];
        private static readonly Dictionary<int, HashSet<int>> removeSkillAI = [];
        private static readonly List<SkillInstanceData> skillInstanceDatas = [];

        public static void Initialize()
        {
            removeSkillSeid.Clear();
            removeSkillAI.Clear();
            skillInstanceDatas.Clear();

            BuildSkillIndex();

            List<SkillData> skillDatas = DataManager.Scan<SkillData>(DataCategory.Skill);
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

                List<SkillInstanceData> instances = ExpandSkillData(data, ref currentId);

                foreach (SkillInstanceData instance in instances)
                {
                    if (skillId == null) skillId = instance.skillId;
                    if (jie == null && instance.skillJie.HasValue) jie = (int)instance.skillJie;
                    if (pin == null && instance.skillPin.HasValue) pin = (int)instance.skillPin;

                    skillInstanceDatas.Add(instance);

                    if (!instance.isNew)
                    {
                        if (instance.seidData != null) RegisterRemoveSkillSeid(instance);
                        if (instance.aiData != null) RegisterRemoveSkillAI(instance);
                    }

                    if (string.IsNullOrEmpty(instance.key)) continue;
                    if (Registry.skill.ContainsKey(instance.key))
                    {
                        Main.Log.LogWarning($"Skill Key重复:{instance.key}, id:{instance.id}");
                        continue;
                    }

                    Registry.skill.Add(instance.key, instance.id);
                }

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

        private static int GenSkillId(int localId)
        {
            return baseSkillId + localId;
        }

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
                    isNew[tier] = !Registry.staticSkillIndex.ContainsKey((skillId, tier));
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

        private static void InitNewSkill(JSONObject skill, SkillInstanceData data)
        {
            if (string.IsNullOrEmpty(data.descr)) return;

            if (data.affix2 == null)
            {
                skill.SetField(
                    "Affix2",
                    JSONObjectHelper.ToJSONObject(AffixProcessor.ExtractAffix(data.descr))
                );
            }

            if (string.IsNullOrEmpty(data.tuJianDescr))
            {
                skill.SetField(
                    "TuJiandescr",
                    AffixProcessor.FormatTuJian(data.descr)
                );
            }
        }

        private static void ApplySkillData(JSONObject skill, SkillInstanceData data)
        {
            // 神通名称
            if (!string.IsNullOrEmpty(data.name)) skill.SetField("name", data.name);
            // 请教类型
            if (data.qingJiaoType.HasValue) skill.SetField("qingjiaotype", (int)data.qingJiaoType.Value);
            // 神通特效
            if (!string.IsNullOrEmpty(data.skillEffect)) skill.SetField("skillEffect", data.skillEffect);
            // 神通类型
            if (data.skillType.HasValue) skill.SetField("Skill_Type", (int)data.skillType.Value);
            // 词缀
            if (data.affix != null)
            {
                skill.SetField("Affix", JSONObjectHelper.ToJSONObject(data.affix));
            }
            // 第二词缀
            if (data.affix2 != null)
            {
                skill.SetField("Affix2", JSONObjectHelper.ToJSONObject(data.affix2));
            }
            // 神通描述
            if (!string.IsNullOrEmpty(data.descr)) skill.SetField("descr", data.descr);
            // 神通图鉴描述
            if (!string.IsNullOrEmpty(data.tuJianDescr)) skill.SetField("TuJiandescr", data.tuJianDescr);
            // 神通属性
            if (data.attackType != null)
            {
                skill.SetField(
                    "AttackType",
                    JSONObjectHelper.ToJSONObject(data.attackType.ConvertAll(type => (int)type))
                );
            }
            // 技能脚本
            if (data.script.HasValue) skill.SetField(
                "script",
                data.script.Value == Script.对自己 ? "SkillSelf" : "SkillAttack"
            );
            // 伤害
            if (data.hp.HasValue) skill.SetField("HP", data.hp.Value);
            // 速度
            if (data.speed.HasValue) skill.SetField("speed", data.speed.Value);
            // 图标
            if (data.icon.HasValue) skill.SetField("icon", data.icon.Value);
            // 释放方式
            if (data.skillDisplayType.HasValue)
                skill.SetField(
                    "Skill_DisplayType",
                    (int)data.skillDisplayType.Value
                );
            // 神通阶级
            if (data.skillJie.HasValue) skill.SetField("Skill_LV", (int)data.skillJie.Value);
            // 神通品级
            if (data.skillPin.HasValue) skill.SetField("typePinJie", (int)data.skillPin.Value);
            // 图鉴类型
            if (data.tuJianType.HasValue) skill.SetField("TuJianType", (int)data.tuJianType.Value);
            // 神仙斗法
            if (data.df.HasValue) skill.SetField("DF", data.df.Value ? 1 : 0);
            // 技能开放
            if (data.skillOpen.HasValue) skill.SetField("Skill_Open", data.skillOpen.Value);
            // 施法时间
            if (data.skillCastTime.HasValue) skill.SetField("Skill_castTime", data.skillCastTime.Value);
            // 最大施法距离
            if (data.canUseDistMax.HasValue) skill.SetField("canUseDistMax", data.canUseDistMax.Value);
            // 冷却时间
            if (data.cd.HasValue) skill.SetField("CD", data.cd.Value);
            // 神通特性 Seid
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
            // 灵气消耗
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