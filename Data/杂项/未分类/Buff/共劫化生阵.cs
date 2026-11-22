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
    [DataBase(DataCategory.Buff, "杂项未分类", "共劫化生阵")]
    public static class 共劫化生阵
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "共劫化生阵(展示)",
                realId = 75,
                buffType = BuffType.阵法,
                descr = "双方受到技能伤害时，将伤害的20%转嫁给对方；每因此转移（attack）点伤害使你获得（attack）点临时血量上限",
                name = "共劫化生阵",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> { 32 },
                        ["value2"] = new List<object> { 1 },
                    },
                    [126] = new()
                    {
                        ["value1"] = "@buff:共劫化生阵(劫)",
                        ["value2"] = "@buff:共劫化生阵(劫指示)"
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:共劫化生阵(触发阵法)" },
                        ["value2"] = new List<object> { 1 },
                    }
                },
                trigger = Trigger.不主动触发,
                showOnlyOne = true,
            },
            new BuffData
            {
                key = "共劫化生阵(劫指示)",
                localId = 55,
                buffType = BuffType.指示物,
                descr = "指示物-每处理（attack）点伤害后，触发一次共劫化生阵并移除劫*（attack）层",
                name = "共劫化生阵（劫指示）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    
                },
                trigger = Trigger.不主动触发,
                buffIcon = 75,
                isHide = true,
            },
            new BuffData
            {
                key = "共劫化生阵(触发阵法)",
                localId = 56,
                buffType = BuffType.指示物,
                descr = "依据指示移除对应层数的劫，然后触发一次共劫化生阵",
                name = "共劫化生阵（触发阵法）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:共劫化生阵(劫指示)",
                        ["value3"] = 1,
                        ["value4"] = "@buff:共劫化生阵(移除劫)"
                    },
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:共劫化生阵(展示)",
                    }
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 75,
                isHide = true,
            },
            new BuffData
            {
                key = "共劫化生阵(移除劫)",
                localId = 57,
                buffType = BuffType.指示物,
                descr = "移除劫*（attack）层",
                name = "共劫化生阵（移除劫）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [89] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:共劫化生阵(劫)",
                    }
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 75,
                isHide = true,
            },
            new BuffData
            {
                key = "共劫化生阵(劫)",
                localId = 58,
                buffType = BuffType.指示物,
                descr = "共劫化生阵已处理技能伤害（attack）点，若层数大于劫指示层数则触发共劫化生阵",
                name = "共劫化生阵（劫）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [119] = new()
                    {
                        ["target"] = 1,
                        ["value1"] = "@buff:共劫化生阵(展示)",
                        ["value2"] = 0
                    },
                    [126] = new()
                    {
                        ["value1"] = "@buff:共劫化生阵(劫)",
                        ["value2"] = "@buff:共劫化生阵(劫指示)"
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:共劫化生阵(触发阵法)" },
                        ["value2"] = new List<object> { 1 },
                    }
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 75,
                isHide = true,
            },
            new BuffData
            {
                key = "共劫化生阵(敌方劫计数)",
                localId = 59,
                buffType = BuffType.指示物,
                descr = "敌方获得劫*（attack）层",
                name = "共劫化生阵（敌方劫计数）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [315] = new()
                    {
                        ["value1"] = "@buff:共劫化生阵(劫)",
                        ["value2"] = 1,
                    }
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 75,
                isHide = true,
            },
            new BuffData
            {
                key = "共劫化生阵(我方转移伤害)",
                localId = 60,
                buffType = BuffType.指示物,
                descr = "受到伤害时，将伤害的20%转移给对方，并获得等量技能伤害的劫",
                name = "共劫化生阵（我方转移伤害）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [119] = new()
                    {
                        ["target"] = 1,
                        ["value1"] = "@buff:共劫化生阵(展示)",
                        ["value2"] = 0
                    },
                    [149] = new()
                    {
                        ["value1"] = new List<object> { "@buff:共劫化生阵(劫)" }
                    },
                    [187] = new()
                    {
                        ["value1"] = 20
                    }
                },
                trigger = Trigger.受到技能伤害时,
                buffIcon = 75,
                isHide = true,
            },
            new BuffData
            {
                key = "共劫化生阵(敌方转移伤害)",
                localId = 61,
                buffType = BuffType.指示物,
                descr = "受到伤害时，将伤害的20%转移给对方，并令对方获得等量技能伤害的劫",
                name = "共劫化生阵（敌方转移伤害）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [119] = new()
                    {
                        ["target"] = 2,
                        ["value1"] = "@buff:共劫化生阵(展示)",
                        ["value2"] = 0
                    },
                    [149] = new()
                    {
                        ["value1"] = new List<object> { "@buff:共劫化生阵(敌方劫计数)" }
                    },
                    [187] = new()
                    {
                        ["value1"] = 20
                    }
                },
                trigger = Trigger.受到技能伤害时,
                buffIcon = 75,
                isHide = true,
            },
        ];
    }
}