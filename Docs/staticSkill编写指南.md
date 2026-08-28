# StaticSkill 编写指南

## 一、概述

本 Mod 采用数据驱动 + 工厂自动注入的方式添加功法。与 NPCLeiXing 工厂类似，StaticSkill 工厂采用**包（Package）展开模式**：一个 `StaticSkillData` 数据包包含多个等级（skillLv）的配置，工厂将其展开为每个等级一个独立的实例，注入到游戏的 `StaticSkillJsonData` 中。

功法工厂在包展开的基础上，增加了**自动数值计算**和**词缀/图鉴自动生成**机制，大幅减少了重复性配置工作。

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
    │   └── StaticSkillFactory.Initialize()
    │       ├── BuildStaticSkillIndex()
    │       │   └── 从游戏现有 StaticSkillJsonData 构建 (Skill_ID, Skill_Lv) → id 索引
    │       │
    │       ├── DataManager.Scan<StaticSkillData>(DataCategory.StaticSkill)
    │       │   └── 扫描程序集中所有带 [DataBase(DataCategory.StaticSkill)] 的类
    │       │       └── 读取每个类的静态字段 Data（List<StaticSkillData>）
    │       │
    │       ├── 对每个 StaticSkillData：
    │       │   ├── 生成 Skill_ID（realId 或 baseSkillId + localId）
    │       │   ├── 展开：遍历 skillLv 列表，每个等级生成一个实例
    │       │   │   ├── 若 (Skill_ID, Skill_Lv) 存在于索引中 → isNew = false，复用旧 id
    │       │   │   ├── 若 (Skill_ID, Skill_Lv) 不存在于索引中 → isNew = true，分配新 id
    │       │   │   └── 实例 key = 包 key + 等级数字（如 "金虹剑诀_天阶1"）
    │       │   ├── 若实例非新建且有 seidData → 收集旧 seid 供清理
    │       │   ├── 注册实例 key → id 到 Registry.staticSkill
    │       │   ├── 注册包 key → Skill_ID 到 Registry.ssId
    │       │   └── 注册包 key → BookInfo 到 Registry.bookInfo
    │       │
    │       └── staticSkillInstanceDatas 缓存所有展开后的实例供 Inject 使用
    │
    ├── CheckRegistryDuplicates()
    │
    ├── InjectFactories()
    │   └── StaticSkillFactory.Inject()
    │       ├── RemoveOldStaticSkillSeid()
    │       │   └── 遍历 removeStaticSkillSeid，从各 seid 表中移除旧数据
    │       │
    │       └── 对每个 StaticSkillInstanceData：
    │           ├── 若 isNew → 新建 JSON 对象（id + Skill_ID + Skill_Lv + defaultData）
    │           │   └── 调用 InitNewStaticSkill()：
    │           │       ├── 若有 skillPin/skillJie/skillStyle → 自动计算 Skill_Speed 和 Skill_castTime
    │           │       └── 若有 descr → 自动提取 Affix 并生成 TuJiandescr
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
| 包展开模式 | 一个 `StaticSkillData` 包含多个等级，工厂按 `skillLv` 列表展开为多个独立实例 |
| 新建/修改判定 | 通过 `(Skill_ID, Skill_Lv)` 复合键在游戏现有数据中查找：存在则复用 id（修改），不存在则分配新 id（新建） |
| 自动数值计算 | 新建实例时，若提供了 `skillPin`、`skillJie`、`skillStyle`，工厂自动计算 `Skill_Speed` 和 `Skill_castTime` |
| 词缀/图鉴自动生成 | 新建实例时，若提供了 `descr` 且未单独指定 `affix`/`tuJianDescr`，工厂自动从描述中提取词缀并生成图鉴描述 |
| 多重注册 | 包 key 同时注册到 `Registry.ssId`（功法编号）和 `Registry.bookInfo`（功法书信息），实例 key 注册到 `Registry.staticSkill` |
| seid 清理 | 修改已有实例时，注入前自动从旧 seid 表中移除该实例的数据，防止残留和冲突 |
| 查重保护 | 注册前检查所有 ID 是否重复，发现重复则抛出异常防止数据错乱 |

---

## 三、数据模型

详细定义见 `Definition.cs`，以下为速查表。枚举值定义见 `Models/StaticSkill/Enums.cs`。

### 3.1 定位字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `key` | `string` | 注册表键名前缀，展开后实例 key = 此 key + 等级数字 |
| `localId` | `int?` | 本地偏移 ID，最终 Skill_ID = baseSkillId(24530) + localId（与 `realId` 二选一） |
| `realId` | `int?` | 绝对 ID，直接作为 Skill_ID 写入 JSON（与 `localId` 二选一） |
| `skillLv` | `List<int>` | 涵盖的功法等级列表，如 `[1,2,3,4,5]`，每个等级展开为一个实例 |

### 3.2 必填字段

| 字段 | 类型 | JSON 字段 | 说明 |
|------|------|-----------|------|
| `attackType` | `TierValue<AttackType>` | `AttackType` | 功法属性（金/木/水/火/土/气/遁术/神/剑/体） |
| `descr` | `TierValue<string>` | `descr` | 功法描述。若未填写 `tuJianDescr`，工厂自动从此字段生成图鉴描述 |
| `name` | `TierValue<string>` | `name` | 功法名称 |
| `seidData` | `TierValue<Dictionary<object, Dictionary<string, object>>>` | `seid` | 特性列表，功法实际效果的核心，必须按等级配置 |

### 3.3 选填字段（使用 TierValue<T> 按等级配置）

| 字段 | 类型 | JSON 字段 | 默认值 |
|------|------|-----------|--------|
| `affix` | `TierValue<List<int>>` | `Affix` | `[]`（若未填写且提供了 `descr`，工厂自动从 `descr` 提取） |
| `df` | `TierValue<bool>` | `DF` | `0`（false） |
| `icon` | `TierValue<int>` | `icon` | `0` |
| `qingJiaoType` | `TierValue<QingJiaoType>` | `qingjiaotype` | `1`（QingJiaoType.普通） |
| `skillCastTime` | `TierValue<int>` | `Skill_castTime` | 由工厂根据 `skillLv`、`skillPin`、`skillJie` 自动计算 |
| `skillJie` | `TierValue<SkillJie>` | `Skill_LV` | `1`（SkillJie.人，若不填写则自动计算需自行指定） |
| `skillPin` | `TierValue<SkillPin>` | `typePinJie` | `1`（SkillPin.下，若不填写则自动计算需自行指定） |
| `skillSpeed` | `TierValue<int>` | `Skill_Speed` | 由工厂根据 `skillLv`、`skillPin`、`skillJie`、`skillStyle` 自动计算 |
| `skillStyle` | `TierValue<SkillStyle>` | （不写入 JSON） | 用于自动计算修炼速度，若不填写则 `skillSpeed` 需自行指定 |
| `tuJianDescr` | `TierValue<string>` | `TuJiandescr` | 若未填写且提供了 `descr`，工厂自动从 `descr` 生成 |
| `tuJianType` | `TierValue<TuJianType>` | `TuJianType` | `0`（TuJianType.无） |

---

## 四、新建与修改的判定

### 4.1 核心规则

工厂通过 `(Skill_ID, Skill_Lv)` 复合键自动判定新建/修改：

| 场景 | 判定条件 | 行为 |
|------|---------|------|
| 新建实例 | `(Skill_ID, Skill_Lv)` 不在游戏现有数据中 | 分配新 id（baseId + 递增），`isNew = true`，执行自动计算 |
| 修改已有实例 | `(Skill_ID, Skill_Lv)` 在游戏现有数据中 | 复用旧 id，`isNew = false`，不执行自动计算，以更新模式覆盖字段 |

### 4.2 设计意图

- 新建：希望添加游戏中原本不存在的功法。
- 修改：希望修改游戏中已有功法的某些字段（如描述、seid 效果等）。只需填写要覆盖的字段即可，未填字段保持游戏原值。

---

## 五、注册表引用系统

### 5.1 三层注册

| 注册表 | Key | Value | 引用语法 | 用途 |
|--------|-----|-------|---------|------|
| `Registry.staticSkill` | `"金虹剑诀_天阶1"` | 实例 id | `@staticSkill:金虹剑诀_天阶1` | 引用功法实例（某个具体等级） |
| `Registry.ssId` | `"金虹剑诀_天阶"` | Skill_ID | `@ssId:金虹剑诀_天阶` | 引用功法编号（如 NPC 配置） |
| `Registry.bookInfo` | `"金虹剑诀_天阶"` | `BookInfo` 对象 | `"@bookInfo:金虹剑诀_天阶"` | 功法书自动填充（ItemFactory 的 `skillKey`） |

### 5.2 支持引用的字段

| 字段 | 支持引用类型 | 说明 |
|------|-------------|------|
| `seidData` 的 Key | 直接 int | seid 编号 |
| `seidData` 的 Value | `@buff:xxx`、`@sId:xxx`、`@staticSkill:xxx`、直接值 | 所有参数值均可使用注册表引用 |

工厂在处理 `seidData` 时会递归解析所有 `object` 值，遇到字符串 `@类型:key` 则查表替换为对应 ID，遇到数组/字典则递归处理内部元素。

---

## 六、TierValue 使用指南

### 6.1 数据结构

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

### 6.2 使用示例

```csharp
// 方式一：索引器手动配置
name = new TierValue<string>
{
    [1] = "金虹剑诀1",
    [2] = "金虹剑诀2",
    [3] = "金虹剑诀3",
    [4] = "金虹剑诀4",
    [5] = "金虹剑诀5",
};

// 方式二：Create 工厂方法（推荐）
name = TierValue<string>.Create(tier => $"金虹剑诀{tier}", skillLv);
descr = TierValue<string>.Create(
    tier => $"每使用一点金系灵气，获得【蓄势】*{tier * 2}", 
    skillLv
);

// 单值（所有等级共用）——隐式转换
attackType = AttackType.剑;
skillJie = SkillJie.天;
skillPin = SkillPin.上;
```

### 6.3 注意事项

- 若某等级未配置，该字段在该等级实例中保持默认值或由工厂计算
- 配置的等级必须在数据包的 `skillLv` 列表中，否则该配置不会被使用
- `TierValue<T>` 支持隐式单值转换，适用于所有等级共用同一值的场景
- 使用 `TierValue<T>.Create()` 时，`skillLv` 列表作为第二个参数传入

---

## 七、自动计算机制

### 7.1 修炼速度（Skill_Speed）

计算公式基于 `skillLv`、`skillPin`、`skillJie`、`skillStyle`：

```
Skill_Speed = 100 * skillLv * 3^(skillJie-1) * (1 + 0.2*(skillPin-2)) * (1 + 0.2*(skillStyle-2))
```

以等级 1 的中庸类型功法为基准的修炼速度：

| 阶级 \ 品级 | 下品 | 中品 | 上品 |
|-------------|------|------|------|
| 人阶 | 80 | 100 | 120 |
| 地阶 | 240 | 300 | 360 |
| 天阶 | 720 | 900 | 1080 |

> **注意**：
> - 修炼类型（skillStyle = 3）的功法在上述基础上额外 +20% 速度
> - 战斗类型（skillStyle = 1）的功法在上述基础上额外 -20% 速度
> - 等级每提升 1 级，速度线性增加 `100 * 技能等级` 的基准值

### 7.2 参悟时间（Skill_castTime）

计算公式基于 `skillLv`、`skillPin`、`skillJie`：

- **等级 1 固定为 1**（第一层通过读书学习，而非闭关突破，因此参悟时间填写任意值均不影响实际游戏表现，工厂参考游戏本体统一写为 1）
- 等级 ≥ 2 时，基于品级和阶级的复杂级数乘积计算

### 7.3 词缀提取（Affix）

从 `descr` 中提取所有 `【词缀名】` 格式的文本，转换为对应的词缀 ID 列表。词缀映射表见 `AffixProcessor.AffixMap`。

### 7.4 图鉴描述（TuJiandescr）

将 `descr` 中的 `【词缀名】` 替换为游戏内图鉴超链接格式：

```
【蓄势】 → #c449491【<hy t=蓄势 l=2_101_1_27 fhc=#e86524 ul=1>】#n
```

### 7.5 自动计算触发条件

上述自动计算**仅在 `isNew = true`（新建实例）时执行**。若用户显式填写了对应字段（如 `skillSpeed`、`tuJianDescr`、`affix`），则以用户填写的值为准，跳过自动计算。

---

## 八、编写步骤

### 8.1 新建功法

1. 复制 `StaticSkillTemplate.cs` 到 `Data/StaticSkill/` 目录下
2. 修改类名为合适的名称
3. 修改 `[DataBase]` 中的 `group` 和 `item` 参数
4. 在类顶部定义 `skillLv` 数组，列出该功法涉及的所有等级（通常为 `[1,2,3,4,5]`）
5. 在 `Data` 中填写定位字段（`key`、`localId`/`realId`）
6. 填写必填字段（`name`、`descr`、`attackType`、`seidData`），使用 `TierValue<T>.Create()` 按等级生成
7. 按需填写选填字段
8. 编译运行，查看日志确认注册成功

### 8.2 修改已有功法

1. 创建 `StaticSkillData`，填写 `realId`（或 `localId`）和 `skillLv` 定位目标
2. 仅填写需要修改的字段（使用 `TierValue<T>` 按等级配置要覆盖的等级和值）
3. 工厂会自动匹配 `(Skill_ID, Skill_Lv)`，以更新模式覆盖对应字段，未填字段保持游戏原值
4. **注意**：修改已有功法时不会触发自动计算，如果需要则需显式填写 `skillSpeed`、`skillCastTime` 等字段以改动对应内容

---

## 九、完整示例

### 示例：金虹剑诀·天阶

```csharp
using System.Collections.Generic;
using SkillRebalanceExpansionMod.Models.StaticSkill;
using SkillRebalanceExpansionMod.Utils;

namespace SkillRebalanceExpansionMod.Data.StaticSkill
{
    [DataBase(DataCategory.StaticSkill, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        private static readonly List<int> skillLv = [1, 2, 3, 4, 5];

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
                        [1] = new()
                        {
                            { "target", 1 },
                            { "value1", new List<string> {
                                "@buff:金虹剑诀_天阶(伤害提升&展示)",
                                "@buff:金虹剑诀_天阶(消散灵气)",
                                "@buff:金虹剑诀_天阶(使用灵气)"
                            } },
                            { "value2", new List<int> { 5 * tier + 5, 2 * tier, 2 * tier } }
                        }
                    },
                    skillLv
                )
            }
        ];
    }
}
```