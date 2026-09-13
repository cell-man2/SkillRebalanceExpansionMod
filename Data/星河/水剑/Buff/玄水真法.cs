using System;
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
    [DataBase(DataCategory.Buff, "星河水剑", "玄水真法")]
    public static class 玄水真法
    {
        private static readonly List<float> ratio = [0.2f, 0.4f, 0.6f, 0.8f, 1f];
        private static readonly List<int> recoverNum = [1, 2, 4, 6, 12];
        private static readonly Func<string> ratioExpr = () =>
        {
            string expr = ((int)Math.Round(ratio[0] * 100f)).ToString();
            for (int i = 1; i < recoverNum.Count; i++)
                expr = "IIF(" +
                    $"attack>={recoverNum[i]}," +
                    $"{(int)Math.Round(ratio[i] * 100f)}," +
                    $"{expr}" +
                ")";
            return expr;
        };

        public static readonly List<BuffData> Data = CreateData();

        private static List<BuffData> CreateData()
        {
            List<BuffData> result = [];

            // 受到伤害治疗自己
            result.Add(new BuffData
            {
                key = "玄水真法(受伤治疗&展示)",
                realId = 334,
                buffType = BuffType.功法被动,
                descr = "受到伤害时，恢复（attack）点生命值；" +
                    $"溢出治疗量的（{ratioExpr()}）%转化为对敌人造成的伤害",
                name = "玄水真法",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [1] = new()
                    {
                        ["value1"] = -1
                    }
                },
                trigger = Trigger.受到伤害后,
                buffIcon = 325,
                isHide = false,
                showOnlyOne = true,
                stackType = StackType.叠加,
            });

            // 伤害抑制，用于调整非化神境界的溢出伤害转换比率
            for (int i = 0; i < 4; i++)
            {
                result.Add(new BuffData
                {
                    key = $"玄水真法(伤害抑制{i + 1})",
                    localId = 37 + i,
                    buffType = BuffType.功法被动,
                    descr = $"下次造成的伤害减少{(int)Math.Round((1f - ratio[i]) * 100f)}%",
                    name = "玄水真法（伤害抑制）",
                    removeTrigger = RemoveTrigger.触发后移除所有,
                    seidData = new ()
                    {
                        [141] = new()
                        {
                            ["value1"] = ratio[i] - 1f
                        },
                    },
                    trigger = Trigger.造成任何伤害时,
                    buffIcon = 325,
                    isHide = true,
                });
            }

            // 溢出伤害转治疗
            for (int i = 0; i < 4; i++)
            {
                result.Add(new BuffData
                {
                    key = $"玄水真法(溢出治疗转伤害{i + 1})",
                    localId = 41 + i,
                    buffType = BuffType.功法被动,
                    descr = $"溢出治疗量的{(int)Math.Round(ratio[i] * 100f)}%转化为对敌人造成的伤害",
                    name = "玄水真法（溢出转化）",
                    removeTrigger = RemoveTrigger.不主动移除,
                    seidData = new ()
                    {
                        [127] = new()
                        {
                            ["value1"] = new List<object> {$"@buff:玄水真法(伤害抑制{i + 1})"},
                            ["value2"] = new List<object> {1},
                        },
                        [29] = null,
                        [111] = new()
                        {
                            ["value1"] = new List<object> {$"@buff:玄水真法(伤害抑制{i + 1})"},
                        }
                    },
                    trigger = Trigger.回血时,
                    buffIcon = 325,
                });
            }

            result.Add(new BuffData
            {
                key = "玄水真法(溢出治疗转伤害5)",
                realId = 325,
                buffType = BuffType.功法被动,
                descr = "溢出治疗量将转化为对敌人造成的伤害",
                name = "玄水真法（溢出治疗）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new ()
                {
                    [29] = null
                },
                trigger = Trigger.回血时,
                buffIcon = 325,
                isHide = true,
            });

            // 包装器buff，推迟溢出治疗转伤害buff的获取时机，解决通水不正确生效的问题
            for (int i = 0; i < 5; i++)
            {
                result.Add(new BuffData
                {
                    key = $"玄水真法(包装{i + 1})",
                    localId = 45 + i,
                    buffType = BuffType.功法被动,
                    descr = "战斗开始时，获得能力" +
                        $"“溢出治疗量的{(int)Math.Round(ratio[i] * 100f)}%转化为对敌人造成的伤害”，" +
                        "如果看到这个buff说明功法玄水真法没有正确生效",
                    name = "玄水真法（包装）",
                    removeTrigger = RemoveTrigger.触发后移除所有,
                    seidData = new ()
                    {
                        [127] = new()
                        {
                            ["value1"] = new List<object> { $"@buff:玄水真法(溢出治疗转伤害{i + 1})" },
                            ["value2"] = new List<object> { 1 },
                        },
                    },
                    trigger = Trigger.战斗开始时,
                    buffIcon = 325,
                });
            }

            return result;
        }
    }
}