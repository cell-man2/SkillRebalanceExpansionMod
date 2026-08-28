using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Models.Item
{
    /// <summary>
    /// 物品数据模型，由各模块通过静态 Data 字段提供，经 ItemFactory 扫描后注入游戏 JSON。
    /// </summary>
    public class ItemData
    {
        // ======================== 定位标识 ========================

        /// <summary>注册表键名，用于跨模块引用（如 "@item:金虹剑诀_天阶"），对应 Registry.item 的 Key。</summary>
        public string key;

        /// <summary>
        /// 本地偏移 ID，最终 realId = baseId + localId（若 qingJiao 为 true 则再加 1000000000），与 realId 二选一。
        /// </summary>
        public int? localId;
        /// <summary>
        /// 绝对 ID，直接作为 id 写入 JSON，与 localId 二选一。
        /// </summary>
        public int? realId;

        // ======================== 特化字段 ========================

        /// <summary>
        /// 是否为请教版本（功法/神通书专用），true 时 ID 额外加上 qingJiaoOffset（1000000000），且价格固定为 1，ShopType 固定为 99。
        /// </summary>
        public bool qingJiao = false;

        /// <summary>
        /// 关联的 BookInfo 注册键名，用于功法/神通书自动填充 ItemIcon、quality、type、StuTime、price 等字段。
        /// </summary>
        public string skillKey;

        // ======================== 物品字段（按字母序） ========================

        /// <summary>词缀列表，对应 JSON 字段 Affix。</summary>
        public List<int> affix;
        /// <summary>是否可出售（false = 可出售，true = 不可出售），对应 JSON 字段 CanSale。</summary>
        public bool? canSale;
        /// <summary>可用次数，对应 JSON 字段 CanUse。</summary>
        public int? canUse;
        /// <summary>丹毒量，对应 JSON 字段 DanDu。</summary>
        public int? danDu;
        /// <summary>物品说明，对应 JSON 字段 desc。</summary>
        public string desc;
        /// <summary>物品描述，对应 JSON 字段 desc2。</summary>
        public string desc2;
        /// <summary>法宝类型，对应 JSON 字段 FaBaoType。</summary>
        public string faBaoType;
        /// <summary>物品标签列表，对应 JSON 字段 ItemFlag。</summary>
        public List<ItemFlag> itemFlag;
        /// <summary>物品图标 ID，对应 JSON 字段 ItemIcon。</summary>
        public int? itemIcon;
        /// <summary>最大堆叠数，对应 JSON 字段 maxNum。</summary>
        public int? maxNum;
        /// <summary>物品名称，对应 JSON 字段 name。</summary>
        public string name;
        /// <summary>NPC 是否能使用（true = 可以），对应 JSON 字段 NPCCanUse。</summary>
        public bool? npcCanUse;
        /// <summary>物品价格，对应 JSON 字段 price。</summary>
        public int? price;
        /// <summary>品阶，对应 JSON 字段 quality。</summary>
        public int? quality;
        /// <summary>特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。</summary>
        public Dictionary<object, Dictionary<string, object>> seidData;
        /// <summary>商店类型，对应 JSON 字段 ShopType。</summary>
        public ShopType? shopType;
        /// <summary>刷新时间，对应 JSON 字段 ShuaXin。</summary>
        public int? shuaXin;
        /// <summary>属性类别，对应 JSON 字段 ShuXingType。</summary>
        public int? shuXingType;
        /// <summary>领悟时间，对应 JSON 字段 StuTime。</summary>
        public int? stuTime;
        /// <summary>图鉴类型，对应 JSON 字段 TuJianType。</summary>
        public TuJianType? tuJianType;
        /// <summary>物品类型（武器/衣服/饰品/技能书/功法书等），对应 JSON 字段 type。</summary>
        public Type? type;
        /// <summary>功法/技能品阶，对应 JSON 字段 typePinJie。</summary>
        public int? typePinJie;
        /// <summary>大类型，对应 JSON 字段 vagueType。</summary>
        public int? vagueType;
        /// <summary>领悟前置条件列表（每两项为一组：大道类型 + 大道等级），对应 JSON 字段 wuDao。</summary>
        public List<(DaoType type, DaoLevel level)> wuDao;
        /// <summary>五维类别，对应 JSON 字段 WuWeiType。</summary>
        public int? wuWeiType;
        /// <summary>药引，对应 JSON 字段 yaoZhi1。</summary>
        public int? yaoZhi1;
        /// <summary>主药，对应 JSON 字段 yaoZhi2。</summary>
        public int? yaoZhi2;
        /// <summary>辅药，对应 JSON 字段 yaoZhi3。</summary>
        public int? yaoZhi3;
    }

    /// <summary>
    /// 功法/神通书信息，用于技能书类物品通过 skillKey 引用后自动计算默认的价格、领悟时间等字段。
    /// </summary>
    public class BookInfo
    {
        /// <summary>书籍类型（功法/神通）。</summary>
        public BookType type;
        /// <summary>关联的技能 ID。</summary>
        public int skillId;
        /// <summary>阶数（1 = 人阶，2 = 地阶，3 = 天阶）。</summary>
        public int? jie;
        /// <summary>品阶（1 = 下品，2 = 中品，3 = 上品）。</summary>
        public int? pin;
        /// <summary>是否请教类型。</summary>
        public bool? qingJiaoType;
    }
}