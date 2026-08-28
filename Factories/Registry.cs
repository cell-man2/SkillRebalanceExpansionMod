using System.Collections;
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Factories
{
    /// <summary>
    /// 注册表类，用于在 Mod 运行期间存储各类型数据的 key → id 映射，
    /// 以及复合键索引，支持跨模块引用解析。
    /// 所有数据在 Initialize 阶段注册，Inject 阶段使用，结束后 Clear 释放。
    /// </summary>
    public static class Registry
    {
        // ======================== 实例注册表（key → 实例 ID） ========================

        /// <summary>Buff 实例注册表，key 为 BuffData.key，值为 Buff 实例的 realId。</summary>
        public static Dictionary<string, int> buff = [];

        /// <summary>物品实例注册表，key 为 ItemData.key，值为物品实例的 realId。</summary>
        public static Dictionary<string, int> item = [];

        /// <summary>神通实例注册表，key 为 SkillData.key + 等级数字，值为神通实例的 id。</summary>
        public static Dictionary<string, int> skill = [];

        /// <summary>功法实例注册表，key 为 StaticSkillData.key + 等级数字，值为功法实例的 id。</summary>
        public static Dictionary<string, int> staticSkill = [];

        /// <summary>NPC 类型实例注册表，key 为 NPCLeiXingData.key + 等级数字，值为 NPC 类型实例的 id。</summary>
        public static Dictionary<string, int> npcLeiXing = [];

        // ======================== 编号注册表（包 key → Skill_ID） ========================

        /// <summary>神通编号注册表，key 为 SkillData.key，值为神通编号（Skill_ID）。</summary>
        public static Dictionary<string, int> sId = [];

        /// <summary>功法编号注册表，key 为 StaticSkillData.key，值为功法编号（Skill_ID）。</summary>
        public static Dictionary<string, int> ssId = [];

        // ======================== 复合信息注册表 ========================

        /// <summary>功法书/神通书信息注册表，key 为 SkillData.key / StaticSkillData.key，值为 BookInfo 对象。</summary>
        public static Dictionary<string, BookInfo> bookInfo = [];

        // ======================== 复合键索引（用于定位已有实例） ========================

        /// <summary>神通实例索引，(Skill_ID, Skill_Lv) → 实例 id，用于判断实例是否已存在。</summary>
        public static Dictionary<(int skillId, int skillLv), int> skillIndex = [];

        /// <summary>功法实例索引，(Skill_ID, Skill_Lv) → 实例 id，用于判断实例是否已存在。</summary>
        public static Dictionary<(int skillId, int skillLv), int> staticSkillIndex = [];

        /// <summary>NPC 类型实例索引，(LiuPai, Level) → 实例 id，用于判断实例是否已存在。</summary>
        public static Dictionary<(int liuPai, int level), int> npcLeiXingIndex = [];

        // ======================== 公开方法 ========================

        /// <summary>
        /// 清空所有注册表。
        /// 在 Loader.Start() 开始时和结束时各调用一次。
        /// </summary>
        public static void Clear()
        {
            buff.Clear();
            item.Clear();
            npcLeiXing.Clear();
            skill.Clear();
            staticSkill.Clear();
            sId.Clear();
            ssId.Clear();
            bookInfo.Clear();
            skillIndex.Clear();
            staticSkillIndex.Clear();
            npcLeiXingIndex.Clear();
        }

        /// <summary>
        /// 递归解析一个对象中的引用。
        /// 支持的类型：
        ///   - string：以 "@" 开头则解析为引用，否则原样返回
        ///   - IList：递归解析列表中的每个元素
        ///   - IDictionary：递归解析键和值
        ///   其他类型：原样返回
        /// </summary>
        public static object Resolve(object value)
        {
            if (value is string str)
            {
                return ResolveReference(str);
            }

            if (value is IList list)
            {
                List<object> result = [];
                foreach (object item in list)
                {
                    result.Add(Resolve(item));
                }
                return result;
            }

            if (value is IDictionary dict)
            {
                Dictionary<object, object> result = [];
                foreach (DictionaryEntry entry in dict)
                {
                    result.Add(
                        Resolve(entry.Key),
                        Resolve(entry.Value)
                    );
                }
                return result;
            }

            return value;
        }

        /// <summary>
        /// 解析单个字符串引用。
        /// 格式：@类型:key
        ///   例如：@buff:蓄势、@item:金虹剑诀_天阶、@sId:蓄势_天阶
        /// 若引用不存在，返回原字符串。
        /// </summary>
        private static object ResolveReference(string value)
        {
            if (!value.StartsWith("@")) return value;

            string reference = value.Substring(1);

            int split = reference.IndexOf(":");
            if (split <= 0) return value;

            string type = reference.Substring(0, split);
            string key = reference.Substring(split + 1);

            return type switch
            {
                "buff" when buff.TryGetValue(key, out int id) => id,
                "item" when item.TryGetValue(key, out int id) => id,
                "npcLeiXing" when npcLeiXing.TryGetValue(key, out int id) => id,
                "skill" when skill.TryGetValue(key, out int id) => id,
                "staticSkill" when staticSkill.TryGetValue(key, out int id) => id,
                "sId" when sId.TryGetValue(key, out int id) => id,
                "ssId" when ssId.TryGetValue(key, out int id) => id,
                "bookInfo" when bookInfo.TryGetValue(key, out BookInfo info) => info,
                _ => value
            };
        }
    }
}