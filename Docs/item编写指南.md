# Item 编写指南

## 一、概述

本 Mod 采用数据驱动 + 工厂自动注入的方式添加物品。你只需要编写一个包含 `ItemData` 数据定义的类，工厂会在游戏启动时自动扫描、注册并注入到游戏的 JSON 数据中，无需手动修改任何游戏原始文件。

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
    │   └── ItemFactory.Initialize()
    │       ├── DataManager.Scan<ItemData>(DataCategory.Item)
    │       │   └── 扫描程序集中所有带 [DataBase(DataCategory.Item)] 的类
    │       │       └── 读取每个类的静态字段 Data（List<ItemData>）
    │       │
    │       ├── 对每个 ItemData：
    │       │   ├── 调用 GenId()：
    │       │   │   ├── 若 localId 有值：生成 realId = baseId(28500) + localId
    │       │   │   ├── 若 realId 有值：直接使用（修改已有，或特殊新建）
    │       │   │   └── 若 qingJiao 为 true：realId += 1000000000
    │       │   ├── 若有 key：注册到 Registry.item
    │       │   └── 若有 seidData：从游戏 JSON 读取该物品已挂载的 seid
    │       │       └── 记录到 removeItemSeid（用于后续清理）
    │       │
    │       └── itemDatas 缓存所有数据供 Inject 使用
    │
    ├── CheckRegistryDuplicates()
    │
    ├── InjectFactories()
    │   └── ItemFactory.Inject()
    │       ├── RemoveOldItemSeid()
    │       │   └── 遍历 removeItemSeid，从各 seid 表中移除旧数据
    │       │
    │       └── 对每个 ItemData：
    │           ├── InjectItemData()
    │           │   ├── 若 ID 已存在 → 获取现有 JSON 对象（更新模式）
    │           │   ├── 若 ID 不存在：
    │           │   │   ├── 用 defaultData 新建 JSON 对象
    │           │   │   └── 若有 skillKey：调用 InitNewItem() 自动计算并填充字段
    │           │   └── 将 ItemData 的非空字段覆盖到 JSON 上
    │           │
    │           └── InjectItemSeid()
    │               └── 将 seidData 写入 ItemsSeidJsonData[seidId][id]
    │
    ├── 打印所有注册表信息到日志
    │
    └── Registry.Clear()
```

### 2.2 关键机制说明

| 机制 | 说明 |
|------|------|
| 自动扫描 | 通过 `[DataBase(DataCategory.Item)]` 标记类，工厂自动发现并加载其中的 `Data` 字段 |
| Key 注册 | 所有 `key` 被注册到 `Registry.item`，其他模块可通过 `@item:key` 引用该物品 |
| 新建/修改区分 | 通过 `localId`/`realId` 的使用来区分新建还是修改已有物品（详见第四节） |
| skillKey 自动填充 | 对于功法书/神通书，通过 `skillKey` 引用 `BookInfo` 自动计算默认字段值 |
| qingJiao 偏移 | 请教版本物品 ID 额外加上 `1000000000` 偏移，与原生请教物品保持一致 |
| seid 清理 | 注入前自动从旧 seid 表中移除该物品的数据，防止残留和冲突 |
| 查重保护 | 注册前检查所有 ID 是否重复，发现重复则抛出异常防止数据错乱 |

---

## 三、数据模型

详细定义见 `Definition.cs`，以下为速查表。枚举值定义见 `Models/Item/Enums.cs`。

### 3.1 定位字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `key` | `string` | 注册表键名，用于跨模块引用 |
| `localId` | `int?` | 本地偏移 ID，最终 ID = baseId(28500) + localId（若 qingJiao 为 true 则再加 1000000000）。新建物品使用此字段 |
| `realId` | `int?` | 绝对 ID，直接作为 id 写入 JSON。修改已有物品使用此字段定位目标 |

### 3.2 必填字段

以下字段仅在新建物品时必须填写。修改已有物品时只需填写需要覆盖的字段即可。

| 字段 | 类型 | JSON 字段 | 说明 |
|------|------|-----------|------|
| `name` | `string` | `name` | 物品名称 |
| `seidData` | `Dictionary<object, Dictionary<string, object>>` | `seid` | 特性列表，物品实际效果的核心 |

### 3.3 技能书/功法书特化字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `skillKey` | `string` | 关联的 BookInfo 注册键名，填写后工厂在新建时自动计算 ItemIcon、quality、type、desc、StuTime、price、maxNum、typePinJie 等字段 |
| `qingJiao` | `bool` | 是否为请教版本，默认 false。true 时 ID 额外加 1000000000 偏移，价格固定为 1，ShopType 固定为 99，且写入 NpcQingJiaoItemData |

依赖 `skillKey` 自动计算的字段：若不填写则由工厂计算；若填写则以填写的值为准。

### 3.4 选填字段（含默认值）

以下字段仅在新建物品时，若不填写则使用默认值。

| 字段 | 类型 | JSON 字段 | 默认值 |
|------|------|-----------|--------|
| `affix` | `List<int>` | `Affix` | `[]` |
| `canSale` | `bool?` | `CanSale` | `0`（可出售） |
| `canUse` | `int?` | `CanUse` | `0` |
| `danDu` | `int?` | `DanDu` | `0` |
| `desc` | `string` | `desc` | `null`（若有 skillKey 则由工厂计算） |
| `desc2` | `string` | `desc2` | `null` |
| `faBaoType` | `string` | `FaBaoType` | `""` |
| `itemFlag` | `List<ItemFlag>` | `ItemFlag` | `[]` |
| `itemIcon` | `int?` | `ItemIcon` | `0`（若有 skillKey 则由工厂计算） |
| `maxNum` | `int?` | `maxNum` | `9999999`（若有 skillKey 则由工厂计算为 `1`） |
| `npcCanUse` | `bool?` | `NPCCanUse` | `0`（false） |
| `price` | `int?` | `price` | `0`（若有 skillKey 则由工厂计算；若 qingJiao 则固定为 1） |
| `quality` | `int?` | `quality` | `null`（若有 skillKey 则由工厂计算） |
| `shopType` | `ShopType?` | `ShopType` | `99`（不投放；若 qingJiao 则固定为 99） |
| `shuaXin` | `int?` | `ShuaXin` | `0` |
| `shuXingType` | `int?` | `ShuXingType` | `0` |
| `stuTime` | `int?` | `StuTime` | `0`（若有 skillKey 则由工厂计算） |
| `tuJianType` | `TuJianType?` | `TuJianType` | `0`（其它） |
| `type` | `Type?` | `type` | `null`（若有 skillKey 则由工厂计算） |
| `typePinJie` | `int?` | `typePinJie` | `1`（若有 skillKey 则由工厂计算） |
| `vagueType` | `int?` | `vagueType` | `1` |
| `wuDao` | `List<(DaoType, DaoLevel)>` | `wuDao` | `[]` |
| `wuWeiType` | `int?` | `WuWeiType` | `0` |
| `yaoZhi1` | `int?` | `yaoZhi1` | `0` |
| `yaoZhi2` | `int?` | `yaoZhi2` | `0` |
| `yaoZhi3` | `int?` | `yaoZhi3` | `0` |

---

## 四、新建与修改的区分

### 4.1 核心规则

| 场景 | key | localId | realId | 行为 |
|------|-----|---------|--------|------|
| 新建物品 | 填写 | 填写 | 不填 | 生成 ID = baseId(28500) + localId（若 qingJiao 则再加偏移量），注册到 Registry.item |
| 修改已有物品 | 填写（便于阅读） | 不填 | 填写目标 ID | 匹配已有 ID，以更新模式覆盖字段 |

（`realId` 也可以用于直接指定 ID 生成新物品，但原则上不推荐，约定上统一使用 `localId` 进行新建。）

### 4.2 设计意图

- 新建：希望添加游戏中原本不存在的物品，需要分配新的唯一 ID。
- 修改：希望修改游戏中已有物品的某些字段，保留原 ID。填写 `key` 便于阅读和识别该 Data 的作用对象，但定位仍以 `realId` 为准。

---

## 五、注册表引用系统

### 5.1 物品注册

工厂初始化时，会将所有 `ItemData` 的 `key` 和生成的物品实际 ID 注册到 `Registry.item` 字典中。

### 5.2 引用物品 ID

在任意支持引用的字段中，使用 `"@item:XXX"` 格式来引用 `key="XXX"` 的 ItemData 的 ID：

```
"@item:金虹剑诀_天阶"    → 解析为 Registry.item["金虹剑诀_天阶"] 的值（int ID）
```

### 5.3 BookInfo 注册

`skillKey` 引用的是 `Registry.bookInfo`，存储的是 `BookInfo` 对象而非 int ID。`BookInfo` 包含 `type`（功法/神通）、`skillId`、`jie`（阶数）、`pin`（品阶）、`qingJiaoType` 等字段，用于工厂自动计算功法书/神通书的默认值。

### 5.4 支持引用的字段

在 `ItemData` 中，以下字段的内容可以使用注册表引用：

| 字段 | 可引用位置 | 说明 |
|------|-----------|------|
| `seidData` 的 Key | `Dictionary<object, ...>` 的 Key | seid 编号本身支持引用解析，通常直接填写 int |
| `seidData` 的 Value | 内层 `Dictionary<string, object>` 的参数值 | 所有参数值均可使用 `@类型:key` 引用已注册的数据 |

工厂在处理时会对 `seidData` 中的所有 `object` 值进行递归解析：遇到字符串 `@类型:key` 则查表替换为对应 ID，遇到数组/字典则递归处理内部元素，最终转换为 JSONObject 对象写入游戏 JSON。

---

## 六、seidData 编写指南

### 6.1 数据结构

```
seidData = new Dictionary<object, Dictionary<string, object>>
{
    { seid编号, new Dictionary<string, object> { { "参数名", 参数值 }, ... } },
    { seid编号, new Dictionary<string, object> { { "参数名", 参数值 }, ... } }
}
```

### 6.2 注入原理

工厂处理 `seidData` 时分为两步：

1. 将 `seidData` 的所有 Key（即 seid 编号）收集为数组，写入该物品的 JSONObject 的 `seid` 字段。
2. 遍历每个 Key，将对应的内层 `Dictionary<string, object>` 转换为 JSONObject，注入到 `jsonData.instance.ItemsSeidJsonData[seid编号]` 中，以当前物品的 `realId` 作为键名。

`id` 字段由工厂自动填写（值为当前物品的 `realId`），无需在 `seidData` 中手动添加，只需填写其它字段的信息。

### 6.3 seid 参数查询

每个 seid 有自己独立的参数类型和结构，详细信息请查阅游戏官方 Excel 表格中各 seid 的字段定义与参数说明。

---

## 七、编写步骤

### 7.1 新建物品

1. 复制 `ItemTemplate.cs` 到想要的位置（例如 `Data/Item/` 目录下）
2. 修改为自己认为合适的类名和文件名
3. 修改 `[DataBase]` 中的 `group` 和 `item` 参数
4. 填写 `key`（全局唯一）
5. 填写 `localId`
6. 填写必填字段（`name`、`seidData`）
7. 若为功法书/神通书，填写 `skillKey`（以及可选的 `qingJiao`）
8. 按需填写选填字段

### 7.2 修改已有物品

1. 创建 `ItemData`
2. 填写 `realId` 为目标物品的 ID
3. 填写 `key`（便于阅读，可选）
4. 仅填写需要修改的字段，其余字段不填
5. 工厂会以更新模式覆盖对应字段，未填字段保持游戏原值