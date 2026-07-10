# 《早咖晚酒》开发变更记录 · v0.02

> 本文档记录 v0.01 → v0.02 期间由 AI 辅助完成的代码改动，方便回顾"做了什么、核心代码在哪、进度到哪、下一步做什么"。
> 对应 Git 提交：`c23c667`（1.添加布局编辑器可编辑物品所占格子数 + 物品形状系统和可视化形状编辑器 2.添加 Obj 数据存储 3.修改吧台逻辑）

---

## 一、本次版本目标

v0.01 已有"吧台摆放 + 相邻 combo"的核心系统代码（网格、规则引擎、数据驱动物品库、纯 Debug 验证脚本）。v0.02 做了两件事：

1. **把核心系统变成能用鼠标玩的 Demo**（可视化 UI、点选摆放、combo 实时反馈、两套数值显示）。
2. **补上"物品形状系统 + 可视化形状编辑器"** —— 解决实测"不好玩"的根因：原来每个物品只占一格、摆哪都一样，缺少空间解谜。现在物品有大小/形状（1×3、2×2、L 形…），摆放变成"怎么拼进有限网格"的策略。

设计定位对齐：参考 **Backpack Hero**（单人构筑循环），不做 PvP。

---

## 二、改动清单

### 新增脚本

| 文件 | 作用 |
|---|---|
| `Assets/Scripts/Game/Bar/Config/ItemConfig.cs` | 物品配置 ScriptableObject。含 id/名称/类别/两套数值/**形状 shapeCells**。数据驱动，策划改数据不改代码。 |
| `Assets/Scripts/Game/Bar/Demo/BarDemoUI.cs` | 运行时交互核心。动态生成网格与物品栏，点选式摆放/移除，监听结算事件刷新数值、combo 列表、格子高亮。支持多格物品显示。 |
| `Assets/Scripts/Editor/BarDemoSceneBuilder.cs` | 编辑器菜单「生成吧台Demo场景」。自顶向下按正确层级/锚点/布局组件生成 UI，避免层级混乱。 |
| `Assets/Scripts/Editor/ItemShapeEditorWindow.cs` | **可视化形状编辑器**（EditorWindow）。左侧物品列表，右侧 5×5 点阵涂格定形状，保存写回 ItemConfig。 |
| `Assets/Scripts/Editor/ItemConfigBootstrap.cs` | 编辑器菜单「生成初始物品配置」。一键把内置数据生成成 11 个 ItemConfig 资源（迁移用，含示范形状）。 |
| `Assets/Scripts/Editor/ChineseFontSetup.cs` | 编辑器菜单「生成中文字体」。用 simhei.ttf 生成动态 TMP 字体并设为默认，解决中文显示为方块。 |

### 修改脚本

| 文件 | 改动 |
|---|---|
| `Assets/Scripts/Game/Bar/BarGrid.cs` | **多格摆放改造**（核心）：PlacedItem 记录所有占用格；Place/Remove/Move 按形状整体处理；新增 `GetCellsForShape`、`CanPlace`、`GetNeighbors(PlacedItem)`；AllItems/Count 按实例去重。 |
| `Assets/Scripts/Game/Bar/ItemDef.cs` | ItemDef 新增 `shapeCells` 字段；ItemDatabase 改为**优先从 Resources/ItemConfigs 加载 SO**，加载不到回退内置数据。对外 API（Get/All/Has）不变。 |
| `Assets/Scripts/Game/Bar/ComboResolver.cs` | 相邻判定改用 `GetNeighbors(source)`（多格物品的正确邻居）；新增"摇酒器+杯垫→稳固"示范规则。 |

### 新增资源

- `Assets/Resources/ItemConfigs/Item_*.asset` —— 11 个物品配置（含形状）。
- `Assets/Scenes/BarDemo.unity` —— Demo 场景（由脚本生成，已加入 Build Settings）。
- `Assets/Fonts/simhei.ttf` + `Assets/Fonts/SimHei SDF.asset` —— 中文字体。

---

## 三、核心代码架构

分层清晰，运行时逻辑与 Unity/编辑器解耦：

```
数据层（磁盘）        ItemConfig (.asset, ScriptableObject) —— 策划可视化编辑，含形状
      │  ItemDatabase 加载并转换
运行时模板           ItemDef —— 纯 C# 数据类，含 shapeCells
      │
核心数据结构         BarGrid —— 网格摆放，支持多格物品（一物占多格、相邻查询）
      │              PlacedItem —— 摆在格子上的实例，记录 occupiedCells
规则引擎             ComboResolver —— 纯计算：布局 → combo 结果（产出饮品/附加buff/两套数值）
      │
唯一入口             BarGridMgr —— 整合网格+结算+事件广播（E_Bar_ComboResolved）
      │  事件解耦
表现层               BarDemoUI —— 监听事件，可视化摆放与反馈
```

**关键设计**：
- **数据驱动**：物品与形状全在 ScriptableObject，加物品/改形状不动代码。
- **形状用相对偏移列表**表示（`List<Vector2Int>`），能表达任意形状，1×3/2×2 只是特例。
- **纯计算的结算器**：ComboResolver 不改布局、不发事件，可做地牢"假想布局试算"。
- **一套布局两套数值**：同一布局同时算出 brewValue（白天经营）和 attack/defense（下午战斗），是全游戏的灵魂。

---

## 四、编辑器工具（菜单「早咖晚酒」）

| 菜单项 | 用途 | 何时用 |
|---|---|---|
| 生成初始物品配置 | 生成 11 个 ItemConfig 资源 | 首次，已完成 |
| 物品形状编辑器 | 可视化编辑每个物品占用的格子形状 | 随时调整形状 |
| 生成吧台Demo场景 | 一键生成可玩场景 | 首次，已完成 |
| 生成中文字体 | 修复中文方块 | 首次，已完成 |

---

## 五、当前进度评估

以策划案第 9.3 节"垂直切片验收标准"为标尺：

| 验收项 | 状态 |
|---|---|
| 3×3 吧台、5-6 种物品、2-3 种 combo | ✅ 完成（11 物品、5 条 combo 规则）|
| 实时看到两套数值 | ✅ 完成 |
| **物品形状/空间解谜** | ✅ 完成（本版新增，超出原验收标准）|
| 白天经营（接订单出杯赚钱） | ❌ 未做 |
| 下午地牢自走棋（摆放→战斗验证） | ❌ 未做 |
| 结算 + 养成（解锁格子/物品） | ❌ 未做 |

**整体进度：约 25-30%（垂直切片阶段）**

- 核心系统（网格/combo/数据驱动/形状）：**~90% 完成**，地基扎实。
- 三大玩法（经营/地牢/叙事）：**0%**，尚未接入。
- 美术：占位阶段（中文字，无正式图标）。

**当前状态：可摆放、可看数值、有空间解谜，但"摆放之后没有下文"——缺少让摆放产生意义的验证环节。**

---

## 六、下一步（v0.03 建议）

**优先级 1：极简地牢战斗（闭合"摆放→验证→奖励"循环）**
这是让 Demo 从"能玩"变"好玩"的关键一块。
- 摆好布局 → 点"下地牢" → 用当前布局的 attack/defense 自动打一只"余烬残影" → 显示胜负。
- 复用已有的 ComboResolver 战斗数值，无需新数据结构。
- 让玩家第一次感受到"我这套摆法到底行不行"，验证核心乐趣。

**优先级 2（可选）：网格尺寸与形状平衡**
- 3×3 对 2×2、1×3 的大物品偏挤，考虑放宽到 4×4，或调整物品形状。

**之后**：白天经营循环 → 合成/升级 → 随机商店与多局重玩（roguelike）→ 夜晚叙事 → 正式美术接入（PixelLab）。

---

## 七、如何运行验证

1. Unity 打开项目，等待编译（无红错）。
2. 打开 `Assets/Scenes/BarDemo.unity`，点 Play。
3. 底部选物品 → 点网格摆放（咖啡机占 2×2、酒架占 1×3）→ 看 combo 与数值实时变化。
4. 菜单「早咖晚酒/物品形状编辑器」可随时改物品形状，保存后重新运行生效。
