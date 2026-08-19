using SkillRebalanceExpansionMod.Factories;

namespace SkillRebalanceExpansionMod.Core
{
    public static class Loader
    {
        public static void Start()
        {
            Registry.Clear();

            InitializeFactories();
            InjectFactories();

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
    }
}