using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.NPCLeiXing
{
    /// <summary>
    /// 【请替换为流派名称】NPC 类型数据包。
    /// </summary>
    /// <remarks>
    /// 定位字段：
    ///   key         - 注册表键名前缀，展开后每个境界实例的 key = 此 key + 等级数字（如 "金虹蓄势NPC10"），
    ///                 对应 Registry.npcLeiXing 的 Key。
    ///   realLiuPai  - 绝对流派编号，直接作为 LiuPai 写入 JSON（与 localLiuPai 二选一）。
    ///   localLiuPai - 本地偏移流派编号，最终 LiuPai = baseLiuPai(680) + localLiuPai（与 realLiuPai 二选一）。
    ///   level       - 涵盖的 NPC 境界列表，如 [10, 11, 12, 13, 14, 15]，每个境界展开为一个实例。
    /// 
    /// 必填字段：
    ///   （无强制必填，但 level 至少包含一个境界，否则数据包无实际内容）
    /// 
    /// 选填字段（使用 TierValue&lt;T&gt; 按境界配置，不填则不覆盖该境界的对应字段）：
    ///   attackType     - 攻击类型（对应 JSON 字段 AttackType）
    ///   avatarType     - 种族（对应 JSON 字段 AvatarType）
    ///   canJiaPaiMai   - 是否参加拍卖（对应 JSON 字段 canjiaPaiMai），true = 参加（写入 0），false = 不参加（写入 1）
    ///   defenseType    - 防御类型（对应 JSON 字段 DefenseType）
    ///   equipClothing  - 偏好防具属性列表（对应 JSON 字段 equipClothing）
    ///   equipRing      - 偏好饰品属性列表（对应 JSON 字段 equipRing）
    ///   equipWeapon    - 偏好武器属性列表（对应 JSON 字段 equipWeapon）
    ///   firstName      - 姓（对应 JSON 字段 FirstName）
    ///   huaShenLingYu  - 化神领域（对应 JSON 字段 HuaShenLingYu）
    ///   jinDanType     - 金丹类型列表（对应 JSON 字段 JinDanType）
    ///   lingGen        - 灵根列表（对应 JSON 字段 LingGen）
    ///   mengPai        - NPC 势力（对应 JSON 字段 MengPai）
    ///   npcTag         - NPC 标签列表（对应 JSON 字段 NPCTag）
    ///   paiMaiFenZu    - 拍卖分组列表（对应 JSON 字段 paimaifenzu）
    ///   shiLi          - 战斗力区间 [最小值, 最大值]（对应 JSON 字段 ShiLi）
    ///   skills         - 绑定技能列表，支持字符串引用（对应 JSON 字段 skills）
    ///   staticSkills   - 绑定功法列表，支持字符串引用（对应 JSON 字段 staticSkills）
    ///   type           - NPC 类型/所属门派（对应 JSON 字段 Type）
    ///   wudaoType      - 悟道类型（对应 JSON 字段 wudaoType）
    ///   xinQuType      - 感兴趣物品类型（对应 JSON 字段 XinQuType）
    ///   yuanYing       - 元婴功法，支持字符串引用（对应 JSON 字段 yuanying）
    /// </remarks>
    [DataBase(DataCategory.NPCLeiXing, "星河水剑", "常规NPC修改")]
    public static class 星河水剑NPC
    {
        private static readonly List<int> level = [10, 11, 12, 13, 14, 15];

        public static List<NPCLeiXingData> Data = CreateData();

        private static List<NPCLeiXingData> CreateData()
        {
            List<NPCLeiXingData> result = [];

            NPCLeiXingData data = new()
            {
                key = "常规NPC修改",
                realLiuPai = (int)LiuPai.星河剑修,
                // localLiuPai = ,
                level = level,

                skills = new TierValue<List<object>>
                {
                    [10] = [ 221, 224, 222, 226, 227, 228, 814, 225, 504, 501 ],
                    [11] = [ 221, 224, 222, 226, 227, 228, 814, 225, 504, 501 ],
                    [12] = [ 221, 224, 222, 226, 227, 228, 814, 225, 504, 501 ],
                    [13] = [ 221, 224, 222, 226, 227, 228, 814, 225, 504, 501 ],
                    [14] = [ 221, 224, 222, 226, 227, 228, 814, 225, 504, 501 ],
                    [15] = [ 221, 224, 222, 226, 227, 228, 814, 225, 504, 501 ],
                },
            };

            result.Add(data);
            return result;
        }
    }
}