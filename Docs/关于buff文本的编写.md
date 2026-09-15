# 关于Buff描述的编写

## 1.Buff描述文本的生成机制

### 生成位置与调用链

Buff描述文本的生成由游戏代码中的以下调用链完成：

```
鼠标悬停Buff图标 (UTooltipBuffTrigger.OnPointerEnter)
    ↓
获取描述文本 (UTooltipBuffTrigger.GetDesc)
    ↓
调用Tools.getDesc(desstr, __attack)
    ↓
核心处理 (Tools.setAttackTxt)
```

### 核心处理逻辑

`Tools.setAttackTxt` 方法的工作原理如下：

1. 提取表达式：从Buff描述模板中提取括号（和）之间的内容
2. 替换占位符：将提取的表达式中的所有 `attack` 替换为传入的数值 `__attack`
3. 计算表达式：使用 .NET 的 `DataTable.Compute` 方法计算替换后的表达式
4. 重新拼装：将计算结果用颜色标记 `[FF00FF]` 和 `[-]` 包裹，放回原描述中

### 模板格式要求

Buff描述文本必须遵循以下格式：
```
前缀文本（表达式）后缀文本
```

示例：
- 模板：`"攻击力（attack * 2 + 10）额外效果"`
- 当 `attack = 50` 时，显示为：`"攻击力[FF00FF]110[-]额外效果"`

### 重要说明

- `attack` 的含义：在Buff描述上下文中，`attack` 代表 Buff自身的当前层数
- 递归处理：如果计算后的文本中仍包含 `attack` 字样，会递归调用直到全部替换
- 表达式的范围：只能是（和）之间的内容，超出此范围不会被计算

---

## 2.表达式语法参考

表达式由 .NET 的 `DataTable.Compute` 方法执行，支持以下运算符和函数。完整的官方语法说明请参考微软文档：
[DataColumn.Expression 属性](https://learn.microsoft.com/zh-tw/dotnet/api/system.data.datacolumn.expression?view=net-10.0#system-data-datacolumn-expression)

### 运算符

| 类别 | 运算符 | 说明 |
| :--- | :--- | :--- |
| 算术 | `+`, `-`, `*`, `/`, `%` | 加、减、乘、除、取模（求余数） |
| 比较 | `=`, `>`, `<`, `>=`, `<=`, `<>` | 等于、大于、小于、大于等于、小于等于、不等于 |
| 逻辑 | `AND`, `OR`, `NOT` | 与、或、非，用于组合多个条件 |
| 字符串 | `+` | 连接字符串 |
| 其他 | `IN`, `LIKE` | 检查值是否在列表中；模糊匹配字符串（`*` 或 `%` 作通配符） |

### 函数

| 函数 | 说明 | 语法 |
| :--- | :--- | :--- |
| `IIF` | 条件判断，根据逻辑表达式的真假返回两个值之一 | `IIF(expr, truepart, falsepart)` |
| `ISNULL` | 检查表达式是否为 `null`，若是则返回替换值 | `ISNULL(expression, replacementvalue)` |
| `LEN` | 返回字符串的长度 | `LEN(expression)` |
| `TRIM` | 去除字符串首尾的空白字符 | `TRIM(expression)` |
| `SUBSTRING` | 从字符串的指定位置开始截取指定长度的子串 | `SUBSTRING(expression, start, length)` |
| `CONVERT` | 将表达式转换为指定的 .NET 类型 | `CONVERT(expression, type)` |

### 聚合函数（用于跨多行计算）

| 函数 | 说明 |
| :--- | :--- |
| `Sum` | 计算数值列的总和 |
| `Avg` | 计算数值列的平均值 |
| `Min` | 计算数值列的最小值 |
| `Max` | 计算数值列的最大值 |
| `Count` | 计算行数 |
| `StDev` | 计算统计标准差 |
| `Var` | 计算统计方差 |

### 语法规则

| 规则 | 说明 |
| :--- | :--- |
| 列名引用 | 直接使用 `ColumnName`。若列名含空格或特殊字符，需用方括号 `[]` 括起来 |
| 字符串字面值 | 必须用单引号 `'` 括起来（如 `'文本'`） |
| 日期字面值 | 用井号 `#` 括起来（如 `#1/31/2006#`） |
| 空值（null） | 直接使用关键字 `null` |
| 关联表引用 | 通过 `Parent` 或 `Child` 关键字引用关联表 |

### 常见注意事项

1. 类型匹配：表达式的返回值类型必须与使用场景匹配（如在数值计算中返回布尔值会导致错误）
2. 字符串拼接数字：如 `'攻击力: ' + attack` 会导致类型错误，需使用 `CONVERT(attack, 'System.String')` 转换
3. `IIF` 返回类型一致：`truepart` 和 `falsepart` 应返回相同类型的值