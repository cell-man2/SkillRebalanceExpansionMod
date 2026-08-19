using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Models.Skill
{
    public class SkillData
    {
        // 注册定位
        public string key;

        // Skill_ID来源（二选一）
        public int? localId;
        public int? realId;

        // 涵盖技能等级 Skill_Lv
        public List<int> skillLv;

        // 神通字段
        public TierValue<string> name;
        public TierValue<QingJiaoType> qingJiaoType;
        public TierValue<string> skillEffect;
        public TierValue<SkillType> skillType;
        public TierValue<Dictionary<object, Dictionary<string, object>>> seidData;
        public TierValue<Dictionary<object, Dictionary<string, object>>> aiData;
        public TierValue<List<int>> affix;
        public TierValue<List<int>> affix2;
        public TierValue<string> descr;
        public TierValue<string> tuJianDescr;
        public TierValue<List<AttackType>> attackType;
        public TierValue<Script> script;
        public TierValue<int> hp;
        public TierValue<int> speed;
        public TierValue<int> icon;
        public TierValue<SkillDisplayType> skillDisplayType;
        public TierValue<List<(CardType type, int amount)>> cost;
        public TierValue<SkillJie> skillJie;
        public TierValue<SkillPin> skillPin;
        public TierValue<TuJianType> tuJianType;
        public TierValue<bool> df;
        public TierValue<int> skillOpen;
        public TierValue<int> skillCastTime;
        public TierValue<int> canUseDistMax;
        public TierValue<float> cd;
    }

    public class SkillInstanceData
    {
        // 是否执行初始化流程
        public bool isNew;

        // 注册定位
        public string key;

        // 最终skillJsonData.id
        public int id;

        // 神通字段
        public int skillId;
        public int skillLv;
        public string name;
        public QingJiaoType? qingJiaoType;
        public string skillEffect;
        public SkillType? skillType;
        public Dictionary<object, Dictionary<string, object>> seidData;
        public Dictionary<object, Dictionary<string, object>> aiData;
        public List<int> affix;
        public List<int> affix2;
        public string descr;
        public string tuJianDescr;
        public List<AttackType> attackType;
        public Script? script;
        public int? hp;
        public int? speed;
        public int? icon;
        public SkillDisplayType? skillDisplayType;
        public List<(CardType type, int amount)> cost;
        public SkillJie? skillJie;
        public SkillPin? skillPin;
        public TuJianType? tuJianType;
        public bool? df;
        public int? skillOpen;
        public int? skillCastTime;
        public int? canUseDistMax;
        public float? cd;
    }

}