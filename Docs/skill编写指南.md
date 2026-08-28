# Skill 编写指南

## 一、概述

本 Mod 采用数据驱动 + 工厂自动注入的方式添加神通。与 StaticSkill 工厂类似，Skill 工厂采用**包（Package）展开模式**：一个 `SkillData` 数据包包含多个等级（skillLv）的配置，工厂将其展开为每个等级一个独立的实例，注入到游戏的 `_skillJsonData` 中。

神通工厂在包展开的基础上，额外处理了**灵气消耗转换**、**AI 行为数据注入**以及**词缀/图鉴自动生成**。

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
    │   └── SkillFactory.Initialize()
    │       ├── BuildSkillIndex()
    │       │   └── 从游戏现有 _skillJsonData 构建 (Skill_ID, Skill_Lv) → id 索引
    │       │
    │       ├── DataManager.Scan<SkillData>(DataCategory.Skill)
    │       │   └── 扫描程序集中所有带 [DataBase(DataCategory.Skill)] 的类
    │       │       └── 读取每个类的静态字段 Data（List<SkillData>）
    │       │
    │       ├── 对每个 SkillData：
    │       │   ├── 生成 Skill_ID（realId 或 baseSkillId + localId）
    │       │   ├── 展开：遍历 skillLv 列表，每个等级生成一个实例
    │       │   │   ├── 若 (Skill_ID, Skill_Lv) 存在于索引中 → isNew = false，复用旧 id
    │       │   │   ├── 若 (Skill_ID, Skill_Lv) 不存在于索引中 → isNew = true，分配新 id
    │       │   │   └── 实例 key = 包 key + 等级数字（如 "蓄势_天阶1"）
    │       │   ├── 若非新建且有 seidData → 收集旧 seid 供清理
    │       │   ├── 若非新建且有 aiData → 遍历 AIJsonDate 收集旧 AI 数据供清理
    │       │   ├── 注册实例 key → id 到 Registry.skill
    │       │   ├── 注册包 key → Skill_ID 到 Registry.sId
    │       │   └── 注册包 key → BookInfo 到 Registry.bookInfo（type = 神通）
    │       │
    │       └── skillInstanceDatas 缓存所有展开后的实例供 Inject 使用
    │
    ├── CheckRegistryDuplicates()
    │
    ├── InjectFactories()
    │   └── SkillFactory.Inject()
    │       ├── RemoveOldSkillSeid()
    │       │   └── 遍历 removeSkillSeid，从各 seid 表中移除旧数据
    │       │
    │       ├── RemoveOldSkillAI()
    │       │   └── 遍历 removeSkillAI，从各 AI 表中移除旧数据
    │       │
    │       └── 对每个 SkillInstanceData：
    │           ├── 若 isNew → 新建 JSON 对象（id + Skill_ID + Skill_Lv + defaultData）
    │           │   └── 调用 InitNewSkill()：若有 descr → 自动提取 Affix2 并生成 TuJiandescr
    │           ├── 若 !isNew → 获取已有 JSON 对象
    │           ├── 将实例的非空字段覆盖到 JSON 上
    │           ├── 注入 seidData 到 SkillSeidJsonData
    │           └── 注入 aiData 到 AIJsonDate
    │
    ├── 打印所有注册表信息到日志
    │
    └── Registry.Clear()
```

### 2.2 关键机制说明

| 机制 | 说明 |
|------|------|
| 包展开模式 | 一个 `SkillData` 包含多个等级，工厂按 `skillLv` 列表展开为多个独立实例 |
| 新建/修改判定 | 通过 `(Skill_ID, Skill_Lv)` 复合键判定：存在则复用 id（修改），不存在则分配新 id（新建） |
| 灵气消耗转换 | `cost` 元组列表自动拆分为 `skill_CastType` + `skill_Cast` + `skill_SameCastNum` |
| 词缀/图鉴自动生成 | 新建实例时，若提供了 `descr` 且未单独指定 `affix2`/`tuJianDescr`，工厂自动从描述中提取词缀并生成图鉴描述 |
| AI 行为注入 | `aiData` 注入到 `AIJsonDate[AI编号][技能实例ID]` |
| 双重清理 | 同时维护 seid 清理和 AI 清理两套系统，AI 清理需全量遍历 `AIJsonDate` |
| 多重注册 | 包 key 同时注册到 `Registry.sId` 和 `Registry.bookInfo`，实例 key 注册到 `Registry.skill` |
| 查重保护 | 注册前检查所有 ID 是否重复，发现重复则抛出异常防止数据错乱 |

---

## 三、数据模型

详细定义见 `Definition.cs`，以下为速查表。枚举值定义见 `Models/Skill/Enums.cs`。

### 3.1 定位字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `key` | `string` | 注册表键名前缀，展开后实例 key = 此 key + 等级数字 |
| `localId` | `int?` | 本地偏移 ID，最终 Skill_ID = baseSkillId(4370) + localId（与 `realId` 二选一） |
| `realId` | `int?` | 绝对 ID，直接作为 Skill_ID 写入 JSON（与 `localId` 二选一） |
| `skillLv` | `List<int>` | 涵盖的神通等级列表，如 `[1,2,3,4,5]`，每个等级展开为一个实例 |

### 3.2 必填字段

| 字段 | 类型 | JSON 字段 | 说明 |
|------|------|-----------|------|
| `attackType` | `TierValue<List<AttackType>>` | `AttackType` | 攻击类型列表（可多选） |
| `cost` | `List<(CardType, int)>` | `skill_CastType` + `skill_Cast` + `skill_SameCastNum` | 灵气消耗列表，参见第四节 |
| `descr` | `TierValue<string>` | `descr` | 神通描述。若未填写 `tuJianDescr`，工厂自动从此字段生成图鉴描述 |
| `name` | `TierValue<string>` | `name` | 神通名称 |
| `script` | `TierValue<Script>` | `script` | 执行脚本类型（对敌人/对自己） |
| `seidData` | `TierValue<Dictionary<object, Dictionary<string, object>>>` | `seid` | 特性列表，神通实际效果的核心 |
| `skillEffect` | `TierValue<string>` | `skillEffect` | 技能特效动画 |

### 3.3 选填字段（使用 TierValue<T> 按等级配置）

| 字段 | 类型 | JSON 字段 | 默认值 |
|------|------|-----------|--------|
| `affix` | `TierValue<List<int>>` | `Affix` | `[]` |
| `affix2` | `TierValue<List<int>>` | `Affix2` | `[]`（若未填写且提供了 `descr`，工厂自动从 `descr` 提取） |
| `aiData` | `TierValue<Dictionary<object, Dictionary<string, object>>>` | — | `null`（注入到 AIJsonDate） |
| `canUseDistMax` | `TierValue<int>` | `canUseDistMax` | `30` |
| `cd` | `TierValue<float>` | `CD` | `10000` |
| `df` | `TierValue<bool>` | `DF` | `0`（false） |
| `hp` | `TierValue<int>` | `HP` | `0` |
| `icon` | `TierValue<int>` | `icon` | `0` |
| `qingJiaoType` | `TierValue<QingJiaoType>` | `qingjiaotype` | `1`（QingJiaoType.普通） |
| `skillCastTime` | `TierValue<int>` | `Skill_castTime` | `1` |
| `skillDisplayType` | `TierValue<SkillDisplayType>` | `Skill_DisplayType` | `0`（目标身上） |
| `skillJie` | `TierValue<SkillJie>` | `Skill_LV` | `1`（SkillJie.人） |
| `skillOpen` | `TierValue<int>` | `Skill_Open` | `1` |
| `skillPin` | `TierValue<SkillPin>` | `typePinJie` | `1`（SkillPin.下） |
| `skillType` | `TierValue<SkillType>` | `Skill_Type` | `20`（废弃） |
| `speed` | `TierValue<int>` | `speed` | `0` |
| `tuJianDescr` | `TierValue<string>` | `TuJiandescr` | 若未填写且提供了 `descr`，工厂自动从 `descr` 生成 |
| `tuJianType` | `TierValue<TuJianType>` | `TuJianType` | `0`（TuJianType.无） |

---

## 四、灵气消耗格式

### 4.1 数据结构

`cost` 为 `List<(CardType type, int amount)>`，其中：

| 类型 | 说明 |
|------|------|
| 元素灵气（金/木/水/火/土/魔） | 按类型累加，写入 `skill_CastType` 和 `skill_Cast` |
| 同系灵气（CardType.同 = 999） | 每次出现独立成为一组同系消耗，写入 `skill_SameCastNum` |

### 4.2 示例

消耗 **3*木 + 2*同 + 2*同**：

```csharp
cost = new List<(CardType, int)>
{
    (CardType.木, 3),
    (CardType.同, 2),
    (CardType.同, 2)
}
```

转换为：

```
skill_CastType = [1]      // 木
skill_Cast = [3]          // 3点
skill_SameCastNum = [2, 2] // 两组同系消耗
```

### 4.3 注意事项

- 同系灵气可多次出现，每次独立为一组消耗
- 元素灵气按类型累加后写入数组，相同类型不会重复出现

---

## 五、新建与修改的判定

### 5.1 核心规则

工厂通过 `(Skill_ID, Skill_Lv)` 复合键自动判定新建/修改：

| 场景 | 判定条件 | 行为 |
|------|---------|------|
| 新建实例 | `(Skill_ID, Skill_Lv)` 不在游戏现有数据中 | 分配新 id（baseId + 递增），`isNew = true`，执行自动计算 |
| 修改已有实例 | `(Skill_ID, Skill_Lv)` 在游戏现有数据中 | 复用旧 id，`isNew = false`，不执行自动计算，以更新模式覆盖字段 |

### 5.2 设计意图

- 新建：希望添加游戏中原本不存在的神通。
- 修改：希望修改游戏中已有神通的某些字段（如描述、seid 效果、AI 行为等）。只需填写要覆盖的字段即可，未填字段保持游戏原值。

---

## 六、注册表引用系统

### 6.1 三层注册

| 注册表 | Key | Value | 引用语法 | 用途 |
|--------|-----|-------|---------|------|
| `Registry.skill` | `"蓄势_天阶1"` | 实例 id | `@skill:蓄势_天阶1` | 引用神通实例（某个具体等级） |
| `Registry.sId` | `"蓄势_天阶"` | Skill_ID | `@sId:蓄势_天阶` | 引用神通编号（如 NPC 配置、seid 引用） |
| `Registry.bookInfo` | `"蓄势_天阶"` | `BookInfo`（type = 神通） | `"@bookInfo:蓄势_天阶"` | 神通书自动填充（ItemFactory 的 `skillKey`） |

### 6.2 支持引用的字段

| 字段 | 支持引用类型 | 说明 |
|------|-------------|------|
| `seidData` 的 Key | 直接 int | seid 编号 |
| `seidData` 的 Value | `@buff:xxx`、`@sId:xxx`、`@staticSkill:xxx`、直接值 | 所有参数值均可使用注册表引用 |
| `aiData` 的 Key | 直接 int | AI 编号 |
| `aiData` 的 Value | 直接值 | AI 参数值，通常为 int/float/bool |

工厂在处理 `seidData` 和 `aiData` 时会递归解析所有 `object` 值，遇到字符串 `@类型:key` 则查表替换为对应 ID，遇到数组/字典则递归处理内部元素。

---

## 七、TierValue 使用指南

### 7.1 数据结构

```
new TierValue<T>
{
    [等级1] = 值1,
    [等级2] = 值2,
    // ...
}
```

或使用工厂方法批量生成：

```
TierValue<T>.Create(tier => 表达式, skillLv列表)
```

### 7.2 使用示例

```csharp
// 按等级生成名称
name = TierValue<string>.Create(tier => $"蓄势·{tier}层", skillLv);

// 按等级生成描述
descr = TierValue<string>.Create(
    tier => $"本回合每使用一点金系灵气，获得【蓄势】*{tier}",
    skillLv
);

// 按等级生成 HP
hp = TierValue<int>.Create(tier => tier * 20, skillLv);

// 单值（所有等级共用）
skillJie = SkillJie.天;
skillPin = SkillPin.上;
attackType = new List<AttackType> { AttackType.剑, AttackType.金 };
```

### 7.3 注意事项

- 若某等级未配置，该字段在该等级实例中保持默认值
- 配置的等级必须在数据包的 `skillLv` 列表中
- `TierValue<T>` 支持隐式单值转换，适用于所有等级共用同一值的场景
- `cost` 为 `List<(CardType, int)>` 类型，**不推荐**使用 `TierValue` 包装（每个等级消耗结构不同，直接在数据包内定义即可）

---

## 八、AI 行为数据（aiData）

### 8.1 数据结构

`aiData` 与 `seidData` 结构相同：

```
aiData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
    tier => new Dictionary<object, Dictionary<string, object>>
    {
        { AI编号, new Dictionary<string, object> { { "参数名", 参数值 }, ... } },
        { AI编号, new Dictionary<string, object> { { "参数名", 参数值 }, ... } }
    },
    skillLv
)
```

### 8.2 注入原理

工厂处理 `aiData` 时：
1. 遍历每个 AI 编号，注入到 `jsonData.instance.AIJsonDate[AI编号]` 中，以当前神通实例的 `id` 作为键名
2. 内层 `Dictionary<string, object>` 转换为 JSONObject，作为该 AI 表内 `id` 对应的 value

### 8.3 清理机制

修改已有神通时，工厂会在注入前**全量遍历 `AIJsonDate`**，收集该神通实例 ID 曾挂载的所有 AI 编号，并从对应 AI 表中移除旧数据。这是必需的，因为 AI 数据不存储于技能本体 JSON 中，无法直接从技能 ID 反向查询。

---

## 九、编写步骤

### 9.1 新建神通

1. 复制 `SkillTemplate.cs` 到 `Data/Skill/` 目录下
2. 修改类名为合适的名称
3. 修改 `[DataBase]` 中的 `group` 和 `item` 参数
4. 在类顶部定义 `skillLv` 数组，列出该神通涉及的所有等级（通常为 `[1,2,3,4,5]`）
5. 在 `Data` 中填写定位字段（`key`、`localId`/`realId`）
6. 填写必填字段（`name`、`descr`、`attackType`、`cost`、`script`、`seidData`、`skillEffect`）
7. 按需填写选填字段
8. 编译运行，查看日志确认注册成功

### 9.2 修改已有神通

1. 创建 `SkillData`，填写 `realId`（或 `localId`）和 `skillLv` 定位目标
2. 仅填写需要修改的字段，其余字段不填
3. 工厂会自动匹配 `(Skill_ID, Skill_Lv)`，以更新模式覆盖对应字段，未填字段保持游戏原值

---

## 十、完整示例

### 示例：紫莲金影罩（修改已有神通）

```csharp
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.Skill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.Skill
{
    [DataBase(DataCategory.Skill, "金虹蓄势", "紫莲金影罩")]
    public static class 紫莲金影罩
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];
        private static readonly List<int> shieldVal = [14, 26, 28, 48, 94];
        private static readonly List<int> bonusShieldVal = [6, 9, 14, 24, 47];

        public static List<SkillData> Data =
        [
            new SkillData
            {
                key = "紫莲金影罩",
                realId = 16,                    // 使用 realId 定位已有神通
                skillLv = skillLv,

                // descr 和 tuJianDescr 按等级重新生成
                descr = TierValue<string>.Create(
                    tier => $"获得【护罩】*{shieldVal[tier-1]}，吸收一点金系灵气，若此时灵气数" +
                        $"大于灵气上限，则本回合结束时额外获得【护罩】*{bonusShieldVal[tier-1]}。",
                    skillLv
                ),
                tuJianDescr = TierValue<string>.Create(
                    tier => AffixProcessor.FormatTuJian(
                        $"获得【护罩】*{shieldVal[tier-1]}，吸收一点金系灵气，若此时灵气数" +
                        $"大于灵气上限，则本回合结束时额外获得【护罩】*{bonusShieldVal[tier-1]}。"
                    ),
                    skillLv
                ),

                // seidData 使用多个 seid 组合实现复杂效果
                seidData = TierValue<Dictionary<object, Dictionary<string, object>>>.Create(
                    tier => new()
                    {
                        // seid 4：获取护罩
                        [4] = new()
                        {
                            { "value1", new List<object> {5, "@buff:紫莲金影罩(灵气检测)"} },
                            { "value2", new List<int> { shieldVal[tier-1], bonusShieldVal[tier-1] } }
                        },
                        // seid 7：吸收金系灵气
                        [7] = new()
                        {
                            { "value1", new List<int> { 0 } },
                            { "value2", new List<int> { 1 } }
                        },
                        // seid 148：条件判断（灵气数 > 上限）
                        [148] = new()
                        {
                            { "panduan", ">"},
                            { "target", 1},
                            { "value1", "@buff:紫莲金影罩(灵气检测)" },
                            { "value2", 0 }
                        },
                        // seid 3：回合结束触发
                        [3] = new()
                        {
                            { "value1", new List<string> { "@buff:紫莲金影罩(灵气检测)" } },
                            { "value2", new List<int> { 1 } }
                        },
                    },
                    skillLv
                )
            }
        ];
    }
}
```
