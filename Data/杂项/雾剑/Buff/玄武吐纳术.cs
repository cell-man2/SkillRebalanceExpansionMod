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
    [DataBase(DataCategory.Buff, "杂项雾剑", "玄武吐纳术")]
    public static class 玄武吐纳术
    {
        public static readonly List<BuffData> Data = CreateData();

        private static List<BuffData> CreateData()
        {
            List<BuffData> result = [];
            int[] prob = [10, 15, 20, 25, 30];

            for (int i = 0; i < 5; i++)
            {
                int level = i + 1;

                // 吸收灵气检测&展示
                result.Add(new BuffData
                {
                    key = $"玄武吐纳术(吸收灵气检测&展示{level})",
                    realId = 382 + i,
                    seidData = new()
                    {
                        {
                            32,
                            new()
                            {
                                { "value1", 1 },
                                { "value2", $"@buff:玄武吐纳术(执行判定{level})" },
                                { "value3", 1 }
                            }
                        }
                    },
                    trigger = Trigger.抽牌时,
                });

                // 执行判定
                result.Add(new BuffData
                {
                    key = $"玄武吐纳术(执行判定{level})",
                    localId = 7 + i,
                    buffType = BuffType.功法被动,
                    descr = $"执行判定：有{prob[i]}%的几率获得【疗】*X。X为自身【幻雾】的层数",
                    name = $"玄武吐纳术（执行判定{level}）",
                    removeTrigger = RemoveTrigger.触发后移除所有,
                    seidData = new()
                    {
                        { 82, new() { { "value1", 1 }, { "value2", $"@buff:玄武吐纳术(获得疗{level})" } } },
                    },
                    trigger = Trigger.获得自身Buff后,
                    buffIcon = 382,
                    isHide = true,
                });

                // 获得疗
                result.Add(new BuffData
                {
                    key = $"玄武吐纳术(获得疗{level})",
                    localId = 12 + i,
                    buffType = BuffType.功法被动,
                    descr = $"触发后有{prob[i]}%的几率获得【疗】*X。X为自身【幻雾】的层数",
                    name = $"玄武吐纳术（获得疗{level}）",
                    removeTrigger = RemoveTrigger.不主动移除,
                    seidData = new()
                    {
                        { 65, new() { { "value1", prob[i] } } },
                        {
                            54,
                            new()
                            {
                                { "value1", 1 },
                                { "value2", 24 },
                                { "value3", 1 },
                                { "value4", 44 }
                            }
                        }
                    },
                    trigger = Trigger.不主动触发,
                    buffIcon = 382,
                    isHide = true,
                });
            }

            return result;
        }
    }
}