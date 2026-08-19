using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Models.Item
{
    public class ItemData
    {
        // 注册定位
        public string key;

        public int? localId;
        public int? realId;
        
        public string skillKey;

        public bool qingJiao = false;

        public string name;
        public int? itemIcon;
        public int? maxNum;
        public Type? type;
        public int? quality;
        public int? typePinJie;
        public TuJianType? tuJianType;
        public ShopType? shopType;
        public List<ItemFlag> itemFlag;
        public int? stuTime;
        public List<(DaoType type, DaoLevel level)> wuDao;
        public string desc;
        public string desc2;
        public Dictionary<object, Dictionary<string, object>> seidData;
        public int? price;
        public bool? canSale;
        public string faBaoType;
        public List<int> affix;
        public int? vagueType;
        public int? canUse;
        public int? danDu;
        public bool? npcCanUse;
        public int? shuXingType;
        public int? wuWeiType;
        public int? shuaXin;
        public int? yaoZhi1;
        public int? yaoZhi2;
        public int? yaoZhi3;
    }

    public class BookInfo
    {
        public BookType type;

        public int skillId;
        public int? jie;
        public int? pin;
        public bool? qingJiaoType;
    }
}