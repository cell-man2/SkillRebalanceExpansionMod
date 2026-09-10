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
    [DataBase(DataCategory.Buff, "星河水剑", "乾坤化剑诀")]
    public static class 乾坤化剑诀
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "乾坤化剑诀(获得剑气&展示)",
                realId = 355,
                descr = "回合结束时，" +
                    "若【惊涛】层数大于【止水】，则获得【剑气】*（attack），每有【惊涛】*1额外获得【剑气】*1；" +
                    "若【止水】层数大于【惊涛】，则获得【疗】*（attack），每有【止水】*1额外获得【疗】*1",
                seidData = new()
                {
                    [126] = new()
                    {
                        ["value1"] = "@buff:惊涛",
                        ["value2"] = "@buff:止水",
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> {27},
                        ["value2"] = new List<object> {1},
                    },
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:惊涛",
                        ["value3"] = 1,
                        ["value4"] = 27
                    }
                },
            },
            new BuffData
            {
                key = "乾坤化剑诀(获得疗)",
                realId = 360,
                descr = "回合结束时，" +
                    "若【止水】层数大于【惊涛】，则获得【疗】*（attack），每有【止水】*1额外获得【疗】*1",
                seidData = new()
                {
                    [126] = new()
                    {
                        ["value1"] = "@buff:止水",
                        ["value2"] = "@buff:惊涛",
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> {44},
                        ["value2"] = new List<object> {1},
                    },
                    [54] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:止水",
                        ["value3"] = 1,
                        ["value4"] = 44
                    }
                },
            }
        ];
    }
}