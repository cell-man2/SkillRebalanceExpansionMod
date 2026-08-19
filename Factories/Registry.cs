using System.Collections;
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Factories
{
    public static class Registry
    {
        // key -> realId
        public static Dictionary<string, int> buff = [];
        public static Dictionary<string, int> item = [];
        public static Dictionary<string, int> skill = [];
        public static Dictionary<string, int> staticSkill = [];
        public static Dictionary<string, int> npcLeiXing = [];

        public static Dictionary<string, int> sId = [];
        public static Dictionary<string, int> ssId = [];
        public static Dictionary<string, BookInfo> bookInfo = [];

        // (Skill_ID, Skill_Lv) -> realId
        public static Dictionary<(int skillId, int skillLv), int> skillIndex = [];
        public static Dictionary<(int skillId, int skillLv), int> staticSkillIndex = [];
        // (LiuPai, Level) -> realId
        public static Dictionary<(int liuPai, int level), int> npcLeiXingIndex = [];

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
                "staticSkill" when staticSkill.TryGetValue(key, out int id)=> id,
                "sId" when sId.TryGetValue(key, out int id) => id,
                "ssId" when ssId.TryGetValue(key, out int id)=> id,
                "bookInfo" when bookInfo.TryGetValue(key, out BookInfo info)=> info,
                _ => value
            };
        }
    
    }
}