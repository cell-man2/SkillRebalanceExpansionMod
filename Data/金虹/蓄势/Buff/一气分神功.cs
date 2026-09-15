using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Buff;

namespace SkillRebalanceExpansionMod.Data.Buff
{
    [DataBase(DataCategory.Buff, "金虹蓄势", "一气分神功")]
    public static class 一气分神功
    {
        public static readonly List<BuffData> Data =
        [
            new BuffData
            {
                key = "一气分神功(上限为零)",
                realId = 106,
                isHide = true
            },
            new BuffData
            {
                key = "一气分神功(吸收灵气&展示)",
                realId = 107,
                descr = "灵气上限为0；吸收灵气阶段，多吸收（attack）点灵气"
            }
        ];
    }
}