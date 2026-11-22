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
    [DataBase(DataCategory.Buff, "碎星飞针", "鹤喙针")]
    public static class 鹤喙针
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "鹤喙针(木灵气检测)",
                localId = 76,
                buffType = BuffType.指示物,
                descr = "若木灵气数（attack）>4，则执行水灵气检测",
                name = "鹤喙针（木灵气检测）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [119] = new()
                    {
                        ["target"] = 1,
                        ["value1"] = "@buff:鹤喙针(木灵气检测)",
                        ["value2"] = 4
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:鹤喙针(执行水灵气检测)" },
                        ["value2"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4096,
                isHide = true,
            },
            new BuffData
            {
                key = "鹤喙针(执行水灵气检测)",
                localId = 77,
                buffType = BuffType.指示物,
                descr = "获得等量于当前水灵气数的水灵气检测",
                name = "鹤喙针（执行水灵气检测）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [37] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:鹤喙针(水灵气检测)",
                        ["value3"] = 1,
                        ["value4"] = 2
                    }
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4096,
                isHide = true,
            },
            new BuffData
            {
                key = "鹤喙针(水灵气检测)",
                localId = 78,
                buffType = BuffType.指示物,
                descr = "若水灵气数（attack）>4，获得消散木灵气*1",
                name = "鹤喙针（水灵气检测）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [119] = new()
                    {
                        ["target"] = 1,
                        ["value1"] = "@buff:鹤喙针(水灵气检测)",
                        ["value2"] = 4
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:鹤喙针(消散木灵气)" },
                        ["value2"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4096,
                isHide = true,
            },
            new BuffData
            {
                key = "鹤喙针(消散木灵气)",
                localId = 79,
                buffType = BuffType.指示物,
                descr = "消散1点木灵气，获得消散水灵气*1",
                name = "鹤喙针（消散木灵气）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [24] = new()
                    {
                        ["value1"] = "@sId:鹤喙针(消散木灵气)"
                    },
                    [129] = null,
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:鹤喙针(消散水灵气)" },
                        ["value2"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4096,
                isHide = true,
            },
            new BuffData
            {
                key = "鹤喙针(消散水灵气)",
                localId = 80,
                buffType = BuffType.指示物,
                descr = "消散1点水灵气，获得释放技能*1",
                name = "鹤喙针（消散水灵气）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [24] = new()
                    {
                        ["value1"] = "@sId:鹤喙针(消散水灵气)"
                    },
                    [129] = null,
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:鹤喙针(释放技能)" },
                        ["value2"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4096,
                isHide = true,
            },
            new BuffData
            {
                key = "鹤喙针(释放技能)",
                localId = 81,
                buffType = BuffType.指示物,
                descr = "释放鹤喙针",
                name = "鹤喙针（释放技能）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [24] = new()
                    {
                        ["value1"] = "@sId:鹤喙针"
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 4096,
                isHide = true,
            },
        ];
    }
}