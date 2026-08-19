using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Models.Buff
{
    public class BuffData
    {
        // 注册定位
        public string key;

        // ID来源（二选一）
        public int? localId;
        public int? realId;

        // Buff字段
        public int? buffIcon;
        public string name;
        public BuffType? buffType;
        public Dictionary<object, Dictionary<string, object>> seidData;
        public string descr;
        public Trigger? trigger;
        public RemoveTrigger? removeTrigger;

        // 可选字段
        public List<int> affix;
        public SkillEffect? skillEffect;
        public string script;
        public int? loopTime;
        public int? totalTime;
        public StackType? stackType;
        public bool? isHide;
        public bool? showOnlyOne;
    }
}