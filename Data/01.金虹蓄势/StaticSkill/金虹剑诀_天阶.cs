using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data
{
    [DataBase(DataCategory.StaticSkill, "金虹剑诀", "天阶金虹剑诀")]
    public static class 金虹剑诀_天阶
    {
        private static readonly List<int> skillLv = [1,2,3,4,5];
        public static List<StaticSkillData> Data =
        [
            new StaticSkillData
            {
                key = "金虹剑诀_天阶",
                localId = 1,
                skillLv = skillLv,
                name = TierValue<string>.Create(tier => $"金虹剑诀{tier}", skillLv),
                qingJiaoType = QingJiaoType.宁州不传,
                descr = TierValue<string>.Create(
                    tier => $"每使用或消散一点金系灵气，获得【蓄势】*{tier * 2}；" +
                        $"每回合第一次剑系伤害提升{tier * 5 + 5}%。",
                    skillLv
                ),
                attackType = AttackType.剑,
                icon = 4,
                skillStyle = SkillStyle.战斗,
                skillJie = SkillJie.天,
                skillPin = SkillPin.上,
                df = true,
                tuJianType = TuJianType.功法,
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new Dictionary<object, Dictionary<string, object>>
                    {
                        {
                            1,
                            new()
                            {
                                { "target", 1 },
                                {
                                    "value1",
                                    new List<string> {
                                        "@buff:金虹剑诀_天阶(伤害提升&展示)",
                                        "@buff:金虹剑诀_天阶(消散灵气)",
                                        "@buff:金虹剑诀_天阶(使用灵气)"
                                    }
                                },
                                {
                                    "value2",
                                    new List<int> {
                                        5 * tier + 5,
                                        2 * tier,
                                        2 * tier
                                    }
                                }
                            }
                        }
                    },
                    skillLv
                )
            }
        ];
    }
}