# NPCLeiXing 编写指南

## 一、概述

本 Mod 采用数据驱动 + 工厂自动注入的方式添加 NPC 类型配置。与 Buff/Item 工厂不同，NPC 类型工厂采用**包（Package）展开模式**：一个 `NPCLeiXingData` 数据包包含多个境界（level）的配置，工厂将其展开为每个境界一个独立的实例，注入到游戏的 `NPCLeiXingDate` 中。

---

## 二、工厂运作机理

### 2.1 整体流程

```
游戏启动
    │
    ▼
Main.Awake()
    │
    ▼
Harmony 补丁挂载到 YSJSONHelper.InitJSONClassData
    │
    ▼
游戏加载 JSON 数据前触发 [HarmonyPrefix]
    │
    ▼
Loader.Start()
    │
    ├── Registry.Clear()
    │
    ├── InitializeFactories()
    │   └── NPCLeiXingFactory.Initialize()
    │       ├── BuildNPCLeiXingIndex()
    │       │   └── 从游戏现有 NPCLeiXingDate 构建 (LiuPai, Level) → id 索引
    │       │
    │       ├── DataManager.Scan<NPCLeiXingData>(DataCategory.NPCLeiXing)
    │       │   └── 扫描程序集中所有带 [DataBase(DataCategory.NPCLeiXing)] 的类
    │       │       └── 读取每个类的静态字段 Data（List<NPCLeiXingData>）
    │       │
    │       ├── 对每个 NPCLeiXingData：
    │       │   ├── 生成 LiuPai（realLiuPai 或 baseLiuPai + localLiuPai）
    │       │   ├── 展开：遍历 level 列表，每个 level 生成一个实例
    │       │   │   ├── 若 (LiuPai, Level) 存在于索引中 → isNew = false，复用旧 id
    │       │   │   ├── 若 (LiuPai, Level) 不存在于索引中 → isNew = true，分配新 id
    │       │   │   └── 实例 key = 包 key + 等级数字（如 "金虹蓄势NPC10"）
    │       │   ├── 注册实例 key → id 到 Registry.npcLeiXing
    │       │   └── 对每个 TierValue 字段，按 level 取值填充到对应实例
    │       │
    │       └── npcLeiXingInstanceDatas 缓存所有展开后的实例供 Inject 使用
    │
    ├── CheckRegistryDuplicates()
    │
    ├── InjectFactories()
    │   └── NPCLeiXingFactory.Inject()
    │       └── 对每个 NPCLeiXingInstanceData：
    │           ├── 若 isNew → 新建 JSON 对象（仅含 id、LiuPai、Level）
    │           ├── 若 !isNew → 获取已有 JSON 对象
    │           └── 将实例的非空字段覆盖到 JSON 上
    │
    ├── 打印所有注册表信息到日志
    │
    └── Registry.Clear()
```

### 2.2 关键机制说明

| 机制 | 说明 |
|------|------|
| 包展开模式 | 一个 `NPCLeiXingData` 包含多个境界，工厂按 `level` 列表展开为多个独立实例 |
| 新建/修改判定 | 通过 `(LiuPai, Level)` 复合键在游戏现有数据中查找：存在则复用 id（修改），不存在则分配新 id（新建） |
| Key 注册 | 实例 key = 包 key + 等级数字（如 `"金虹蓄势NPC10"`），注册到 `Registry.npcLeiXing` |
| TierValue 分阶 | 所有字段使用 `TierValue<T>` 按境界配置，不同境界可配置不同值；未配置的境界字段保持 `null`（不覆盖游戏原值） |
| 无默认值 | NPC 类型数据无 `defaultData`，新建实例仅写入 `id`、`LiuPai`、`Level` 三个基础字段，其余字段由数据显式填充 |
| 查重保护 | 注册前检查所有 ID 是否重复，发现重复则抛出异常防止数据错乱 |

---

## 三、数据模型

详细定义见 `Definition.cs`，以下为速查表。枚举值定义见 `Models/NPCLeiXing/Enums.cs`。

### 3.1 定位字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `key` | `string` | 注册表键名前缀，展开后实例 key = 此 key + 等级数字 |
| `realLiuPai` | `int?` | 绝对流派编号，直接作为 LiuPai 写入 JSON（与 `localLiuPai` 二选一） |
| `localLiuPai` | `int?` | 本地偏移流派编号，最终 LiuPai = baseLiuPai(680) + localLiuPai（与 `realLiuPai` 二选一） |
| `level` | `List<int>` | 涵盖的 NPC 境界列表，如 `[10, 11, 12, 13, 14, 15]`，每个境界展开为一个实例 |

### 3.2 必填字段

无强制必填字段，但 `level` 至少包含一个境界，否则数据包无实际内容。

### 3.3 选填字段（使用 TierValue<T> 按境界配置）

所有字段均为选填。使用 `TierValue<T>` 按境界配置，不填则不覆盖该境界的对应字段。

| 字段 | 类型 | JSON 字段 | 说明 |
|------|------|-----------|------|
| `attackType` | `TierValue<int>` | `AttackType` | 攻击类型 |
| `avatarType` | `TierValue<AvatarType>` | `AvatarType` | 种族 |
| `canJiaPaiMai` | `TierValue<bool>` | `canjiaPaiMai` | 是否参加拍卖（true = 参加，写入 0；false = 不参加，写入 1） |
| `defenseType` | `TierValue<int>` | `DefenseType` | 防御类型 |
| `equipClothing` | `TierValue<List<int>>` | `equipClothing` | 偏好防具属性列表 |
| `equipRing` | `TierValue<List<int>>` | `equipRing` | 偏好饰品属性列表 |
| `equipWeapon` | `TierValue<List<int>>` | `equipWeapon` | 偏好武器属性列表 |
| `firstName` | `TierValue<string>` | `FirstName` | 姓 |
| `huaShenLingYu` | `TierValue<HuaShenLingYu>` | `HuaShenLingYu` | 化神领域 |
| `jinDanType` | `TierValue<List<JinDanType>>` | `JinDanType` | 金丹类型列表 |
| `lingGen` | `TierValue<List<int>>` | `LingGen` | 灵根列表 |
| `mengPai` | `TierValue<MengPai>` | `MengPai` | NPC 势力 |
| `npcTag` | `TierValue<List<NPCTag>>` | `NPCTag` | NPC 标签列表 |
| `paiMaiFenZu` | `TierValue<List<PaiMaiFenZu>>` | `paimaifenzu` | 拍卖分组列表 |
| `shiLi` | `TierValue<List<int>>` | `ShiLi` | 战斗力区间 `[最小值, 最大值]` |
| `skills` | `TierValue<List<object>>` | `skills` | 绑定技能列表，支持字符串引用 |
| `staticSkills` | `TierValue<List<object>>` | `staticSkills` | 绑定功法列表，支持字符串引用 |
| `type` | `TierValue<Type>` | `Type` | NPC 类型/所属门派 |
| `wudaoType` | `TierValue<int>` | `wudaoType` | 悟道类型 |
| `xinQuType` | `TierValue<int>` | `XinQuType` | 感兴趣物品类型 |
| `yuanYing` | `TierValue<object>` | `yuanying` | 元婴功法，支持字符串引用 |

---

## 四、新建与修改的判定

### 4.1 核心规则

工厂通过 `(LiuPai, Level)` 复合键自动判定新建/修改：

| 场景 | 判定条件 | 行为 |
|------|---------|------|
| 新建实例 | `(LiuPai, Level)` 不在游戏现有数据中 | 分配新 id（baseId + 递增），`isNew = true` |
| 修改已有实例 | `(LiuPai, Level)` 在游戏现有数据中 | 复用旧 id，`isNew = false`，以更新模式覆盖字段 |

### 4.2 设计意图

- 新建：希望添加游戏中原本不存在的 NPC 流派配置（如新增流派或现有流派新增境界）。
- 修改：希望修改游戏中已有 NPC 的配置（如调整技能、功法、属性等）。只需填写要覆盖的字段即可，未填字段保持游戏原值。

---

## 五、注册表引用系统

### 5.1 NPCLeiXing 注册

工厂展开实例后，将每个实例的 `key` 和 `id` 注册到 `Registry.npcLeiXing`：

```
Registry.npcLeiXing["金虹蓄势NPC10"] = 2501
Registry.npcLeiXing["金虹蓄势NPC11"] = 2502
```

### 5.2 引用 NPCLeiXing ID

在任意支持引用的字段中，使用 `"@npcLeiXing:XXX"` 格式来引用 `key="XXX"` 的 NPCLeiXing 实例的 ID：

```
"@npcLeiXing:金虹蓄势NPC10"    → 解析为 Registry.npcLeiXing["金虹蓄势NPC10"] 的值（int ID）
```

### 5.3 支持引用的字段

| 字段 | 支持引用类型 | 说明 |
|------|-------------|------|
| `skills` | `@sId:XXX`、直接 int ID | 技能列表，字符串引用解析为技能 `Skill_ID` |
| `staticSkills` | `@staticSkill:XXX`、直接 int ID | 功法列表，字符串引用解析为功法 `id` |
| `yuanYing` | `@staticSkill:XXX`、直接 int ID | 元婴功法，字符串引用解析为功法 `id` |

### 5.4 引用解析机制

工厂在处理 `skills`、`staticSkills`、`yuanYing` 字段时，会对数组/对象中的每个元素调用 `Registry.Resolve()` 进行解析：

- 遇到 `@sId:蓄势_天阶` → 查 `Registry.sId` 表，返回技能 ID
- 遇到 `@staticSkill:金虹剑诀_天阶5` → 查 `Registry.staticSkill` 表，返回功法 ID
- 遇到直接数字（如 `17`、`60`）→ 原样返回

---

## 六、TierValue 使用指南

### 6.1 数据结构

```
new TierValue<T>
{
    [境界1] = 值1,
    [境界2] = 值2,
    // ...
}
```

### 6.2 境界编号参考

## 境界编号参考

| 大境界 | 编号 |
|--------|------|
| 炼气 | 前期 1 / 中期 2 / 后期 3 |
| 筑基 | 前期 4 / 中期 5 / 后期 6 |
| 金丹 | 前期 7 / 中期 8 / 后期 9 |
| 元婴 | 前期 10 / 中期 11 / 后期 12 |
| 化神 | 前期 13 / 中期 14 / 后期 15 |

### 6.3 使用示例

```csharp
// 不同境界配置不同技能列表
skills = new TierValue<List<object>>
{
    [10] = ["@sId:蓄势_天阶", 17, 16, 12],
    [11] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28],
    [12] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31],
    [13] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10],
};

// 不同境界配置不同功法
staticSkills = new TierValue<List<object>>
{
    [11] = ["@staticSkill:金虹剑诀_天阶5", 60, 65],
    [12] = ["@staticSkill:金虹剑诀_天阶5", 60, 65, 5245],
    [13] = ["@staticSkill:金虹剑诀_天阶5", 60, 65, 5245, 90],
};
```

### 6.4 注意事项

- 若某境界未配置，该字段在该境界实例中保持 `null`，工厂不会写入该字段（即不覆盖游戏原有值）
- 配置的境界必须在数据包的 `level` 列表中，否则该配置不会被使用
- `TierValue<T>` 支持隐式转换为单值：`new TierValue<int> { 默认值 }` 或直接赋值 `TierValue<int> value = 10`，但 NPC 配置中通常按境界区分，建议使用索引器形式

---

## 七、编写步骤

### 7.1 新建 NPC 类型包

1. 复制 `NPCLeiXingTemplate.cs` 到 `Data/NPCLeiXing/` 目录下
2. 修改类名为合适的名称
3. 修改 `[DataBase]` 中的 `group` 和 `item` 参数
4. 在类顶部定义 `level` 数组，列出该流派涉及的所有境界
5. 在 `CreateData()` 中填写定位字段（`key`、`realLiuPai`/`localLiuPai`）
6. 按需填写选填字段（使用 `TierValue<T>` 按境界配置）
7. 编译运行，查看日志确认注册成功

### 7.2 修改已有 NPC 类型

1. 创建 `NPCLeiXingData`，填写 `realLiuPai`（或 `localLiuPai`）和 `level` 定位目标
2. 仅填写需要修改的字段（使用 `TierValue<T>` 按境界配置要覆盖的境界和值）
3. 工厂会自动匹配 `(LiuPai, Level)`，以更新模式覆盖对应字段，未填字段保持游戏原值

---

## 八、完整示例

### 示例：修改徐凡（金虹蓄势流派）各境界技能配置

```csharp
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.NPCLeiXing;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.NPCLeiXing
{
    /// <summary>
    /// 徐凡（金虹蓄势流派）NPC 类型数据。
    /// </summary>
    [DataBase(DataCategory.NPCLeiXing, "金虹蓄势", "固定NPC修改")]
    public static class 徐凡NPC
    {
        private static readonly List<int> level = [10, 11, 12, 13, 14, 15];

        public static List<NPCLeiXingData> Data = CreateData();

        private static List<NPCLeiXingData> CreateData()
        {
            List<NPCLeiXingData> result = [];

            NPCLeiXingData data = new()
            {
                key = "金虹蓄势NPC",
                realLiuPai = 14,                    // 徐凡的 LiuPai 编号
                level = level,

                skills = new TierValue<List<object>>
                {
                    [10] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [11] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [12] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [13] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [14] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                    [15] = ["@sId:蓄势_天阶", 17, 16, 12, 6, 28, 31, 10, 501, 504],
                },

                staticSkills = new TierValue<List<object>>
                {
                    [11] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [12] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [13] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [14] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                    [15] = [60, 65, 5245, 90, "@staticSkill:金虹剑诀_天阶5"],
                },
            };

            result.Add(data);
            return result;
        }
    }
}
```