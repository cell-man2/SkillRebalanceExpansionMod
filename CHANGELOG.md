# SkillRebalanceExpansionMod 更新日志

## [1.1.10] - 2026-08-27

> 本次更新主要优化扩展框架，不涉及具体流派内容。

### 新增

- 新增自定义内容开发指南到 Doc 文件夹下。
  - 包含 Buff、Item、NPC、Shop、Skill、StaticSkill 配置说明。
  - 补充字段说明、使用方式及完整示例。

### 修改

- 更新流派模板，使其有更好的编写体验。

### 修复

- 移除了之前错误添加到 Item 工厂的 seid 默认值。

### 优化

- 补全 Models、Factories 及模板代码注释。
- 明确数据字段与游戏 JSONObject 字段之间的映射关系。
- 为流派改动拆分出CHANGELOG.md和DEVLOG.md，记录一下改动的心路历程。

## [1.1.0] - 2026-08-19

> 新增流派「金虹蓄势」。

详细更新：

- [金虹蓄势更新日志](Data/01.金虹蓄势/CHANGELOG.md)


## [1.0.0] - 2026-08-19

> Mod 基础框架初版。

### 新增

- 新增 Mod 基础加载框架。

- 新增自定义内容注册与加载机制。

- 新增内容模板体系，用于快速创建新的流派扩展内容。

#### 工厂系统

- 新增 `BuffFactory`。
  - 支持自定义 Buff 数据生成与注入。

- 新增 `ItemFactory`。
  - 支持自定义物品、功法书等物品数据生成与注入。

- 新增 `SkillFactory`。
  - 支持自定义神通数据生成与注入。

- 新增 `StaticSkillFactory`。
  - 支持静态技能相关数据生成与注入。

- 新增 `ShopFactory`。
  - 支持自定义商店商品配置与注入。

- 新增 `NPCLeiXingFactory`。
  - 支持 NPC 类型相关内容扩展。


#### 数据模板

- 新增 `Data/00.流派模板`。

- 新增 Buff 模板。

- 新增 Item 模板。

- 新增 NPC 模板。

- 新增 Shop 模板。

- 新增 Skill 模板。

- 新增 StaticSkill 模板。


### 修改

- 建立统一的数据定义结构。

- 建立 Models 层，用于规范各类自定义内容的数据模型。

- 建立 Registry 注册机制，统一管理自定义内容加载。