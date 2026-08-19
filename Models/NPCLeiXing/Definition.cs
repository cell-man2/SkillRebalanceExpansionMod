using System.Collections.Generic;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Models.NPCLeiXing
{
    public class NPCLeiXingData
    {
        // 注册定位
        public string key;

        // NPC流派
        public int? realLiuPai;
        public int? localLiuPai;

        // 涵盖NPC境界 Level
        public List<int> level;

        // NPC类型
        public TierValue<Type> type;
        // NPC势力
        public TierValue<MengPai> mengPai;
        // 绑定技能
        public TierValue<List<object>> skills;
        // 绑定功法
        public TierValue<List<object>> staticSkills;
        // 金丹类型
        public TierValue<List<JinDanType>> jinDanType;
        // 元婴功法
        public TierValue<object> yuanYing;
        // 化神领域
        public TierValue<HuaShenLingYu> huaShenLingYu;
        // 灵根
        public TierValue<List<int>> lingGen;
        // 悟道类型
        public TierValue<int> wudaoType;
        // 标签
        public TierValue<List<NPCTag>> npcTag;
        // 是否参加拍卖
        public TierValue<bool> canJiaPaiMai;
        // 拍卖分组
        public TierValue<List<PaiMaiFenZu>> paiMaiFenZu;
        // 种族
        public TierValue<AvatarType> avatarType;
        // 感兴趣物品
        public TierValue<int> xinQuType;
        // 偏好武器属性
        public TierValue<List<int>> equipWeapon;
        // 偏好防具属性
        public TierValue<List<int>> equipClothing;
        // 偏好饰品属性
        public TierValue<List<int>> equipRing;
        // 姓
        public TierValue<string> firstName;
        // 战斗力区间
        public TierValue<List<int>> shiLi;
        // 攻击类型
        public TierValue<int> attackType;
        // 防御类型
        public TierValue<int> defenseType;
    }

    public class NPCLeiXingInstanceData
    {
        // 是否新增数据
        public bool isNew;

        // 注册定位
        public string key;

        // 最终 NPCLeiXingDate.id
        public int id;

        // NPC类型
        public Type? type;
        // NPC流派
        public int liuPai;
        // NPC境界
        public int level;
        // NPC势力
        public MengPai? mengPai;
        // 绑定技能
        public List<object> skills;
        // 绑定功法
        public List<object> staticSkills;
        // 金丹类型
        public List<JinDanType> jinDanType;
        // 元婴功法
        public object yuanYing;
        // 化神领域
        public HuaShenLingYu? huaShenLingYu;
        // 灵根
        public List<int> lingGen;
        // 悟道类型
        public int? wudaoType;
        // 标签
        public List<NPCTag> npcTag;
        // 是否参加拍卖
        public bool? canJiaPaiMai;
        // 拍卖分组
        public List<PaiMaiFenZu> paiMaiFenZu;
        // 种族
        public AvatarType? avatarType;
        // 感兴趣物品
        public int? xinQuType;
        // 偏好武器属性
        public List<int> equipWeapon;
        // 偏好防具属性
        public List<int> equipClothing;
        // 偏好饰品属性
        public List<int> equipRing;
        // 姓
        public string firstName;
        // 战斗力区间
        public List<int> shiLi;
        // 攻击类型
        public int? attackType;
        // 防御类型
        public int? defenseType;
    }
}