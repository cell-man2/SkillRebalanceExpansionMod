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
    [DataBase(DataCategory.Buff, "竹山缠绕", "万象寄生诀")]
    public static class 万象寄生诀
    {
        public static readonly List<BuffData> Data = CreateData();

        private static List<BuffData> CreateData()
        {
            List<BuffData> result = [];

            for (int i = 1; i <= 5; i++)
            {
                var seidData = new Dictionary<object, Dictionary<string, object>>();

                if (i < 5)
                {
                    seidData[65] = new()
                    {
                        ["value1"] = 50 + 10 * i
                    };
                }

                seidData[127] = new()
                {
                    ["value1"] = new List<object> { "@buff:万象寄生诀(放技能)" },
                    ["value2"] = new List<object> { 1 }
                };

                result.Add(new BuffData
                {
                    key = $"万象寄生诀(展示{i})",
                    realId = 221 + i,
                    buffType = BuffType.功法被动,
                    descr = "每累计使用或消散3点木系灵气，" +
                        $"{(i<5 ? $"有{50+10*i}%的几率" : "")}" +
                        "释放一次【寄生引血术】",
                    name = "万象寄生诀",
                    removeTrigger = RemoveTrigger.不主动移除,
                    seidData = seidData,
                    trigger = Trigger.不主动触发,
                    buffIcon = 222,
                    showOnlyOne = true,
                });
            }

            // ==================== 万象寄生诀(放技能) ====================
            result.Add(new BuffData
            {
                key = "万象寄生诀(放技能)",
                realId = 227,
                descr = "释放一次【寄生引血术】",
                buffIcon = 222,
                isHide = true,
            });

            for (int i = 1; i <= 5; i++)
            {
                result.Add(new BuffData
                {
                    key = $"万象寄生诀(触发{i})",
                    localId = 66 + i,
                    buffType = BuffType.功法被动,
                    descr = $"立刻执行（attack）次第{i}层万象寄生诀的效果",
                    name = $"万象寄生诀（触发{i}）",
                    removeTrigger = RemoveTrigger.触发后移除所有,
                    seidData = new()
                    {
                        [82] = new()
                        {
                            ["value1"] = 1,
                            ["value2"] = $"@buff:万象寄生诀(展示{i})",
                        }
                    },
                    trigger = Trigger.获得自身Buff后,
                    buffIcon = 222,
                    isHide = true,
                });
            }

            result.Add(new BuffData
            {
                key = "万象寄生诀(计数)",
                localId = 72,
                buffType = BuffType.功法被动,
                descr = "已累积使用或消散（attack）点木系灵气，累积达到3层后触发一次万象寄生诀的效果",
                name = "万象寄生诀（计数）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [119] = new()
                    {
                        ["target"] = 1,
                        ["value1"] = "@buff:万象寄生诀(计数)",
                        ["value2"] = 2,
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object>
                        {
                            "@buff:万象寄生诀(触发1)",
                            "@buff:万象寄生诀(触发2)",
                            "@buff:万象寄生诀(触发3)",
                            "@buff:万象寄生诀(触发4)",
                            "@buff:万象寄生诀(触发5)",
                            "@buff:万象寄生诀(移除计数)"
                        },
                        ["value2"] = new List<object> {1, 1, 1, 1, 1, 1},
                    },
                },
                trigger = Trigger.满3层后,
                buffIcon = 222,
                isHide = true,
            });

            result.Add(new BuffData
            {
                key = "万象寄生诀(移除计数)",
                localId = 73,
                buffType = BuffType.功法被动,
                descr = "移除3层万象寄生诀的计数，再触发计数",
                name = "万象寄生诀（移除计数）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [89] = new()
                    {
                        ["value1"] = 3,
                        ["value2"] = "@buff:万象寄生诀(计数)",
                    },
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:万象寄生诀(计数)",
                    }
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 222,
                isHide = true,
            });

            result.Add(new BuffData
            {
                key = "万象寄生诀(使用灵气)",
                localId = 74,
                buffType = BuffType.功法被动,
                descr = "每使用1点木系灵气，获得计数*1",
                name = "万象寄生诀（使用灵气）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [39] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:万象寄生诀(计数)",
                        ["value3"] = 1,
                        ["value4"] = 1
                    }
                },
                trigger = Trigger.使用卡牌时,
                buffIcon = 222,
                isHide = true,
            });

            result.Add(new BuffData
            {
                key = "万象寄生诀(消散灵气)",
                localId = 75,
                buffType = BuffType.功法被动,
                descr = "每消散1点木系灵气，获得计数*1",
                name = "万象寄生诀（消散灵气）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [195] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = 1,
                        ["value3"] = "@buff:万象寄生诀(计数)",
                        ["value4"] = 1
                    }
                },
                trigger = Trigger.弃牌时,
                buffIcon = 222,
                isHide = true,
            });

            return result;
        }
    }
}