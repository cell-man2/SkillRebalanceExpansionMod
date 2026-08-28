using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;
using System.Linq;

namespace SkillRebalanceExpansionMod.Factories
{
    /// <summary>
    /// NPC 类型工厂，负责扫描所有标记为 DataCategory.NPCLeiXing 的数据类，
    /// 将其展开为各境界实例后注入到游戏的 NPC 类型 JSON 数据中。
    /// 定位键为 (LiuPai, Level)，与技能工厂的 (Skill_ID, Skill_Lv) 类似。
    /// </summary>
    public static class NPCLeiXingFactory
    {
        // ======================== 常量 ========================

        /// <summary>
        /// 新 NPC 类型实例 ID 起始基数，用于分配 NPCLeiXingDate 中的 id。
        /// </summary>
        private const int baseId = 2500;

        /// <summary>
        /// 流派编号基数，localLiuPai 最终偏移为 baseLiuPai + localLiuPai。
        /// 此 ID 是流派编号（LiuPai），而非实例 ID（id）。
        /// </summary>
        private const int baseLiuPai = 680;

        // ======================== 静态字段 ========================

        /// <summary>
        /// 缓存所有展开后的 NPC 类型实例数据，供 Inject 阶段使用。
        /// </summary>
        private static readonly List<NPCLeiXingInstanceData> npcLeiXingInstanceDatas = [];

        // ======================== 公开方法 ========================

        /// <summary>
        /// 初始化阶段：扫描所有 NPC 类型包数据，展开为实例并注册到 Registry。
        /// </summary>
        public static void Initialize()
        {
            npcLeiXingInstanceDatas.Clear();

            // 构建 (LiuPai, Level) → id 索引，用于判断实例是否已存在
            BuildNPCLeiXingIndex();

            List<NPCLeiXingData> npcLeiXingDatas = DataManager.Scan<NPCLeiXingData>(DataCategory.NPCLeiXing);

            // 排序确保 realLiuPai 优先，保证 ID 稳定性
            npcLeiXingDatas.Sort((a, b) =>
            {
                bool aReal = a.realLiuPai.HasValue;
                bool bReal = b.realLiuPai.HasValue;

                if (aReal != bReal) return aReal ? -1 : 1;

                if (aReal) return a.realLiuPai.Value.CompareTo(b.realLiuPai.Value);

                return (a.localLiuPai ?? int.MaxValue).CompareTo(b.localLiuPai ?? int.MaxValue);
            });

            int currentId = baseId;
            foreach (NPCLeiXingData data in npcLeiXingDatas)
            {
                // 展开为多个境界实例
                List<NPCLeiXingInstanceData> instances = ExpandNPCLeiXingData(data, ref currentId);

                foreach (NPCLeiXingInstanceData instance in instances)
                {
                    npcLeiXingInstanceDatas.Add(instance);

                    // 注册实例 key → id（用于 "@npcLeiXing:xxx" 引用）
                    if (string.IsNullOrEmpty(instance.key)) continue;
                    if (Registry.npcLeiXing.ContainsKey(instance.key))
                    {
                        Main.Log.LogWarning($"NPCLeiXing Key重复:{instance.key}, id:{instance.id}");
                        continue;
                    }

                    Registry.npcLeiXing.Add(instance.key, instance.id);
                }
            }
        }

        /// <summary>
        /// 注入阶段：将 NPC 类型实例数据写入游戏 JSON。
        /// </summary>
        public static void Inject()
        {
            foreach (NPCLeiXingInstanceData data in npcLeiXingInstanceDatas)
            {
                InjectNPCLeiXingData(data);
            }
            npcLeiXingInstanceDatas.Clear();
        }

        // ======================== 私有方法（按执行顺序排列） ========================

        /// <summary>
        /// 从游戏现有的 NPCLeiXingDate 中构建 (LiuPai, Level) → id 索引。
        /// 用于判断某个境界的 NPC 类型是否已存在。
        /// </summary>
        private static void BuildNPCLeiXingIndex()
        {
            foreach (JSONObject json in jsonData.instance.NPCLeiXingDate.list)
            {
                int liuPai = json["LiuPai"].I;
                int level = json["Level"].I;
                int id = json["id"].I;

                Registry.npcLeiXingIndex[(liuPai, level)] = id;
            }
        }

        /// <summary>
        /// 生成流派编号（LiuPai）：baseLiuPai + localLiuPai。
        /// </summary>
        private static int GenLiuPai(int localLiuPai)
        {
            return baseLiuPai + localLiuPai;
        }

        /// <summary>
        /// 将 NPCLeiXingData 展开为多个 NPCLeiXingInstanceData。
        /// 每个 level 对应一个实例。
        /// 新实例分配递增 id，已存在实例复用旧 id。
        /// </summary>
        private static List<NPCLeiXingInstanceData> ExpandNPCLeiXingData(NPCLeiXingData data, ref int curr)
        {
            List<NPCLeiXingInstanceData> result = [];

            if (data.level == null || data.level.Count == 0) return result;

            int liuPai;
            Dictionary<int, bool> isNew = data.level.ToDictionary(lv => lv, lv => true);

            if (data.realLiuPai.HasValue)
            {
                liuPai = data.realLiuPai.Value;
                foreach (int level in data.level)
                {
                    isNew[level] = !Registry.npcLeiXingIndex.ContainsKey((liuPai, level));
                }
            }
            else
            {
                liuPai = GenLiuPai(data.localLiuPai.Value);
            }

            foreach (int level in data.level)
            {
                NPCLeiXingInstanceData instance = new()
                {
                    isNew = isNew[level],
                    // 实例 key = 包 key + 等级数字
                    key = string.IsNullOrEmpty(data.key)
                        ? string.Empty
                        : $"{data.key}{level}",
                    id = isNew[level]
                        ? ++curr
                        : Registry.npcLeiXingIndex[(liuPai, level)],
                    liuPai = liuPai,
                    level = level,
                    type = data.type != null && data.type.TryGetValue(level, out var typeVal)
                        ? typeVal : null,
                    mengPai = data.mengPai != null && data.mengPai.TryGetValue(level, out var mengPaiVal)
                        ? mengPaiVal : null,
                    skills = data.skills != null && data.skills.TryGetValue(level, out var skillsVal)
                        ? skillsVal : null,
                    staticSkills = data.staticSkills != null && data.staticSkills.TryGetValue(level, out var staticSkillsVal)
                        ? staticSkillsVal : null,
                    jinDanType = data.jinDanType != null && data.jinDanType.TryGetValue(level, out var jinDanTypeVal)
                        ? jinDanTypeVal : null,
                    yuanYing = data.yuanYing != null && data.yuanYing.TryGetValue(level, out var yuanYingVal)
                        ? yuanYingVal : null,
                    huaShenLingYu = data.huaShenLingYu != null && data.huaShenLingYu.TryGetValue(level, out var huaShenLingYuVal)
                        ? huaShenLingYuVal : null,
                    lingGen = data.lingGen != null && data.lingGen.TryGetValue(level, out var lingGenVal)
                        ? lingGenVal : null,
                    wudaoType = data.wudaoType != null && data.wudaoType.TryGetValue(level, out var wudaoTypeVal)
                        ? wudaoTypeVal : null,
                    npcTag = data.npcTag != null && data.npcTag.TryGetValue(level, out var npcTagVal)
                        ? npcTagVal : null,
                    canJiaPaiMai = data.canJiaPaiMai != null && data.canJiaPaiMai.TryGetValue(level, out var canJiaPaiMaiVal)
                        ? canJiaPaiMaiVal : null,
                    paiMaiFenZu = data.paiMaiFenZu != null && data.paiMaiFenZu.TryGetValue(level, out var paiMaiFenZuVal)
                        ? paiMaiFenZuVal : null,
                    avatarType = data.avatarType != null && data.avatarType.TryGetValue(level, out var avatarTypeVal)
                        ? avatarTypeVal : null,
                    xinQuType = data.xinQuType != null && data.xinQuType.TryGetValue(level, out var xinQuTypeVal)
                        ? xinQuTypeVal : null,
                    equipWeapon = data.equipWeapon != null && data.equipWeapon.TryGetValue(level, out var equipWeaponVal)
                        ? equipWeaponVal : null,
                    equipClothing = data.equipClothing != null && data.equipClothing.TryGetValue(level, out var equipClothingVal)
                        ? equipClothingVal : null,
                    equipRing = data.equipRing != null && data.equipRing.TryGetValue(level, out var equipRingVal)
                        ? equipRingVal : null,
                    firstName = data.firstName != null && data.firstName.TryGetValue(level, out var firstNameVal)
                        ? firstNameVal : null,
                    shiLi = data.shiLi != null && data.shiLi.TryGetValue(level, out var shiLiVal)
                        ? shiLiVal : null,
                    attackType = data.attackType != null && data.attackType.TryGetValue(level, out var attackTypeVal)
                        ? attackTypeVal : null,
                    defenseType = data.defenseType != null && data.defenseType.TryGetValue(level, out var defenseTypeVal)
                        ? defenseTypeVal : null
                };
                result.Add(instance);
            }
            return result;
        }

        /// <summary>
        /// 构建一个仅包含 id、LiuPai、Level 的新 NPC 类型 JSON 对象。
        /// 无默认数据，所有字段由 ApplyNPCLeiXingData 填充。
        /// </summary>
        private static JSONObject BuildNewNPCLeiXing(NPCLeiXingInstanceData data)
        {
            JSONObject npc = JSONObject.Create(JSONObject.Type.OBJECT);

            npc.AddField("id", data.id);
            npc.AddField("LiuPai", data.liuPai);
            npc.AddField("Level", data.level);

            return npc;
        }

        /// <summary>
        /// 将 NPCLeiXingInstanceData 中的非空字段应用到 JSON 对象上。
        /// </summary>
        private static void ApplyNPCLeiXingData(
            JSONObject npc,
            NPCLeiXingInstanceData data
        )
        {
            // type → Type
            if (data.type.HasValue) npc.SetField("Type", (int)data.type.Value);
            // mengPai → MengPai
            if (data.mengPai.HasValue) npc.SetField("MengPai", (int)data.mengPai.Value);
            // skills → skills（支持字符串引用）
            if (data.skills != null)
            {
                JSONObject skills = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (object rawSkill in data.skills)
                {
                    if (Registry.Resolve(rawSkill) is int skillId)
                    {
                        skills.Add(skillId);
                    }
                    else
                    {
                        Main.Log.LogWarning($"NPCLeiXing技能引用无效:{rawSkill}");
                    }
                }
                npc.SetField("skills", skills);
            }
            // staticSkills → staticSkills（支持字符串引用）
            if (data.staticSkills != null)
            {
                JSONObject staticSkills = JSONObject.Create(JSONObject.Type.ARRAY);
                foreach (object rawStaticSkill in data.staticSkills)
                {
                    if (Registry.Resolve(rawStaticSkill) is int staticSkillId)
                    {
                        staticSkills.Add(staticSkillId);
                    }
                    else
                    {
                        Main.Log.LogWarning($"NPCLeiXing功法引用无效:{rawStaticSkill}");
                    }
                }
                npc.SetField("staticSkills", staticSkills);
            }
            // jinDanType → JinDanType
            if (data.jinDanType != null)
            {
                npc.SetField(
                    "JinDanType",
                    JSONObjectHelper.ToJSONObject(
                        data.jinDanType.ConvertAll(type => (int)type)
                    )
                );
            }
            // yuanYing → yuanying（支持字符串引用）
            if (data.yuanYing != null)
            {
                if (Registry.Resolve(data.yuanYing) is int id)
                {
                    npc.SetField("yuanying", id);
                }
                else
                {
                    Main.Log.LogWarning($"NPCLeiXing元婴功法引用无效:{data.yuanYing}");
                }
            }
            // huaShenLingYu → HuaShenLingYu
            if (data.huaShenLingYu.HasValue) npc.SetField("HuaShenLingYu",(int)data.huaShenLingYu.Value);
            // lingGen → LingGen
            if (data.lingGen != null) npc.SetField("LingGen", JSONObjectHelper.ToJSONObject(data.lingGen));
            // wudaoType → wudaoType
            if (data.wudaoType.HasValue) npc.SetField("wudaoType", data.wudaoType.Value);
            // npcTag → NPCTag
            if (data.npcTag != null)
            {
                npc.SetField(
                    "NPCTag",
                    JSONObjectHelper.ToJSONObject(
                        data.npcTag.ConvertAll(tag => (int)tag)
                    )
                );
            }
            // canJiaPaiMai → canjiaPaiMai（true = 参加，写入 0；false = 不参加，写入 1）
            if (data.canJiaPaiMai.HasValue)
            {
                npc.SetField(
                    "canjiaPaiMai",
                    data.canJiaPaiMai.Value ? 0 : 1
                );
            }
            // paiMaiFenZu → paimaifenzu
            if (data.paiMaiFenZu != null)
            {
                npc.SetField(
                    "paimaifenzu",
                    JSONObjectHelper.ToJSONObject(
                        data.paiMaiFenZu.ConvertAll(type => (int)type)
                    )
                );
            }
            // avatarType → AvatarType
            if (data.avatarType.HasValue)
            {
                npc.SetField(
                    "AvatarType",
                    (int)data.avatarType.Value
                );
            }
            // xinQuType → XinQuType
            if (data.xinQuType.HasValue)
            {
                npc.SetField(
                    "XinQuType",
                    data.xinQuType.Value
                );
            }
            // equipWeapon → equipWeapon
            if (data.equipWeapon != null)
            {
                npc.SetField(
                    "equipWeapon",
                    JSONObjectHelper.ToJSONObject(data.equipWeapon)
                );
            }
            // equipClothing → equipClothing
            if (data.equipClothing != null)
                npc.SetField(
                    "equipClothing",
                    JSONObjectHelper.ToJSONObject(data.equipClothing)
                );
            // equipRing → equipRing
            if (data.equipRing != null)
            {
                npc.SetField(
                    "equipRing",
                    JSONObjectHelper.ToJSONObject(data.equipRing)
                );
            }
            // firstName → FirstName
            if (!string.IsNullOrEmpty(data.firstName))
            {
                npc.SetField(
                    "FirstName",
                    data.firstName
                );
            }
            // shiLi → ShiLi
            if (data.shiLi != null)
            {
                npc.SetField(
                    "ShiLi",
                    JSONObjectHelper.ToJSONObject(data.shiLi)
                );
            }
            // attackType → AttackType
            if (data.attackType.HasValue)
            {
                npc.SetField(
                    "AttackType",
                    data.attackType.Value
                );
            }
            // defenseType → DefenseType
            if (data.defenseType.HasValue)
            {
                npc.SetField(
                    "DefenseType",
                    data.defenseType.Value
                );
            }
        }

        /// <summary>
        /// 将单个 NPC 类型实例注入到 NPCLeiXingDate 中。
        /// 若 isNew 为 true 则新建，否则更新已有对象。
        /// </summary>
        private static void InjectNPCLeiXingData(
            NPCLeiXingInstanceData data
        )
        {
            string id = data.id.ToString();

            JSONObject npc;

            if (data.isNew)
            {
                npc = BuildNewNPCLeiXing(data);
                jsonData.instance.NPCLeiXingDate.AddField(id, npc);
            }
            else
            {
                npc = jsonData.instance.NPCLeiXingDate.GetField(id);
            }

            ApplyNPCLeiXingData(npc, data);
        }
    }
}