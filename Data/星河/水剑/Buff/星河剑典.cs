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
    [DataBase(DataCategory.Buff, "星河水剑", "星河剑典")]
    public static class 星河剑典
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "星河剑典(每回合收益&展示)",
                realId = 4238,
                descr = "回合开始时，获得【惊涛】*（attack），【止水】*（attack）；" +
                    "技能消耗【惊涛】/【止水】时，若大于【止水】/【惊涛】则获得等量【止水】/【惊涛】，" +
                    "若无【止水】/【惊涛】则因此功法获得【止水】/【惊涛】时额外获得1层",
                seidData = new()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object>
                        {
                            "@buff:惊涛",
                            "@buff:止水"
                        },
                        ["value2"] = new List<object> { 1, 1 }
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object>
                        {
                            "@buff:星河剑典(触发额外惊涛)",
                            "@buff:星河剑典(触发额外止水)"
                        },
                        ["value2"] = new List<object> { 1, 1 }
                    }
                },
            },
            new BuffData
            {
                key = "星河剑典(惊涛记录)",
                localId = 23,
                buffType = BuffType.功法被动,
                descr = "上一次发动消耗增益的技能前，【惊涛】层数：（attack）",
                name = "星河剑典（惊涛记录）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [111] = new()
                    {
                        ["value1"] = new List<object> {"@buff:星河剑典(惊涛记录)"}
                    }
                },
                trigger = Trigger.释放技能后,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(止水记录)",
                localId = 24,
                buffType = BuffType.功法被动,
                descr = "上一次发动消耗增益的技能前，【止水】层数：（attack）",
                name = "星河剑典（止水记录）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [111] = new()
                    {
                        ["value1"] = new List<object> {"@buff:星河剑典(止水记录)"}
                    }
                },
                trigger = Trigger.释放技能后,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(惊涛消耗技能检测)",
                localId = 25,
                buffType = BuffType.功法被动,
                descr = "发动消耗增益的技能时，记录当前【惊涛】层数",
                name = "星河剑典（惊涛消耗技能检测）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [169] = new()
                    {
                        ["value1"] = 51
                    },
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:惊涛",
                        ["value3"] = 1,
                        ["value4"] = "@buff:星河剑典(惊涛记录)"
                    }
                },
                trigger = Trigger.释放技能前,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(止水消耗技能检测)",
                localId = 26,
                buffType = BuffType.功法被动,
                descr = "发动消耗增益的技能时，记录当前【止水】层数",
                name = "星河剑典（止水消耗技能检测）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [169] = new()
                    {
                        ["value1"] = 51
                    },
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:止水",
                        ["value3"] = 1,
                        ["value4"] = "@buff:星河剑典(止水记录)"
                    }
                },
                trigger = Trigger.释放技能前,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(延迟惊涛)",
                localId = 27,
                buffType = BuffType.功法被动,
                descr = "释放技能后，获得【惊涛】*（attack）",
                name = "星河剑典（延迟惊涛）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> {"@buff:惊涛"},
                        ["value2"] = new List<object> {1},
                    },
                },
                trigger = Trigger.释放技能后,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(延迟止水)",
                localId = 28,
                buffType = BuffType.功法被动,
                descr = "释放技能后，获得【止水】*（attack）",
                name = "星河剑典（延迟止水）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> {"@buff:止水"},
                        ["value2"] = new List<object> {1},
                    },
                },
                trigger = Trigger.释放技能后,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(额外惊涛)",
                localId = 29,
                buffType = BuffType.功法被动,
                descr = "因星河剑典获得【惊涛】时，额外获得【惊涛】*（attack）",
                name = "星河剑典（额外惊涛）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> {"@buff:惊涛"},
                        ["value2"] = new List<object> {1},
                    },
                },
                trigger = Trigger.不主动触发,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(额外止水)",
                localId = 30,
                buffType = BuffType.功法被动,
                descr = "因星河剑典获得【止水】时，额外获得【止水】*（attack）",
                name = "星河剑典（额外止水）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> {"@buff:止水"},
                        ["value2"] = new List<object> {1},
                    },
                },
                trigger = Trigger.不主动触发,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(触发额外惊涛)",
                localId = 31,
                buffType = BuffType.功法被动,
                descr = "立刻触发（attack）次【星河剑典（额外惊涛）】",
                name = "星河剑典（触发额外惊涛）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:星河剑典(额外惊涛)",
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4238,
                isHide = true,
                stackType = StackType.覆盖,
            },
            new BuffData
            {
                key = "星河剑典(触发额外止水)",
                localId = 32,
                buffType = BuffType.功法被动,
                descr = "立刻触发（attack）次【星河剑典（额外止水）】",
                name = "星河剑典（触发额外止水）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:星河剑典(额外止水)",
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4238,
                isHide = true,
                stackType = StackType.覆盖,
            },
            new BuffData
            {
                key = "星河剑典(延迟额外惊涛)",
                localId = 33,
                buffType = BuffType.功法被动,
                descr = "释放技能后，触发（attack）次【星河剑典（额外惊涛）】",
                name = "星河剑典（延迟额外惊涛）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:星河剑典(额外惊涛)",
                    },
                },
                trigger = Trigger.释放技能后,
                buffIcon = 4238,
                isHide = true,
                stackType = StackType.覆盖,
            },
            new BuffData
            {
                key = "星河剑典(延迟额外止水)",
                localId = 34,
                buffType = BuffType.功法被动,
                descr = "释放技能后，触发（attack）次【星河剑典（额外止水）】",
                name = "星河剑典（延迟额外止水）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new ()
                {
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:星河剑典(额外止水)",
                    },
                },
                trigger = Trigger.释放技能后,
                buffIcon = 4238,
                isHide = true,
                stackType = StackType.覆盖,
            },
            new BuffData
            {
                key = "星河剑典(惊涛消耗检测)",
                localId = 35,
                buffType = BuffType.功法被动,
                descr = "使用技能消耗【惊涛】时，若【惊涛】记录层数大于【止水】记录层数，" +
                    "则释放技能后额外获得【止水】记录层数的【止水】，" + 
                    "若没有记录则令因此获得的【止水】层数永久增加（attack）层",
                name = "星河剑典（惊涛检测）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [164] = new()
                    {
                        ["value1"] = "@buff:惊涛",
                    },
                    [126] = new()
                    {
                        ["value1"] = "@buff:星河剑典(惊涛记录)",
                        ["value2"] = "@buff:星河剑典(止水记录)",
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> {"@buff:星河剑典(延迟额外止水)"},
                        ["value2"] = new List<object> {1},
                    },
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:星河剑典(止水记录)",
                        ["value3"] = 1,
                        ["value4"] = "@buff:星河剑典(延迟止水)"
                    },
                    [165] = new()
                    {
                        ["value1"] = "@buff:星河剑典(止水记录)"
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> {"@buff:星河剑典(额外止水)"},
                        ["value2"] = new List<object> {1},
                    }
                },
                trigger = Trigger.Buff移除后,
                buffIcon = 4238,
                isHide = true,
            },
            new BuffData
            {
                key = "星河剑典(止水消耗检测)",
                localId = 36,
                buffType = BuffType.功法被动,
                descr = "使用技能消耗【止水】时，若【止水】记录层数大于【惊涛】记录层数，" +
                    "则释放技能后额外获得【惊涛】记录层数的【惊涛】，" + 
                    "若没有记录则令因此获得的【惊涛】层数永久增加（attack）层",
                name = "星河剑典（止水检测）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [164] = new()
                    {
                        ["value1"] = "@buff:止水",
                    },
                    [126] = new()
                    {
                        ["value1"] = "@buff:星河剑典(止水记录)",
                        ["value2"] = "@buff:星河剑典(惊涛记录)",
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> {"@buff:星河剑典(延迟额外惊涛)"},
                        ["value2"] = new List<object> {1},
                    },
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:星河剑典(惊涛记录)",
                        ["value3"] = 1,
                        ["value4"] = "@buff:星河剑典(延迟惊涛)"
                    },
                    [165] = new()
                    {
                        ["value1"] = "@buff:星河剑典(惊涛记录)"
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> {"@buff:星河剑典(额外惊涛)"},
                        ["value2"] = new List<object> {1},
                    }
                },
                trigger = Trigger.Buff移除后,
                buffIcon = 4238,
                isHide = true,
            },
        ];
    }
}