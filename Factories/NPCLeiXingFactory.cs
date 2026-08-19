using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;
using System.Linq;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class NPCLeiXingFactory
    {
        private const int baseId = 2500;
        private const int baseLiuPai = 680;

        private static readonly List<NPCLeiXingInstanceData> npcLeiXingInstanceDatas = [];

        public static void Initialize()
        {
            npcLeiXingInstanceDatas.Clear();

            BuildNPCLeiXingIndex();

            List<NPCLeiXingData> npcLeiXingDatas = DataManager.Scan<NPCLeiXingData>(DataCategory.NPCLeiXing);

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
                List<NPCLeiXingInstanceData> instances = ExpandNPCLeiXingData(data, ref currentId);

                foreach (NPCLeiXingInstanceData instance in instances)
                {
                    npcLeiXingInstanceDatas.Add(instance);

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

        public static void Inject()
        {
            foreach (NPCLeiXingInstanceData data in npcLeiXingInstanceDatas)
            {
                InjectNPCLeiXingData(data);
            }
            npcLeiXingInstanceDatas.Clear();
        }

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

        private static int GenLiuPai(int localLiuPai)
        {
            return baseLiuPai + localLiuPai;
        }

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
                    isNew[level] = !Registry.staticSkillIndex.ContainsKey((liuPai, level));
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

        private static JSONObject BuildNewNPCLeiXing(NPCLeiXingInstanceData data)
        {
            JSONObject npc = JSONObject.Create(JSONObject.Type.OBJECT);

            npc.AddField("id", data.id);
            npc.AddField("LiuPai", data.liuPai);
            npc.AddField("Level", data.level);

            return npc;
        }

        private static void ApplyNPCLeiXingData(
            JSONObject npc,
            NPCLeiXingInstanceData data
        )
        {
            // NPC类型
            if (data.type.HasValue) npc.SetField("Type", (int)data.type.Value);
            // NPC势力
            if (data.mengPai.HasValue) npc.SetField("MengPai", (int)data.mengPai.Value);
            // 技能
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
            // 功法
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
            // 金丹类型
            if (data.jinDanType != null)
            {
                npc.SetField(
                    "JinDanType",
                    JSONObjectHelper.ToJSONObject(
                        data.jinDanType.ConvertAll(type => (int)type)
                    )
                );
            }
            // 元婴功法
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
            // 化神领域
            if (data.huaShenLingYu.HasValue) npc.SetField("HuaShenLingYu",(int)data.huaShenLingYu.Value);
            // 灵根
            if (data.lingGen != null) npc.SetField("LingGen", JSONObjectHelper.ToJSONObject(data.lingGen));
            // 悟道类型
            if (data.wudaoType.HasValue) npc.SetField("wudaoType", data.wudaoType.Value);
            // 标签
            if (data.npcTag != null)
            {
                npc.SetField(
                    "NPCTag",
                    JSONObjectHelper.ToJSONObject(
                        data.npcTag.ConvertAll(tag => (int)tag)
                    )
                );
            }
            // 是否参加拍卖
            if (data.canJiaPaiMai.HasValue)
            {
                npc.SetField(
                    "canjiaPaiMai",
                    data.canJiaPaiMai.Value ? 0 : 1
                );
            }
            // 拍卖分组
            if (data.paiMaiFenZu != null)
            {
                npc.SetField(
                    "paimaifenzu",
                    JSONObjectHelper.ToJSONObject(
                        data.paiMaiFenZu.ConvertAll(type => (int)type)
                    )
                );
            }
            // 种族
            if (data.avatarType.HasValue)
            {
                npc.SetField(
                    "AvatarType",
                    (int)data.avatarType.Value
                );
            }
            // 感兴趣物品
            if (data.xinQuType.HasValue)
            {
                npc.SetField(
                    "XinQuType",
                    data.xinQuType.Value
                );
            }
            // 装备偏好
            if (data.equipWeapon != null)
            {
                npc.SetField(
                    "equipWeapon",
                    JSONObjectHelper.ToJSONObject(data.equipWeapon)
                );
            }
            if (data.equipClothing != null)
                npc.SetField(
                    "equipClothing",
                    JSONObjectHelper.ToJSONObject(data.equipClothing)
                );

            if (data.equipRing != null)
            {
                npc.SetField(
                    "equipRing",
                    JSONObjectHelper.ToJSONObject(data.equipRing)
                );
            }
            // 姓
            if (!string.IsNullOrEmpty(data.firstName))
            {
                npc.SetField(
                    "FirstName",
                    data.firstName
                );
            }
            // 战斗力
            if (data.shiLi != null)
            {
                npc.SetField(
                    "ShiLi",
                    JSONObjectHelper.ToJSONObject(data.shiLi)
                );
            }
            // 攻击类型
            if (data.attackType.HasValue)
            {
                npc.SetField(
                    "AttackType",
                    data.attackType.Value
                );
            }
            // 防御类型
            if (data.defenseType.HasValue)
            {
                npc.SetField(
                    "DefenseType",
                    data.defenseType.Value
                );
            }
        }

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