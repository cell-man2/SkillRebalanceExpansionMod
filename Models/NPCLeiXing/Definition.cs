using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Models.NPCLeiXing
{
    /// <summary>
    /// NPC 类型包数据模型。一个 NPCLeiXingData 代表一种 NPC 流派配置，
    /// 包含所有境界（level）的数据，经工厂展开为多个 NPCLeiXingInstanceData 实例。
    /// </summary>
    public class NPCLeiXingData
    {
        // ======================== 定位标识 ========================

        /// <summary>注册表键名，用于跨模块引用。展开后实例 key = 此 key + 等级数字。</summary>
        public string key;

        /// <summary>
        /// 绝对流派编号，直接作为 LiuPai 写入 JSON（与 localLiuPai 二选一）。
        /// </summary>
        public int? realLiuPai;
        /// <summary>
        /// 本地偏移流派编号，最终 liuPai = baseLiuPai(680) + localLiuPai（与 realLiuPai 二选一）。
        /// </summary>
        public int? localLiuPai;

        /// <summary>涵盖的 NPC 境界列表，如 [9,10,11,12,13,14,15]，每个境界展开为一个实例。</summary>
        public List<int> level;

        // ======================== NPC 字段（按字母序） ========================

        /// <summary>攻击类型，对应 JSON 字段 AttackType。</summary>
        public TierValue<int> attackType;
        /// <summary>种族，对应 JSON 字段 AvatarType。</summary>
        public TierValue<AvatarType> avatarType;
        /// <summary>是否参加拍卖（true = 参加，false = 不参加），对应 JSON 字段 canjiaPaiMai。</summary>
        public TierValue<bool> canJiaPaiMai;
        /// <summary>防御类型，对应 JSON 字段 DefenseType。</summary>
        public TierValue<int> defenseType;
        /// <summary>偏好防具属性列表，对应 JSON 字段 equipClothing。</summary>
        public TierValue<List<int>> equipClothing;
        /// <summary>偏好饰品属性列表，对应 JSON 字段 equipRing。</summary>
        public TierValue<List<int>> equipRing;
        /// <summary>偏好武器属性列表，对应 JSON 字段 equipWeapon。</summary>
        public TierValue<List<int>> equipWeapon;
        /// <summary>姓，对应 JSON 字段 FirstName。</summary>
        public TierValue<string> firstName;
        /// <summary>化神领域，对应 JSON 字段 HuaShenLingYu。</summary>
        public TierValue<HuaShenLingYu> huaShenLingYu;
        /// <summary>金丹类型列表，对应 JSON 字段 JinDanType。</summary>
        public TierValue<List<JinDanType>> jinDanType;
        /// <summary>灵根列表，对应 JSON 字段 LingGen。</summary>
        public TierValue<List<int>> lingGen;
        /// <summary>NPC 势力，对应 JSON 字段 MengPai。</summary>
        public TierValue<MengPai> mengPai;
        /// <summary>NPC 标签列表，对应 JSON 字段 NPCTag。</summary>
        public TierValue<List<NPCTag>> npcTag;
        /// <summary>拍卖分组列表，对应 JSON 字段 paimaifenzu。</summary>
        public TierValue<List<PaiMaiFenZu>> paiMaiFenZu;
        /// <summary>战斗力区间 [最小值, 最大值]，对应 JSON 字段 ShiLi。</summary>
        public TierValue<List<int>> shiLi;
        /// <summary>绑定技能列表，支持字符串引用，对应 JSON 字段 skills。</summary>
        public TierValue<List<object>> skills;
        /// <summary>绑定功法列表，支持字符串引用，对应 JSON 字段 staticSkills。</summary>
        public TierValue<List<object>> staticSkills;
        /// <summary>NPC 类型（所属门派/势力），对应 JSON 字段 Type。</summary>
        public TierValue<Type> type;
        /// <summary>悟道类型，对应 JSON 字段 wudaoType。</summary>
        public TierValue<int> wudaoType;
        /// <summary>感兴趣物品类型，对应 JSON 字段 XinQuType。</summary>
        public TierValue<int> xinQuType;
        /// <summary>元婴功法，支持字符串引用，对应 JSON 字段 yuanying。</summary>
        public TierValue<object> yuanYing;
    }

    /// <summary>
    /// NPC 类型实例数据模型。由 NPCLeiXingData 展开生成，每个实例对应一个具体境界。
    /// </summary>
    public class NPCLeiXingInstanceData
    {
        /// <summary>是否为新建实例（若游戏中已存在则复用 ID）。</summary>
        public bool isNew;

        /// <summary>注册表键名，由包数据 key + 等级数字生成（如 "金虹蓄势NPC10"）。</summary>
        public string key;

        /// <summary>最终写入 NPCLeiXingDate 的主键 id。</summary>
        public int id;

        /// <summary>NPC 流派编号（LiuPai），同一流派所有境界共用此编号。</summary>
        public int liuPai;
        /// <summary>NPC 境界（Level）。</summary>
        public int level;

        // ======================== NPC 字段 ========================

        /// <summary>攻击类型，对应 JSON 字段 AttackType。</summary>
        public int? attackType;
        /// <summary>种族，对应 JSON 字段 AvatarType。</summary>
        public AvatarType? avatarType;
        /// <summary>是否参加拍卖，对应 JSON 字段 canjiaPaiMai。</summary>
        public bool? canJiaPaiMai;
        /// <summary>防御类型，对应 JSON 字段 DefenseType。</summary>
        public int? defenseType;
        /// <summary>偏好防具属性列表，对应 JSON 字段 equipClothing。</summary>
        public List<int> equipClothing;
        /// <summary>偏好饰品属性列表，对应 JSON 字段 equipRing。</summary>
        public List<int> equipRing;
        /// <summary>偏好武器属性列表，对应 JSON 字段 equipWeapon。</summary>
        public List<int> equipWeapon;
        /// <summary>姓，对应 JSON 字段 FirstName。</summary>
        public string firstName;
        /// <summary>化神领域，对应 JSON 字段 HuaShenLingYu。</summary>
        public HuaShenLingYu? huaShenLingYu;
        /// <summary>金丹类型列表，对应 JSON 字段 JinDanType。</summary>
        public List<JinDanType> jinDanType;
        /// <summary>灵根列表，对应 JSON 字段 LingGen。</summary>
        public List<int> lingGen;
        /// <summary>NPC 势力，对应 JSON 字段 MengPai。</summary>
        public MengPai? mengPai;
        /// <summary>NPC 标签列表，对应 JSON 字段 NPCTag。</summary>
        public List<NPCTag> npcTag;
        /// <summary>拍卖分组列表，对应 JSON 字段 paimaifenzu。</summary>
        public List<PaiMaiFenZu> paiMaiFenZu;
        /// <summary>战斗力区间，对应 JSON 字段 ShiLi。</summary>
        public List<int> shiLi;
        /// <summary>绑定技能列表，对应 JSON 字段 skills。</summary>
        public List<object> skills;
        /// <summary>绑定功法列表，对应 JSON 字段 staticSkills。</summary>
        public List<object> staticSkills;
        /// <summary>NPC 类型，对应 JSON 字段 Type。</summary>
        public Type? type;
        /// <summary>悟道类型，对应 JSON 字段 wudaoType。</summary>
        public int? wudaoType;
        /// <summary>感兴趣物品类型，对应 JSON 字段 XinQuType。</summary>
        public int? xinQuType;
        /// <summary>元婴功法，对应 JSON 字段 yuanying。</summary>
        public object yuanYing;
    }
}