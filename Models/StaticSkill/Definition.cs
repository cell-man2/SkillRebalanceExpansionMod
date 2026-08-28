using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Models.StaticSkill
{
    /// <summary>
    /// 功法包数据模型。一个 StaticSkillData 代表一个功法，
    /// 包含所有等级（skillLv）的效果数据，经工厂展开为多个 StaticSkillInstanceData 实例。
    /// </summary>
    public class StaticSkillData
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

        /// <summary>涵盖的功法等级列表，如 [1,2,3,4,5]，每个等级展开为一个实例。</summary>
        public List<int> skillLv;

        // ======================== 功法字段（按字母序） ========================

        /// <summary>词缀列表，对应 JSON 字段 Affix。</summary>
        public TierValue<List<int>> affix;
        /// <summary>功法属性（金/木/水/火/土/气/遁术/神/剑/体），对应 JSON 字段 AttackType。</summary>
        public TierValue<AttackType> attackType;
        /// <summary>功法描述，对应 JSON 字段 descr。</summary>
        public TierValue<string> descr;
        /// <summary>是否可用于神仙斗法（DF），对应 JSON 字段 DF。</summary>
        public TierValue<bool> df;
        /// <summary>功法图标 ID，对应 JSON 字段 icon。</summary>
        public TierValue<int> icon;
        /// <summary>功法名称，对应 JSON 字段 name。</summary>
        public TierValue<string> name;
        /// <summary>功法请教类型，对应 JSON 字段 qingjiaotype。</summary>
        public TierValue<QingJiaoType> qingJiaoType;
        /// <summary>特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。</summary>
        public TierValue<Dictionary<object, Dictionary<string, object>>> seidData;
        /// <summary>参悟时间，对应 JSON 字段 Skill_castTime。</summary>
        public TierValue<int> skillCastTime;
        /// <summary>功法阶级（人/地/天），对应 JSON 字段 Skill_LV。</summary>
        public TierValue<SkillJie> skillJie;
        /// <summary>功法品级（下/中/上），对应 JSON 字段 typePinJie。</summary>
        public TierValue<SkillPin> skillPin;
        /// <summary>修炼速度，对应 JSON 字段 Skill_Speed。</summary>
        public TierValue<int> skillSpeed;
        /// <summary>功法类型标识（战斗/中庸/修炼），用于自动计算修炼速度，不直接写入 JSON。</summary>
        public TierValue<SkillStyle> skillStyle;
        /// <summary>功法图鉴描述，对应 JSON 字段 TuJiandescr。</summary>
        public TierValue<string> tuJianDescr;
        /// <summary>图鉴类型，对应 JSON 字段 TuJianType。</summary>
        public TierValue<TuJianType> tuJianType;
    }

    /// <summary>
    /// 功法实例数据模型。由 StaticSkillData 展开生成，每个实例对应一个具体等级。
    /// </summary>
    public class StaticSkillInstanceData
    {
        /// <summary>是否为新建实例（若游戏中已存在则复用 ID）。</summary>
        public bool isNew;

        /// <summary>注册表键名，由包数据 key + 等级数字生成（如 "金虹剑诀_天阶1"）。</summary>
        public string key;

        /// <summary>最终写入 StaticSkillJsonData 的主键 id。</summary>
        public int id;

        /// <summary>功法编号（Skill_ID），同一功法所有等级共用此编号。</summary>
        public int skillId;
        /// <summary>功法等级（Skill_Lv）。</summary>
        public int skillLv;

        // ======================== 功法字段 ========================

        /// <summary>词缀列表，对应 JSON 字段 Affix。</summary>
        public List<int> affix;
        /// <summary>功法属性，对应 JSON 字段 AttackType。</summary>
        public AttackType? attackType;
        /// <summary>功法描述，对应 JSON 字段 descr。</summary>
        public string descr;
        /// <summary>是否可用于神仙斗法，对应 JSON 字段 DF。</summary>
        public bool? df;
        /// <summary>功法图标 ID，对应 JSON 字段 icon。</summary>
        public int? icon;
        /// <summary>功法名称，对应 JSON 字段 name。</summary>
        public string name;
        /// <summary>功法请教类型，对应 JSON 字段 qingjiaotype。</summary>
        public QingJiaoType? qingJiaoType;
        /// <summary>特性列表，对应 JSON 字段 seid。</summary>
        public Dictionary<object, Dictionary<string, object>> seidData;
        /// <summary>参悟时间，对应 JSON 字段 Skill_castTime。</summary>
        public int? skillCastTime;
        /// <summary>功法阶级，对应 JSON 字段 Skill_LV。</summary>
        public SkillJie? skillJie;
        /// <summary>功法品级，对应 JSON 字段 typePinJie。</summary>
        public SkillPin? skillPin;
        /// <summary>修炼速度，对应 JSON 字段 Skill_Speed。</summary>
        public int? skillSpeed;
        /// <summary>功法类型标识，用于计算修炼速度，不写入 JSON。</summary>
        public SkillStyle? skillStyle;
        /// <summary>功法图鉴描述，对应 JSON 字段 TuJiandescr。</summary>
        public string tuJianDescr;
        /// <summary>图鉴类型，对应 JSON 字段 TuJianType。</summary>
        public TuJianType? tuJianType;
    }
}