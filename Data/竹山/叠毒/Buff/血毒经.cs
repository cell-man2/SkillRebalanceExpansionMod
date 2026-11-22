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
    [DataBase(DataCategory.Buff, "竹山叠毒", "血毒经")]
    public static class 血毒经
    {
        private static readonly List<int> healNum = [14, 28, 70, 140, 420];
        private static readonly List<int> poisonRemoveNum = [1, 2, 5, 10, 30];
        private static readonly Func<string> descrFunc = () =>
        {
            string expr = poisonRemoveNum[0].ToString();
            for (int i = 1; i < healNum.Count; i++)
            {
                expr = $"IIF(attack >= {healNum[i]}, {poisonRemoveNum[i]}, {expr})";
            }
            return "【中毒】触发时，恢复（attack）点生命值，" +
                $"若触发角色为你则额外移除【中毒】*（{expr}）；" +
                "回合开始时，若没有【中毒】则获得【中毒】*1";
        };


        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "血毒经(回血&展示)",
                localId = 50,
                buffType = BuffType.功法被动,
                descr = descrFunc(),
                name = "血毒经",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [1] = new()
                    {
                        ["value1"] = -1
                    }
                },
                trigger = Trigger.对方中毒时,
                buffIcon = 240,
                showOnlyOne = true,
            },
            new BuffData
            {
                key = "血毒经(触发回血)",
                localId = 51,
                buffType = BuffType.功法被动,
                descr = "立刻触发一次血毒经的恢复效果",
                name = "血毒经（触发回血）",
                removeTrigger = RemoveTrigger.触发后移除所有,
                seidData = new()
                {
                    [82] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:血毒经(回血&展示)"
                    },
                },
                trigger = Trigger.获得自身Buff后,
                buffIcon = 240,
                isHide = true,
            },
            new BuffData
            {
                key = "血毒经(移除中毒)",
                localId = 52,
                buffType = BuffType.功法被动,
                descr = "自身中毒触发时，移除自身（attack）层中毒",
                name = "血毒经（移除中毒）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [127] = new()
                    {
                        ["value1"] = new List<object> { "@buff:血毒经(触发回血)" },
                        ["value2"] = new List<object> { 1 },
                    },
                    [89] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = 1
                    },
                    [159] = new()
                    {
                        ["target"] = 1,
                        ["value1"] = 1,
                        ["panduan"] = "<",
                        ["value2"] = 0,
                    },
                    [111] = new()
                    {
                        ["value1"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.不主动触发,
                buffIcon = 240,
                isHide = true,
            },
            new BuffData
            {
                key = "血毒经(每回合中毒)",
                localId = 53,
                buffType = BuffType.功法被动,
                descr = "回合开始时，若没有【中毒】，获得【中毒】*1",
                name = "血毒经（每回合中毒）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [165] = new()
                    {
                        ["value1"] = 1
                    },
                    [127] = new()
                    {
                        ["value1"] = new List<object> { 1 },
                        ["value2"] = new List<object> { 1 },
                    },
                },
                trigger = Trigger.回合开始时,
                buffIcon = 240,
                isHide = true,
            },
            new BuffData
            {
                key = "血毒经(我方中毒检测)",
                localId = 54,
                buffType = BuffType.功法被动,
                descr = "当敌方中毒触发时，执行其拥有的血毒经后续效果",
                name = "血毒经（我方中毒检测）",
                removeTrigger = RemoveTrigger.不主动移除,
                seidData = new()
                {
                    [28] = new()
                    {
                        ["value1"] = 1,
                        ["value2"] = "@buff:血毒经(移除中毒)"
                    }
                },
                trigger = Trigger.对方中毒时,
                buffIcon = 240,
                isHide = true,
            },
            ..Wrappers()
        ];

        private static List<BuffData> Wrappers()
        {
            var list = new List<BuffData>();

            for (int i = 0; i < 5; i++)
            {
                list.Add(new BuffData
                {
                    key = $"血毒经(包装{i + 1})",
                    realId = 240 + i,
                    buffType = BuffType.功法被动,
                    descr = $"立刻获得血毒经（第{i + 1}层的效果），如果看到这个buff说明血毒经没有被正确应用",
                    name = $"血毒经(包装{i + 1})",
                    removeTrigger = RemoveTrigger.触发后移除所有,
                    seidData = new()
                    {
                        [127] = new()
                        {
                            ["value1"] = new List<object>
                            {
                                "@buff:血毒经(回血&展示)",
                                "@buff:血毒经(移除中毒)",
                                "@buff:血毒经(每回合中毒)"
                            },
                            ["value2"] = new List<object>
                            {
                                healNum[i],
                                poisonRemoveNum[i],
                                1
                            },
                        },
                        [185] = new()
                        {
                            ["value1"] = new List<object> { "@buff:血毒经(我方中毒检测)" },
                            ["value2"] = new List<object> { 1 },
                        },
                    },
                    trigger = Trigger.获得自身Buff后,
                    buffIcon = 240,
                    showOnlyOne = true,
                });
            }

            return list;
        }
    }
}