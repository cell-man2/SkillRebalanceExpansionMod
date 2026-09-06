using System.Collections.Generic;
using System.Linq;
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
        public static readonly List<BuffData> Data =
        [
            .. Enumerable.Range(0, 3).Select(i =>
            {
                return new BuffData
                {
                    key = $"幻雾真法(闪避聚气&展示{i + 1})",
                    realId = 335 + i,
                    descr = "每次闪避对手的攻击时，获得【聚气】*（attack）" +
                    (
                        i >= 1
                        ? $"；释放【幻雾术】额外消耗一点水系灵气，并使获得的幻雾层数+{i}"
                        : ""
                    ),
                    removeTrigger = RemoveTrigger.不主动移除,
                    seidData = new()
                    {
                        [5] = new()
                        {
                            ["value1"] = new List<object> { 15 },
                            ["value2"] = new List<object> { 1 }
                        }
                    },
                    trigger = Trigger.闪避伤害时,
                    showOnlyOne = true
                };
            }),

            new BuffData
            {
                key = "幻雾真法(消耗增加)",
                realId = 338,
                descr = "释放【幻雾术】技能额外消耗一点水系灵气",
                name = "幻雾真法（消耗增加）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [144] = new()
                    {
                        ["value1"] = 207,
                        ["value2"] = 1,
                        ["value3"] = 2
                    }
                },
                trigger = Trigger.受到伤害时,
                buffIcon = 335,
                isHide = true
            },

            new BuffData
            {
                key = "幻雾真法(额外幻雾)",
                realId = 339,
                descr = "释放【幻雾术】技能额外获得【幻雾】*（attack）",
                name = "幻雾真法（额外幻雾）",
                seidData = new()
                {
                    [76] = new()
                    {
                        ["value1"] = 207
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> { 24 },
                        ["value2"] = new List<object> { 1 }
                    }
                },
                trigger = Trigger.使用技能时,
                buffIcon = 335,
                isHide = true
            }
        ];
    }
}