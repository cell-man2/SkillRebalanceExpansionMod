using System;

namespace SkillRebalanceExpansionMod
{
    public enum DataCategory
    {
        Buff,
        StaticSkill,
        Skill,
        Item,
        Shop,
        NPCLeiXing
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DataBaseAttribute : Attribute
    {
        public DataCategory Category { get; }     // 分类
        public string Group { get; }        // 流派
        public string Item { get; }         // 具体项目

        public DataBaseAttribute(DataCategory category, string group, string item)
        {
            Category = category;
            Group = group;
            Item = item;
        }
    }
}