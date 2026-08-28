using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Models.Buff
{
    /// <summary>
    /// Buff 数据模型，由各模块通过静态 Data 字段提供，经 BuffFactory 扫描后注入游戏 JSON。
    /// </summary>
    public class BuffData
    {
        // ======================== 定位标识 ========================

        /// <summary>注册表键名，用于跨模块引用（如 "@buff:蓄势"），对应 Registry.buff 的 Key。</summary>
        public string key;

        /// <summary>本地偏移 ID，最终 realId = baseId + localId，与 realId 二选一。</summary>
        public int? localId;
        /// <summary>绝对 ID，直接作为 buffid 写入 JSON，与 localId 二选一。</summary>
        public int? realId;

        // ======================== Buff 字段 ========================

        /// <summary>词缀列表，对应 JSON 字段 Affix。</summary>
        public List<int> affix;
        /// <summary>Buff 图标 ID，对应 JSON 字段 BuffIcon。</summary>
        public int? buffIcon;
        /// <summary>Buff 分类，对应 JSON 字段 bufftype。</summary>
        public BuffType? buffType;
        /// <summary>Buff 描述，对应 JSON 字段 descr。</summary>
        public string descr;
        /// <summary>是否隐藏，对应 JSON 字段 isHide。</summary>
        public bool? isHide;
        /// <summary>循环触发间隔，对应 JSON 字段 looptime。</summary>
        public int? loopTime;
        /// <summary>Buff 名称，对应 JSON 字段 name。</summary>
        public string name;
        /// <summary>移除方式，对应 JSON 字段 removeTrigger。</summary>
        public RemoveTrigger? removeTrigger;
        /// <summary>脚本类型，对应 JSON 字段 script。</summary>
        public string script;
        /// <summary>特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。</summary>
        public Dictionary<object, Dictionary<string, object>> seidData;
        /// <summary>是否只显示一层，对应 JSON 字段 ShowOnlyOne。</summary>
        public bool? showOnlyOne;
        /// <summary>技能特效，对应 JSON 字段 skillEffect。</summary>
        public SkillEffect? skillEffect;
        /// <summary>叠加方式，对应 JSON 字段 BuffType。</summary>
        public StackType? stackType;
        /// <summary>总持续时间，对应 JSON 字段 totaltime。</summary>
        public int? totalTime;
        /// <summary>触发时机，对应 JSON 字段 trigger。</summary>
        public Trigger? trigger;
    }
}