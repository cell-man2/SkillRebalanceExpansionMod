# SkillRebalanceExpansionMod

## [1.0.0] - 2026-08-19

### 新增

- 新增Mod基础加载框架
- 新增自定义内容注册与加载机制
- 新增内容模板体系，用于快速创建新的流派扩展内容

#### 工厂系统

- 新增 `BuffFactory`
  - 支持自定义Buff数据生成与注入

- 新增 `ItemFactory`
  - 支持自定义物品、功法书等物品数据生成与注入

- 新增 `SkillFactory`
  - 支持自定义神通数据生成与注入

- 新增 `StaticSkillFactory`
  - 支持静态技能相关数据生成与注入

- 新增 `ShopFactory`
  - 支持自定义商店商品配置与注入

- 新增 `NPCLeiXingFactory`
  - 支持NPC类型相关内容扩展

#### 数据模板

- 新增 `Data/00.流派模板`
- 新增Buff模板
- 新增Item模板
- 新增NPC模板
- 新增Shop模板
- 新增Skill模板
- 新增StaticSkill模板

### 修改

- 建立统一的数据定义结构
- 建立 Models 层，用于规范各类自定义内容的数据模型
- 建立 Registry 注册机制，统一管理自定义内容加载

