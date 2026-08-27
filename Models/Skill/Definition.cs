using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Models.Skill
{
    /// <summary>
    /// 神通包数据模型。一个 SkillData 代表一个神通，
    /// 包含所有等级（skillLv）的效果数据，经工厂展开为多个 SkillInstanceData 实例。
    /// </summary>
    public class SkillData
    {
        // ======================== 定位标识 ========================

        /// <summary>注册表键名，用于跨模块引用。展开后实例 key = 此 key + 等级数字。</summary>
        public string key;

        /// <summary>
        /// 本地偏移 ID，最终 skillId = baseId + localId（与 realId 二选一）。
        /// 注意：此 ID 是技能编号（Skill_ID），而非实例 ID（id）。
        /// </summary>
        public int? localId;
        /// <summary>
        /// 绝对 ID，直接作为技能编号（Skill_ID）写入 JSON（与 localId 二选一）。
        /// </summary>
        public int? realId;

        /// <summary>涵盖的神通等级列表，如 [1,2,3,4,5]，每个等级展开为一个实例。</summary>
        public List<int> skillLv;

        // ======================== 神通字段（按字母序） ========================

        /// <summary>词缀列表（旧版），对应 JSON 字段 Affix。</summary>
        public TierValue<List<int>> affix;
        /// <summary>词缀列表（新版），对应 JSON 字段 Affix2。</summary>
        public TierValue<List<int>> affix2;
        /// <summary>AI 行为数据，外层 Key 为 AI 编号，内层为参数键值对，注入到 AIJsonDate。</summary>
        public TierValue<Dictionary<object, Dictionary<string, object>>> aiData;
        /// <summary>攻击类型列表（金/木/水/火/土/气/神/剑/阵法等），对应 JSON 字段 AttackType。</summary>
        public TierValue<List<AttackType>> attackType;
        /// <summary>最大施法距离，对应 JSON 字段 canUseDistMax。</summary>
        public TierValue<int> canUseDistMax;
        /// <summary>冷却时间（秒），对应 JSON 字段 CD。</summary>
        public TierValue<float> cd;
        /// <summary>灵气消耗列表，每项为 (灵气类型, 数量)，转换为 skill_CastType + skill_Cast + skill_SameCastNum。</summary>
        public TierValue<List<(CardType type, int amount)>> cost;
        /// <summary>神通描述，对应 JSON 字段 descr。</summary>
        public TierValue<string> descr;
        /// <summary>是否可用于神仙斗法（DF），对应 JSON 字段 DF。</summary>
        public TierValue<bool> df;
        /// <summary>基础伤害，对应 JSON 字段 HP。</summary>
        public TierValue<int> hp;
        /// <summary>神通图标 ID，对应 JSON 字段 icon。</summary>
        public TierValue<int> icon;
        /// <summary>神通名称，对应 JSON 字段 name。</summary>
        public TierValue<string> name;
        /// <summary>神通请教类型，对应 JSON 字段 qingjiaotype。</summary>
        public TierValue<QingJiaoType> qingJiaoType;
        /// <summary>执行脚本类型（对敌人/对自己），对应 JSON 字段 script。</summary>
        public TierValue<Script> script;
        /// <summary>特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。</summary>
        public TierValue<Dictionary<object, Dictionary<string, object>>> seidData;
        /// <summary>参悟时间（月），对应 JSON 字段 Skill_castTime。</summary>
        public TierValue<int> skillCastTime;
        /// <summary>释放方式（目标身上/我到目标），对应 JSON 字段 Skill_DisplayType。</summary>
        public TierValue<SkillDisplayType> skillDisplayType;
        /// <summary>技能特效动画，对应 JSON 字段 skillEffect。</summary>
        public TierValue<string> skillEffect;
        /// <summary>神通阶级（人/地/天），对应 JSON 字段 Skill_LV。</summary>
        public TierValue<SkillJie> skillJie;
        /// <summary>开启境界，对应 JSON 字段 Skill_Open。</summary>
        public TierValue<int> skillOpen;
        /// <summary>神通品级（下/中/上），对应 JSON 字段 typePinJie。</summary>
        public TierValue<SkillPin> skillPin;
        /// <summary>神通类型（阵法/抽牌/buff/伤害/防御/填充），对应 JSON 字段 Skill_Type。</summary>
        public TierValue<SkillType> skillType;
        /// <summary>技能移动速度（通常为 0），对应 JSON 字段 speed。</summary>
        public TierValue<int> speed;
        /// <summary>神通图鉴描述，对应 JSON 字段 TuJiandescr。</summary>
        public TierValue<string> tuJianDescr;
        /// <summary>图鉴类型，对应 JSON 字段 TuJianType。</summary>
        public TierValue<TuJianType> tuJianType;
    }

    /// <summary>
    /// 神通实例数据模型。由 SkillData 展开生成，每个实例对应一个具体等级。
    /// </summary>
    public class SkillInstanceData
    {
        /// <summary>是否为新建实例（若游戏中已存在则复用 ID）。</summary>
        public bool isNew;

        /// <summary>注册表键名，由包数据 key + 等级数字生成（如 "蓄势_天阶1"）。</summary>
        public string key;

        /// <summary>最终写入 _skillJsonData 的主键 id。</summary>
        public int id;

        /// <summary>神通编号（Skill_ID），同一神通所有等级共用此编号。</summary>
        public int skillId;
        /// <summary>神通等级（Skill_Lv）。</summary>
        public int skillLv;

        // ======================== 神通字段 ========================

        /// <summary>词缀列表（旧版），对应 JSON 字段 Affix。</summary>
        public List<int> affix;
        /// <summary>词缀列表（新版），对应 JSON 字段 Affix2。</summary>
        public List<int> affix2;
        /// <summary>AI 行为数据，注入到 AIJsonDate。</summary>
        public Dictionary<object, Dictionary<string, object>> aiData;
        /// <summary>攻击类型列表，对应 JSON 字段 AttackType。</summary>
        public List<AttackType> attackType;
        /// <summary>最大施法距离，对应 JSON 字段 canUseDistMax。</summary>
        public int? canUseDistMax;
        /// <summary>冷却时间，对应 JSON 字段 CD。</summary>
        public float? cd;
        /// <summary>灵气消耗列表，转换为 skill_CastType + skill_Cast + skill_SameCastNum。</summary>
        public List<(CardType type, int amount)> cost;
        /// <summary>神通描述，对应 JSON 字段 descr。</summary>
        public string descr;
        /// <summary>是否可用于神仙斗法，对应 JSON 字段 DF。</summary>
        public bool? df;
        /// <summary>基础伤害，对应 JSON 字段 HP。</summary>
        public int? hp;
        /// <summary>神通图标 ID，对应 JSON 字段 icon。</summary>
        public int? icon;
        /// <summary>神通名称，对应 JSON 字段 name。</summary>
        public string name;
        /// <summary>神通请教类型，对应 JSON 字段 qingjiaotype。</summary>
        public QingJiaoType? qingJiaoType;
        /// <summary>执行脚本类型，对应 JSON 字段 script。</summary>
        public Script? script;
        /// <summary>特性列表，对应 JSON 字段 seid。</summary>
        public Dictionary<object, Dictionary<string, object>> seidData;
        /// <summary>参悟时间，对应 JSON 字段 Skill_castTime。</summary>
        public int? skillCastTime;
        /// <summary>释放方式，对应 JSON 字段 Skill_DisplayType。</summary>
        public SkillDisplayType? skillDisplayType;
        /// <summary>技能特效动画，对应 JSON 字段 skillEffect。</summary>
        public string skillEffect;
        /// <summary>神通阶级，对应 JSON 字段 Skill_LV。</summary>
        public SkillJie? skillJie;
        /// <summary>开启境界，对应 JSON 字段 Skill_Open。</summary>
        public int? skillOpen;
        /// <summary>神通品级，对应 JSON 字段 typePinJie。</summary>
        public SkillPin? skillPin;
        /// <summary>神通类型，对应 JSON 字段 Skill_Type。</summary>
        public SkillType? skillType;
        /// <summary>技能移动速度，对应 JSON 字段 speed。</summary>
        public int? speed;
        /// <summary>神通图鉴描述，对应 JSON 字段 TuJiandescr。</summary>
        public string tuJianDescr;
        /// <summary>图鉴类型，对应 JSON 字段 TuJianType。</summary>
        public TuJianType? tuJianType;
    }
}