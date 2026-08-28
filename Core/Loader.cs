using SkillRebalanceExpansionMod.Factories;
using System;
using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Core
{
    public static class Loader
    {
        public static void Start()
        {
            Registry.Clear();

            InitializeFactories();
            CheckRegistryDuplicates();
            InjectFactories();

            // 打印注册信息（仅当没有错误时执行）
            foreach (var buff in Registry.buff)
            {
                Main.Log.LogInfo($"Buff: {buff.Key} -> {buff.Value}");
            }
            Main.Log.LogInfo("==============================================");
            foreach (var skill in Registry.skill)
            {
                Main.Log.LogInfo($"Skill: {skill.Key} -> {skill.Value}");
            }
            Main.Log.LogInfo("==============================================");
            foreach (var staticSkill in Registry.staticSkill)
            {
                Main.Log.LogInfo($"StaticSkill: {staticSkill.Key} -> {staticSkill.Value}");
            }
            Main.Log.LogInfo("==============================================");
            foreach (var item in Registry.item)
            {
                Main.Log.LogInfo($"Item: {item.Key} -> {item.Value}");
            }
            Main.Log.LogInfo("==============================================");
            foreach (var item in Registry.bookInfo)
            {
                Main.Log.LogInfo($"Key:{item.Key} -> Value:{item.Value.skillId}");
            }
            Main.Log.LogInfo("==============================================");
            foreach (var item in Registry.sId)
            {
                Main.Log.LogInfo($"Key:{item.Key} -> Value:{item.Value}");
            }
            Main.Log.LogInfo("==============================================");
            foreach (var item in Registry.ssId)
            {
                Main.Log.LogInfo($"Key:{item.Key} -> Value:{item.Value}");
            }

            Registry.Clear();
        }

        private static void InitializeFactories()
        {
            BuffFactory.Initialize();
            SkillFactory.Initialize();
            StaticSkillFactory.Initialize();
            ItemFactory.Initialize();
            ShopFactory.Initialize();
            NPCLeiXingFactory.Initialize();
        }

        private static void InjectFactories()
        {
            BuffFactory.Inject();
            SkillFactory.Inject();
            StaticSkillFactory.Inject();
            ItemFactory.Inject();
            ShopFactory.Inject();
            NPCLeiXingFactory.Inject();
        }

        private static void CheckRegistryDuplicates()
        {
            // 需要检查的字典列表（所有存储 int ID 的字典）
            var dicts = new Dictionary<string, Dictionary<string, int>>
            {
                { "buff", Registry.buff },
                { "item", Registry.item },
                { "skill", Registry.skill },
                { "staticSkill", Registry.staticSkill },
                { "npcLeiXing", Registry.npcLeiXing },
                { "sId", Registry.sId },
                { "ssId", Registry.ssId }
            };

            foreach (var kvp in dicts)
            {
                string dictName = kvp.Key;
                var dict = kvp.Value;
                var seenIds = new HashSet<int>();

                foreach (var pair in dict)
                {
                    int id = pair.Value;
                    if (!seenIds.Add(id))
                    {
                        // 找出所有使用这个 ID 的 key
                        var keys = new List<string>();
                        foreach (var p in dict)
                        {
                            if (p.Value == id)
                                keys.Add(p.Key);
                        }
                        string keyList = string.Join(", ", keys);
                        throw new Exception(
                            $"注册表 {dictName} 中存在重复的 ID: {id}，被多个 key 使用: {keyList}，请检查配表"
                        );
                    }
                }
            }

            Main.Log.LogInfo("[Loader] 注册表查重完成，未发现重复 ID。");
        }
    }
}