using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Models.StaticSkill
{
    public class StaticSkillData
    {
        // 注册定位
        public string key;

        // Skill_ID来源（二选一）
        public int? localId;
        public int? realId;

        // 涵盖功法层数 Skill_Lv
        public List<int> skillLv;

        // 功法字段
        public TierValue<string> name;
        public TierValue<QingJiaoType> qingJiaoType;
        public TierValue<List<int>> affix;
        public TierValue<Dictionary<object, Dictionary<string, object>>> seidData;
        public TierValue<string> descr;
        public TierValue<AttackType> attackType;
        public TierValue<int> icon;
        public TierValue<SkillStyle> skillStyle;
        public TierValue<SkillJie> skillJie;
        public TierValue<SkillPin> skillPin;
        public TierValue<string> tuJianDescr;
        public TierValue<int> skillCastTime;
        public TierValue<int> skillSpeed;
        public TierValue<bool> df;
        public TierValue<TuJianType> tuJianType;
    }

    public class StaticSkillInstanceData
    {
        // 是否执行初始化流程
        public bool isNew;

        // 注册定位
        public string key;

        // 最终StaticSkillJsonData.id
        public int id;

        // 功法编号
        public int skillId;
        public int skillLv;

        // 功法字段
        public string name;
        public QingJiaoType? qingJiaoType;
        public List<int> affix;
        public Dictionary<object, Dictionary<string, object>> seidData;
        public string descr;
        public AttackType? attackType;
        public int? icon;
        public SkillStyle? skillStyle;
        public SkillJie? skillJie;
        public SkillPin? skillPin;
        public string tuJianDescr;
        public int? skillCastTime;
        public int? skillSpeed;
        public bool? df;
        public TuJianType? tuJianType;
    }
}