using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    /// <summary>
    /// 【请替换为Buff名称】Buff 数据。
    /// </summary>
    /// <remarks>
    /// 定位字段：
    ///   key      - 注册表键名，用于跨模块引用（如 "@buff:蓄势"），对应 Registry.buff 的 Key。
    ///   localId  - 本地偏移 ID，最终 realId = baseId + localId，与 realId 二选一。
    ///   realId   - 绝对 ID，直接作为 buffid 写入 JSON，与 localId 二选一。
    /// 
    /// 必填字段：
    ///   buffType     - Buff 分类，对应 JSON 字段 bufftype。
    ///   descr        - Buff 描述，对应 JSON 字段 descr。
    ///   name         - Buff 名称，对应 JSON 字段 name。
    ///   removeTrigger - 移除方式，对应 JSON 字段 removeTrigger。
    ///   seidData     - 特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。
    ///   trigger      - 触发时机，对应 JSON 字段 trigger。
    /// 
    /// 选填字段（含默认值，不填则写入以下值）：
    ///   affix        - 词缀列表，对应 JSON 字段 Affix。默认值：[]（空数组）
    ///   buffIcon     - Buff 图标 ID，对应 JSON 字段 BuffIcon。默认值：0
    ///   isHide       - 是否隐藏，对应 JSON 字段 isHide。默认值：0（false）
    ///   loopTime     - 循环触发间隔，对应 JSON 字段 looptime。默认值：1
    ///   script       - 脚本类型，对应 JSON 字段 script。默认值："Buff"
    ///   showOnlyOne  - 是否只显示一层，对应 JSON 字段 ShowOnlyOne。默认值：0（false）
    ///   skillEffect  - 技能特效，对应 JSON 字段 skillEffect。默认值："fx_Summoner_o"
    ///   stackType    - 叠加方式，对应 JSON 字段 BuffType。默认值：0（StackType.叠加）
    ///   totalTime    - 总持续时间，对应 JSON 字段 totaltime。默认值：1
    /// </remarks>
    [DataBase(DataCategory.Buff, "杂项雾剑", "幻雾真法")]
    public static class 幻雾真法
    {
        public static readonly List<BuffData> Data = CreateData();

        private static List<BuffData> CreateData()
        {
            List<BuffData> result = [];
            int[] prob = [ 50, 55, 65, 80 ];

            for (int i = 0; i < 5; i++)
            {
                int level = i + 1;
                Dictionary<object, Dictionary<string, object>> seid = [];
                
                if (level < 5)
                {
                    seid.Add(65, new Dictionary<string, object> {{ "value1", prob[i] }});
                }
                seid.Add(79, new Dictionary<string, object> {{ "value1", 1 }});

                // 闪避偷灵气
                result.Add(new BuffData
                {
                    key = $"幻雾真法(闪避偷灵气&展示{level})",
                    realId = 335 + i,
                    descr =  "每次闪避对手的攻击时，" +
                        (level < 5 ? $"便有{prob[i]}%的几率" : "") +
                        "窃取对手的一点灵气" +
                        (level >= 3 ? $"；释放【幻雾术】技能将额外消耗一点水系灵气，并使获得的【幻雾】层数+1" : ""),
                    removeTrigger = RemoveTrigger.不主动移除,
                    seidData = seid,
                    trigger = Trigger.闪避伤害时,
                    showOnlyOne = true
                });
            }

            result.Add(new BuffData
            {
                key = "幻雾真法(消耗增加)",
                localId = 17,
                buffType = BuffType.功法被动,
                descr = "释放【幻雾术】技能额外消耗一点水系灵气",
                name = "幻雾真法（消耗增加）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    {
                        144,
                        new Dictionary<string, object>
                        {
                            { "value1", 207 },
                            { "value2", 1 },
                            { "value3", 2 }
                        } 
                    }
                },
                trigger = Trigger.受到伤害时,
                buffIcon = 335,
                isHide = true
            });

            result.Add(new BuffData
            {
                key = "幻雾真法(额外幻雾)",
                localId = 18,
                buffType = BuffType.功法被动,
                descr = "释放【幻雾术】技能额外获得【幻雾】*（attack）",
                name = "幻雾真法（额外幻雾）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    { 76, new Dictionary<string, object> {{"value1", 207}} },
                    {
                        5,
                        new Dictionary<string, object>
                        {
                            {"value1", new List<int> { 24 }},
                            {"value2", new List<int> { 1 }}
                        }
                    }
                },
                trigger = Trigger.使用技能时,
                buffIcon = 335,
                isHide = true
            });

            return result;
        }
    }
}