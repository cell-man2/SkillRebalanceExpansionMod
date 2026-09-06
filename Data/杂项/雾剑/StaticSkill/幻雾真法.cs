using System;
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.StaticSkill
{
    /// <summary>
    /// 【请替换为功法名称】功法数据包。
    /// </summary>
    /// <remarks>
    /// 定位字段：
    ///   key      - 注册表键名前缀，展开后实例 key = 此 key + 等级数字（如 "金虹剑诀_天阶1"），
    ///              对应 Registry.staticSkill 的 Key。包数据 key 同时注册到 Registry.ssId 和 Registry.bookInfo。
    ///   localId  - 本地偏移 ID，最终 Skill_ID = baseSkillId(24530) + localId（与 realId 二选一）。
    ///   realId   - 绝对 ID，直接作为 Skill_ID 写入 JSON（与 localId 二选一）。
    ///   skillLv  - 涵盖的功法等级列表，如 [1,2,3,4,5]，每个等级展开为一个实例。
    /// 
    /// 必填字段：
    ///   attackType - 功法属性，对应 JSON 字段 AttackType。
    ///   descr      - 功法描述，对应 JSON 字段 descr。若未填写 tuJianDescr，工厂自动从此字段生成图鉴描述。
    ///   name       - 功法名称，对应 JSON 字段 name。
    ///   seidData   - 特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。默认值：null
    /// 
    /// 选填字段（使用 TierValue&lt;T&gt; 按等级配置，不填则使用默认值或由工厂自动计算）：
    ///   affix         - 词缀列表，对应 JSON 字段 Affix。默认值：[]（若未填写且提供了 descr，工厂自动从 descr 提取）
    ///   df            - 是否可用于神仙斗法，对应 JSON 字段 DF。默认值：0（false）
    ///   icon          - 功法图标 ID，对应 JSON 字段 icon。默认值：0
    ///   qingJiaoType  - 功法请教类型，对应 JSON 字段 qingjiaotype。默认值：1（QingJiaoType.普通）
    ///   skillCastTime - 参悟时间，对应 JSON 字段 Skill_castTime。默认值：由工厂根据 skillLv、skillPin、skillJie 自动计算
    ///   skillJie      - 功法阶级（人/地/天），对应 JSON 字段 Skill_LV。默认值：1（SkillJie.人，若不填写则修炼速度/参悟时间需自行指定）
    ///   skillPin      - 功法品级（下/中/上），对应 JSON 字段 typePinJie。默认值：1（SkillPin.下，若不填写则修炼速度/参悟时间需自行指定）
    ///   skillSpeed    - 修炼速度，对应 JSON 字段 Skill_Speed。默认值：由工厂根据 skillLv、skillPin、skillJie、skillStyle 自动计算
    ///   skillStyle    - 功法类型（战斗/中庸/修炼），用于自动计算修炼速度，不直接写入 JSON。若不填写则修炼速度需自行指定
    ///   tuJianDescr   - 功法图鉴描述，对应 JSON 字段 TuJiandescr。默认值：若未填写且提供了 descr，工厂自动从 descr 生成（替换【词缀名】为图鉴超链接）
    ///   tuJianType    - 图鉴类型，对应 JSON 字段 TuJianType。默认值：0（TuJianType.无）
    /// 
    /// 引用说明：
    ///   包数据 key → Registry.ssId（用于 "@ssId:xxx" 引用功法编号）
    ///   包数据 key → Registry.bookInfo（用于 ItemFactory 的 skillKey 引用）
    ///   实例 key   → Registry.staticSkill（用于 "@staticSkill:xxx" 引用功法实例）
    ///   实例 key 格式为 "包key + 等级数字"，如 "金虹剑诀_天阶1"
    /// </remarks>
    [DataBase(DataCategory.StaticSkill, "杂项雾剑", "幻雾真法")]
    public static class 幻雾真法
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];
        private static readonly Func<int, string> descrFunc = tier =>
            "每次闪避对手的攻击时，获得【聚气】*1" +
            (
                tier >= 3
                ? $"；释放【幻雾术】额外消耗一点水系灵气，并使获得的幻雾层数+{(tier == 5 ? 2 : 1)}"
                : ""
            );

        public static List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "幻雾真法",
                realId = 205,
                skillLv = skillLv,
                descr = TierValue<string>.Create(descrFunc, skillLv),
                affix = TierValue<List<int>>.Create(
                    tier => AffixProcessor.ExtractAffix(descrFunc(tier)),
                    skillLv
                ),
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier =>
                    {   
                        List<object> value1 = [$"@buff:幻雾真法(闪避聚气&展示{(tier+1)/2})"];
                        if (tier >= 3)
                        {
                            value1.Add("@buff:幻雾真法(消耗增加)");
                            value1.Add("@buff:幻雾真法(额外幻雾)");
                        }
                        
                        List<object> value2 = tier < 3
                            ? [1]
                            : [1, 1, tier == 5 ? 2 : 1];
                        
                        return new Dictionary<object, Dictionary<string, object>>
                        {
                            [1] = new Dictionary<string, object>
                            {
                                ["target"] = 1,
                                ["value1"] = value1,
                                ["value2"] = value2
                            }
                        };
                    },
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(descrFunc(tier)),
                    skillLv
                ),
            }
        ];
    }
}