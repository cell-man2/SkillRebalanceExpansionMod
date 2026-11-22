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
    [DataBase(DataCategory.Buff, "竹山落英", "阳春百花诀")]
    public static class 阳春百花诀
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "阳春百花诀",
                realId = 239,
                buffType = BuffType.功法被动,
                descr = "每回合吸收灵气前，令你本回合吸收灵气数+（attack）",
                name = "阳春百花诀",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [2] = new()
                    {
                        ["value1"] = 1
                    }
                },
                trigger = Trigger.抽牌阶段开始时,
                buffIcon = 239,
                showOnlyOne = true,
            },
            new BuffData
            {
                key = "阳春百花诀(额外灵气)",
                localId = 64,
                buffType = BuffType.功法被动,
                descr = "本回合吸收灵气数+（attack）",
                name = "阳春百花诀",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [2] = new()
                    {
                        ["value1"] = 1
                    },
                },
                trigger = Trigger.抽牌阶段开始时,
                buffIcon = 239,
                isHide = true,
            },
            new BuffData
            {
                key = "阳春百花诀(限制&展示)",
                localId = 65,
                buffType = BuffType.功法被动,
                descr = "每回合吸收灵气前，令你本回合吸收灵气数+1；对手每有一种不同的负面状态，额外使吸收灵气数+1(上限为（attack-1）)",
                name = "阳春百花诀",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [126] = new()
                    {
                        ["value1"] = "@buff:阳春百花诀(额外灵气)",
                        ["value2"] = "@buff:阳春百花诀(限制&展示)"
                    },
                    [111] = new()
                    {
                        ["value1"] = new List<object> {"@buff:阳春百花诀(额外灵气)"}
                    },
                    [5] = new()
                    {
                        ["value1"] = new List<object> { "@buff:阳春百花诀(额外灵气)" },
                        ["value2"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.不主动触发,
                buffIcon = 239,
                showOnlyOne = true,
            },
            new BuffData
            {
                key = "阳春百花诀(负面检测)",
                localId = 66,
                buffType = BuffType.功法被动,
                descr = "吸收灵气前，令你本回合吸收灵气数+（attack），且对手每有一种不同的负面状态，额外使吸收灵气数+1",
                name = "阳春百花诀",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [5] = new()
                    {
                        ["value1"] = new List<object> { "@buff:阳春百花诀(额外灵气)" },
                        ["value2"] = new List<object> { 1 },
                    },
                    [156] = new()
                    {
                        ["target"] = 2,
                        ["value1"] = 1,
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:阳春百花诀(额外灵气)" },
                        ["value2"] = new List<object> { 1 },
                    },
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:阳春百花诀(限制&展示)"
                    },
                },
                trigger = Trigger.抽牌阶段开始时,
                buffIcon = 239,
                isHide = true,
            },
        ];
    }
}