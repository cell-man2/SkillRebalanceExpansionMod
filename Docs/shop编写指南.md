# Shop 编写指南

## 一、概述

Shop 工厂是**操作型工厂**，与其他所有工厂的设计模式完全不同。它不采用"扫描数据 → 展开 → 注入"的流程，而是采用**命令收集模式**：数据层通过 `Register()` 方法向指定商店发送增删命令，工厂在初始化阶段收集所有命令，在注入阶段统一执行。

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
    │   └── ShopFactory.Initialize()
    │       ├── 遍历所有 ShopDefinition，调用 Clear() 清空增删列表
    │       └── DataManager.Register(DataCategory.Shop)
    │           └── 扫描程序集中所有带 [DataBase(DataCategory.Shop)] 的类
    │               └── 调用每个类的静态 Register() 方法
    │                   └── 执行 ShopCollection.XXX.商店.Add() / Remove()
    │                       └── 命令存入对应 ShopDefinition 的 Adds / Removes 列表
    │
    ├── CheckRegistryDuplicates()
    │
    ├── InjectFactories()
    │   └── ShopFactory.Inject()
    │       ├── 遍历所有 ShopDefinition：
    │       │   ├── 收集 Remove 命令 → 解析 goodsId → 加入移除列表
    │       │   └── 收集 Add 命令 → 解析所有参数 → 加入添加列表
    │       ├── 遍历 jiaoHuanShopGoods，匹配移除列表删除条目
    │       └── 为每个添加请求分配新 id（baseId + 递增）并写入
    │
    ├── 打印所有注册表信息到日志
    │
    └── Registry.Clear()
```

### 2.2 关键机制说明

| 机制 | 说明 |
|------|------|
| 命令收集模式 | 数据类通过 `Register()` 方法发送增删命令，工厂统一收集后执行 |
| 商店定义 | `ShopCollection` 中预先定义了各商店及其默认配置（货币类型、价格规则） |
| 参数继承 | `Add()` 方法未传的参数继承商店定义的默认值 |
| 动态 ID 分配 | 新增商品条目每次启动从 `baseId = 1470` 开始递增分配 ID |
| 引用解析 | `goodsId`、`exGoodsId` 支持字符串引用（`@item:XXX`、`@skill:XXX` 等） |

---

## 三、核心概念

### 3.1 ShopDefinition

商店定义类，描述一个商店的配置：

| 属性 | 说明 |
|------|------|
| `ShopId` | 商店 ID，对应游戏内商店 |
| `ExGoodsId` | 默认兑换物 ID（即默认货币类型） |
| `Money` | 默认直接价格（兑换商店用，灵石兑换商店默认为 1） |
| `Percent` | 默认价格百分比（门派商店用，价格为原价 / percent，不可为 0） |
| `EventValue` | 默认事件值（剧情条件判定） |
| `Fuhao` | 默认符号（剧情条件判定） |

### 3.2 商店类型

通过两个工厂方法创建，区别在于价格计算方式：

| 方法 | 商店类型 | 价格计算 |
|------|---------|---------|
| `MoneyShop(id, exGoodsId)` | 兑换商店（灵石购买） | 价格固定为 `money` 值（默认 1），使用 `exGoodsId` 对应的货币 |
| `PercentShop(id, exGoodsId, percent)` | 门派商店（贡献兑换） | 价格 = 物品原价 / `percent`，使用 `exGoodsId` 对应的货币 |

### 3.3 ShopCollection 结构

```
ShopCollection.地区势力.具体商店
```

示例：
- `ShopCollection.金虹.藏经阁法术`
- `ShopCollection.竹山.灵核兑换`
- `ShopCollection.天机.秘籍`

---

## 四、操作方法

### 4.1 添加商品

```csharp
ShopCollection.XXX.商店.Add(goodsId, exGoodsId, money, percent, eventValue, fuhao)
```

| 参数 | 必填 | 说明 |
|------|------|------|
| `goodsId` | ✅ | 商品 ID，支持字符串引用 |
| `exGoodsId` | ❌ | 兑换物 ID（货币类型），继承商店默认值 |
| `money` | ❌ | 直接价格（仅兑换商店），继承商店默认值（灵石商店默认 1） |
| `percent` | ❌ | 价格百分比（仅门派商店），继承商店默认值，不可为 0 |
| `eventValue` | ❌ | 事件值，继承商店默认值 |
| `fuhao` | ❌ | 符号，继承商店默认值 |

### 4.2 移除商品

```csharp
ShopCollection.XXX.商店.Remove(goodsId)
```

| 参数 | 必填 | 说明 |
|------|------|------|
| `goodsId` | ✅ | 商品 ID，支持字符串引用 |

---

## 五、数据层编写

### 5.1 基本结构

```csharp
using SkillRebalanceExpansionMod.Models.Shop;

namespace SkillRebalanceExpansionMod.Data.Shop
{
    [DataBase(DataCategory.Shop, "流派", "描述")]
    public static class ShopTemplate
    {
        public static void Register()
        {
            // 添加商品
            // ShopCollection.天机.秘籍.Add("@item:XXX");

            // 移除商品
            // ShopCollection.天机.秘籍.Remove(114514);
        }
    }
}
```

### 5.2 完整示例

```csharp
using SkillRebalanceExpansionMod.Models.Shop;

namespace SkillRebalanceExpansionMod.Data.Shop
{
    [DataBase(DataCategory.Shop, "金虹蓄势", "金虹剑诀_天阶")]
    public static class 金虹剑诀_天阶
    {
        public static void Register()
        {
            // 向金虹秘阁功法商店添加商品
            ShopCollection.金虹.秘阁功法.Add("@item:金虹剑诀_天阶");

            // 如需移除商品：
            // ShopCollection.金虹.秘阁功法.Remove("@item:旧商品");

            // 如需覆盖货币类型：
            // ShopCollection.金虹.秘阁功法.Add("@item:金虹剑诀_天阶", exGoodsId: 10005);

            // 如需调整折扣比例（门派商店）：
            // ShopCollection.金虹.秘阁功法.Add("@item:金虹剑诀_天阶", percent: 80);

            // 如需指定固定价格（兑换商店）：
            // ShopCollection.金虹.灵核兑换.Add("@item:金虹剑诀_天阶", money: 100);
        }
    }
}
```

---

## 六、注意事项

1. **货币由每个商品条目独立决定**：`exGoodsId` 字段指定了该商品使用何种货币，`ShopDefinition` 中的 `ExGoodsId` 仅作为默认值

2. **Percent 不可为 0**：门派商店的 `percent` 值作为除数，不能为 0

3. **Money 商店记得写价格**：兑换商店若不传 `money` 参数，默认值为 1

4. **ID 每次重新分配**：新增商品条目的 ID 每次启动从 `baseId = 1470` 开始递增，不持久化

5. **引用解析**：`goodsId` 和 `exGoodsId` 均支持 `@item:XXX`、`@skill:XXX` 等字符串引用格式

6. **命令在 Initialize 阶段收集**：`Register()` 方法会在 `ShopFactory.Initialize()` 中被调用，此时 `Registry` 已填充完毕，引用解析可用