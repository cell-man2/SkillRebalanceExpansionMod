using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.StaticSkill
{
    [DataBase(DataCategory.StaticSkill, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];

        public static readonly List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "金虹剑诀_天阶",
                localId = 1,
                skillLv = skillLv,
                name = TierValue<string>.Create(tier => $"金虹剑诀{tier}", skillLv),
                descr = TierValue<string>.Create(
                    tier => $"每使用或消散一点金系灵气，获得【蓄势】*{tier * 2}；" +
                        $"每回合第一次剑系伤害提升{tier * 5 + 5}%。",
                    skillLv
                ),
                attackType = AttackType.剑,
                df = true,
                icon = 4,
                qingJiaoType = QingJiaoType.宁州不传,
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new Dictionary<object, Dictionary<string, object>>
                    {
                        [1] = new()
                        {
                            ["target"] = 1,
                            ["value1"] = new List<object>
                            {
                                "@buff:金虹剑诀_天阶(伤害提升&展示)",
                                "@buff:金虹剑诀_天阶(消散灵气)",
                                "@buff:金虹剑诀_天阶(使用灵气)"
                            },
                            ["value2"] = new List<object>
                            {
                                5 * tier + 5,
                                2 * tier,
                                2 * tier
                            }
                        }
                    },
                    skillLv
                ),
                skillJie = SkillJie.天,
                skillPin = SkillPin.上,
                skillStyle = SkillStyle.战斗,
                tuJianType = TuJianType.功法,
            }
        ];
    }
}