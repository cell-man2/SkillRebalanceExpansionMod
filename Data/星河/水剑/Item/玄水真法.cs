using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Item;

namespace SkillRebalanceExpansionMod.Data.Item
{
    /// <summary>
    /// 【请替换为物品名称】物品数据。
    /// </summary>
    /// <remarks>
    /// 定位字段：
    ///   key      - 注册表键名，用于跨模块引用（如 "@item:金虹剑诀_天阶"），对应 Registry.item 的 Key。
    ///   localId  - 本地偏移 ID，最终 ID = baseId(28500) + localId（若 qingJiao 为 true 则再加 1000000000），与 realId 二选一。
    ///   realId   - 绝对 ID，直接作为 id 写入 JSON，与 localId 二选一。
    /// 
    /// 必填字段：
    ///   name       - 物品名称，对应 JSON 字段 name。
    ///   seidData   - 特性列表，外层 Key 为 seid 编号，内层为参数键值对，对应 JSON 字段 seid。
    /// 
    /// 技能书/功法书特化字段：
    ///   skillKey   - 关联的 BookInfo 注册键名，填写后工厂在新建物品时自动计算并填充 ItemIcon、quality、type、desc、StuTime、price、maxNum、typePinJie 等字段。
    ///                依赖此字段自动计算的字段，若不填写则由工厂计算；若填写则以填写的值为准。
    ///   qingJiao   - 是否为请教版本，默认 false。
    ///                true 时 ID 额外加上 1000000000 偏移，价格固定为 1，ShopType 固定为 99，且写入 NpcQingJiaoItemData。
    /// 
    /// 选填字段（含默认值，不填则写入以下值）：
    ///   affix        - 词缀列表（对应 JSON 字段 Affix），默认值：[]
    ///   canSale      - 是否可出售，对应 JSON 字段 CanSale，默认值：0（可出售）
    ///   canUse       - 可用次数，对应 JSON 字段 CanUse，默认值：0
    ///   danDu        - 丹毒量，对应 JSON 字段 DanDu，默认值：0
    ///   desc         - 物品说明，对应 JSON 字段 desc，默认值：null（若填写了 skillKey，则由工厂计算为 skillId）
    ///   desc2        - 物品描述，对应 JSON 字段 desc2，默认值：null
    ///   faBaoType    - 法宝类型，对应 JSON 字段 FaBaoType，默认值：""
    ///   itemFlag     - 物品标签列表，对应 JSON 字段 ItemFlag，默认值：[]
    ///   itemIcon     - 物品图标 ID，对应 JSON 字段 ItemIcon，默认值：0（若填写了 skillKey，则由工厂计算）
    ///   maxNum       - 最大堆叠数，对应 JSON 字段 maxNum，默认值：9999999（若填写了 skillKey，则由工厂计算为 1）
    ///   npcCanUse    - NPC 是否能使用，对应 JSON 字段 NPCCanUse，默认值：0（false）
    ///   price        - 物品价格，对应 JSON 字段 price，默认值：0（若填写了 skillKey，则由工厂计算）
    ///   quality      - 品阶，对应 JSON 字段 quality，默认值：null（若填写了 skillKey，则由工厂计算）
    ///   shopType     - 商店类型，对应 JSON 字段 ShopType，默认值：99（不投放）
    ///   shuaXin      - 刷新时间，对应 JSON 字段 ShuaXin，默认值：0
    ///   shuXingType  - 属性类别，对应 JSON 字段 ShuXingType，默认值：0
    ///   stuTime      - 领悟时间，对应 JSON 字段 StuTime，默认值：0（若填写了 skillKey，则由工厂计算）
    ///   tuJianType   - 图鉴类型，对应 JSON 字段 TuJianType，默认值：0（其它）
    ///   type         - 物品类型，对应 JSON 字段 type，默认值：null（若填写了 skillKey，则由工厂计算）
    ///   typePinJie   - 功法/技能品阶，对应 JSON 字段 typePinJie，默认值：1（若填写了 skillKey，则由工厂计算）
    ///   vagueType    - 大类型，对应 JSON 字段 vagueType，默认值：1
    ///   wuDao        - 领悟前置条件列表，每两项为一组（大道类型 + 大道等级），对应 JSON 字段 wuDao，默认值：[]
    ///   wuWeiType    - 五维类别，对应 JSON 字段 WuWeiType，默认值：0
    ///   yaoZhi1      - 药引，对应 JSON 字段 yaoZhi1，默认值：0
    ///   yaoZhi2      - 主药，对应 JSON 字段 yaoZhi2，默认值：0
    ///   yaoZhi3      - 辅药，对应 JSON 字段 yaoZhi3，默认值：0
    /// </remarks>
    [DataBase(DataCategory.Item, "星河水剑", "玄水真法")]
    public static class 玄水真法
    {
        public static readonly List<ItemData> Data =
        [
            new ItemData
            {
                key = "玄水真法",
                realId = 4213,
                desc2 = "星河剑派秘传天阶战斗类功法，此法参玄水之道，可在受伤时治疗自己。",
                price = 1000000,
                quality = 3,
                stuTime = 720,
                typePinJie = 3,
            },
            new ItemData
            {
                key = "玄水真法(请教)",
                realId = 4213,
                qingJiao = true,
                desc2 = "星河剑派秘传天阶战斗类功法。",
                quality = 3,
                stuTime = 720,
                typePinJie = 3,
            }
        ];
    }
}