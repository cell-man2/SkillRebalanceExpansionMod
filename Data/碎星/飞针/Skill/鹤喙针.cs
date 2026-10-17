using System;
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Skill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.Skill
{
    /// <summary>
    /// 【请替换为神通名称】神通数据包。
    /// </summary>
    /// <remarks>
    /// 定位字段：
    ///   key      - 注册表键名前缀，展开后实例 key = 此 key + 等级数字（如 "蓄势_天阶1"），
    ///              对应 Registry.skill 的 Key。包数据 key 同时注册到 Registry.sId 和 Registry.bookInfo。
    ///   localId  - 本地偏移 ID，最终 Skill_ID = baseSkillId(4370) + localId（与 realId 二选一）。
    ///   realId   - 绝对 ID，直接作为 Skill_ID 写入 JSON（与 localId 二选一）。
    ///   skillLv  - 涵盖的神通等级列表，如 [1,2,3,4,5]，每个等级展开为一个实例。
    /// 
    /// 必填字段：
    ///   attackType   - 攻击类型列表（可多选），对应 JSON 字段 AttackType。
    ///   cost         - 灵气消耗列表，每项为 (灵气类型, 数量)，转换为 skill_CastType + skill_Cast + skill_SameCastNum。
    ///                  支持指定属性消耗和同系消耗混合，参见下方灵气消耗格式说明。
    ///   descr        - 神通描述，对应 JSON 字段 descr。若未填写 tuJianDescr，工厂自动从此字段生成图鉴描述。
    ///   name         - 神通名称，对应 JSON 字段 name。
    ///   script       - 执行脚本类型（对敌人/对自己），对应 JSON 字段 script。
    ///   seidData     - 特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。
    ///   skillEffect  - 技能特效动画，对应 JSON 字段 skillEffect。
    /// 
    /// 选填字段（使用 TierValue&lt;T&gt; 按等级配置，不填则使用默认值）：
    ///   affix          - 词缀列表（旧版），对应 JSON 字段 Affix。默认值：[]
    ///   affix2         - 词缀列表（新版），对应 JSON 字段 Affix2。默认值：[]（若未填写且提供了 descr，工厂自动从 descr 提取）
    ///   aiData         - AI 行为数据，外层 Key 为 AI 编号，内层为参数键值对，注入到 AIJsonDate。默认值：null
    ///   canUseDistMax  - 最大施法距离，对应 JSON 字段 canUseDistMax。默认值：30
    ///   cd             - 冷却时间（秒），对应 JSON 字段 CD。默认值：10000
    ///   df             - 是否可用于神仙斗法，对应 JSON 字段 DF。默认值：0（false）
    ///   hp             - 基础伤害，对应 JSON 字段 HP。默认值：0
    ///   icon           - 神通图标 ID，对应 JSON 字段 icon。默认值：0
    ///   qingJiaoType   - 神通请教类型，对应 JSON 字段 qingjiaotype。默认值：1（QingJiaoType.普通）
    ///   skillCastTime  - 参悟时间（月），对应 JSON 字段 Skill_castTime。默认值：1
    ///   skillDisplayType - 释放方式，对应 JSON 字段 Skill_DisplayType。默认值：0（目标身上）
    ///   skillJie       - 神通阶级（人/地/天），对应 JSON 字段 Skill_LV。默认值：1（SkillJie.人）
    ///   skillOpen      - 开启境界，对应 JSON 字段 Skill_Open。默认值：1
    ///   skillPin       - 神通品级（下/中/上），对应 JSON 字段 typePinJie。默认值：1（SkillPin.下）
    ///   skillType      - 神通类型，对应 JSON 字段 Skill_Type。默认值：20（废弃）
    ///   speed          - 技能移动速度，对应 JSON 字段 speed。默认值：0
    ///   tuJianDescr    - 神通图鉴描述，对应 JSON 字段 TuJiandescr。默认值：若未填写且提供了 descr，工厂自动从 descr 生成
    ///   tuJianType     - 图鉴类型，对应 JSON 字段 TuJianType。默认值：0（TuJianType.无）
    /// 
    /// 灵气消耗格式说明：
    ///   cost 为 List&lt;(CardType type, int amount)&gt;，其中：
    ///   - 元素灵气（金/木/水/火/土/魔）：按类型累加，写入 skill_CastType 和 skill_Cast
    ///   - 同系灵气（CardType.同 = 999）：每次出现独立成为一组同系消耗，写入 skill_SameCastNum
    ///   
    ///   示例：消耗 3*木 + 2*同 + 2*同
    ///   cost = [
    ///       (CardType.木, 3),
    ///       (CardType.同, 2),
    ///       (CardType.同, 2)
    ///   ]
    ///   转换为：
    ///   skill_CastType = [1]      // 木
    ///   skill_Cast = [3]          // 3点
    ///   skill_SameCastNum = [2, 2] // 两组同系消耗
    /// 
    /// 引用说明：
    ///   包数据 key → Registry.sId（用于 "@sId:xxx" 引用神通编号）
    ///   包数据 key → Registry.bookInfo（用于 ItemFactory 的 skillKey 引用）
    ///   实例 key   → Registry.skill（用于 "@skill:xxx" 引用神通实例）
    ///   实例 key 格式为 "包key + 等级数字"，如 "蓄势_天阶1"
    /// </remarks>
    [DataBase(DataCategory.Skill, "碎星飞针", "鹤喙针")]
    public static class 鹤喙针
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];
        private static readonly Func<int, string> descrFunc = tier =>
            $"造成（attack）点伤害，获得【针】*1；若你的木系灵气与水系灵气均有至少5点，则消散1点木灵气与1点水灵气再次释放本技能";

        public static List<SkillData> Data =
        [
            new SkillData
            {
                key = "鹤喙针",
                realId = 127,
                skillLv = skillLv,
                descr = TierValue<string>.Create(descrFunc, skillLv),
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new()
                    {
                        [31] = new()
                        {
                            ["value1"] = new List<object> { 1 },
                            ["value2"] = new List<object> { 4096 },
                        },
                        [108] = new()
                        {
                            ["value1"] = 1,
                            ["value2"] = "@buff:鹤喙针(木灵气检测)",
                            ["value3"] = 1,
                        }
                    },
                    skillLv
                ),
                affix2 = TierValue<List<int>>.Create(
                    tier => AffixProcessor.ExtractAffix(descrFunc(tier)),
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(descrFunc(tier)),
                    skillLv
                ),
            },
            new SkillData
            {
                key = "鹤喙针(消散木灵气)",
                localId = 1,
                skillLv = skillLv,
                name = TierValue<string>.Create(tier => "消散1木灵气", skillLv),
                descr = TierValue<string>.Create(tier => "消散1木灵气", skillLv),
                attackType = new List<AttackType> { AttackType.气 },
                script = Script.对自己,
                skillEffect = "Ice1",
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new()
                    {
                        [15] = new()
                        {
                            ["value1"] = 1,
                            ["value2"] = 1,
                        }
                    },
                    skillLv
                ),
                cost = new List<(CardType, int)> {},
                affix2 = TierValue<List<int>>.Create(
                    tier => AffixProcessor.ExtractAffix(descrFunc(tier)),
                    skillLv
                ),
                df = false,
                hp = TierValue<int>.Create(tier => 0, skillLv),
                qingJiaoType = QingJiaoType.不可请教,
                skillJie = SkillJie.人,
                skillPin = SkillPin.下,
                skillType = SkillType.废弃,
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(descrFunc(tier)),
                    skillLv
                ),
                tuJianType = TuJianType.无,
            },
            new SkillData
            {
                key = "鹤喙针(消散水灵气)",
                localId = 2,
                skillLv = skillLv,
                name = TierValue<string>.Create(tier => "消散1水灵气", skillLv),
                descr = TierValue<string>.Create(tier => "消散1水灵气", skillLv),
                attackType = new List<AttackType> { AttackType.气 },
                script = Script.对自己,
                skillEffect = "Ice1",
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new()
                    {
                        [15] = new()
                        {
                            ["value1"] = 1,
                            ["value2"] = 2,
                        }
                    },
                    skillLv
                ),
                cost = new List<(CardType, int)> {},
                affix2 = TierValue<List<int>>.Create(
                    tier => AffixProcessor.ExtractAffix(descrFunc(tier)),
                    skillLv
                ),
                df = false,
                hp = TierValue<int>.Create(tier => 0, skillLv),
                qingJiaoType = QingJiaoType.不可请教,
                skillJie = SkillJie.人,
                skillPin = SkillPin.下,
                skillType = SkillType.废弃,
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(descrFunc(tier)),
                    skillLv
                ),
                tuJianType = TuJianType.无,
            },
        ];
    }
}