using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ============================================================================
/// 吧台核心玩法 · 可玩化 Demo 交互脚本（BarDemoUI）
/// ----------------------------------------------------------------------------
/// 对应策划案第 9.3 节垂直切片验收：3×3 吧台、5-6 种物品、能触发 2-3 种 combo、
/// 实时看到"白天出杯效率"与"下午攻防"两套数值。
///
/// 它把已完成的核心系统（BarGridMgr / ComboResolver / ItemDatabase）包一层
/// 可点击、可视化、有即时反馈的 UI，让人能真正用鼠标摆放、感受 combo。
///
/// 交互（点选式，比拖拽简单且不易出 bug）：
///   1. 点 palette 里的物品 → 记为"当前选中物品"（高亮）
///   2. 点网格空格 → BarGridMgr.Place(选中id, pos)
///   3. 点已占用格 → BarGridMgr.Remove(pos)（方便反复试摆）
/// 每次布局变化，BarGridMgr 会广播 E_Bar_ComboResolved，本脚本刷新整个界面。
///
/// 设计要点：所有格子按钮、物品按钮都在运行时按数据动态生成——
///   网格从 3×3 扩到 5×5、物品从 6 种加到 30 种，都不用改 UI 结构。
/// UI 容器（父节点、锚点、布局组件）由 BarDemoSceneBuilder 编辑器脚本预先建好，
///   本脚本只往容器里填内容，从而层级/位置绝不会乱。
/// ============================================================================
/// </summary>
public class BarDemoUI : MonoBehaviour
{
    [Header("由 BarDemoSceneBuilder 连好的容器引用")]
    [Tooltip("网格区容器，需挂 GridLayoutGroup")]
    public RectTransform gridContainer;
    [Tooltip("物品选择栏容器，需挂 Horizontal/GridLayoutGroup")]
    public RectTransform paletteContainer;
    [Tooltip("顶部数值栏文本（出杯效率/攻击/防御）")]
    public TMP_Text statsText;
    [Tooltip("combo 结算列表文本")]
    public TMP_Text comboListText;
    [Tooltip("当前选中物品提示文本")]
    public TMP_Text selectionText;

    [Header("网格规格（与 BarGridMgr 初始一致）")]
    public int rows = 3;
    public int cols = 3;

    /// <summary>
    /// id → 占位图标。用中文单字而非 emoji：demo 用的黑体(simhei)不含 emoji 字形，
    /// emoji 会显示成方块；中文单字黑体一定能显示，格子里一眼能认出是什么物品。
    /// 以后接入 PixelLab 正式图标时，把这里换成 Image + iconPath 即可。
    /// </summary>
    private static readonly Dictionary<string, string> iconMap = new Dictionary<string, string>
    {
        { "coffee_machine", "咖啡机" },
        { "liquor_rack",    "酒架" },
        { "milk",           "奶" },
        { "coffee_bean",    "豆" },
        { "ice",            "冰" },
        { "shaker",         "摇酒器" },
        { "coaster",        "杯垫" },
        { "latte",          "拿铁" },
        { "americano",      "美式" },
        { "iced_liquor",    "烈酒" },
        { "cocktail",       "鸡尾酒" },
    };

    // 运行时生成的格子按钮，按坐标索引，用于刷新显示
    private readonly Dictionary<GridPos, GridCellView> cellViews = new Dictionary<GridPos, GridCellView>();

    // 当前选中的物品 id（palette 里点的那个），为空表示未选
    private string selectedItemId;

    /// <summary>单个格子的视图缓存（按钮 + 图标文本 + 背景，用于高亮）</summary>
    private class GridCellView
    {
        public GridPos pos;
        public Button button;
        public TMP_Text icon;
        public Image background;
    }

    private static readonly Color colorEmpty = new Color(0.20f, 0.20f, 0.24f, 1f);
    private static readonly Color colorFilled = new Color(0.30f, 0.32f, 0.40f, 1f);
    private static readonly Color colorComboHi = new Color(0.85f, 0.62f, 0.25f, 1f); // 命中 combo 的发起格高亮

    private void Start()
    {
        // 确保网格规格与本脚本一致（BarGridMgr 默认 3×3，这里显式对齐，扩展也走这里）
        BarGridMgr.Instance.InitGrid(rows, cols, useDiagonal: false);

        BuildGrid();
        BuildPalette();

        EventCenter.Instance.AddEventListener<ComboResult>(E_EventType.E_Bar_ComboResolved, OnComboResolved);

        UpdateSelectionText();
        RefreshAll(BarGridMgr.Instance.LastResult);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<ComboResult>(E_EventType.E_Bar_ComboResolved, OnComboResolved);
    }

    // ---------------------------------------------------------------- 生成 UI

    /// <summary>按 rows×cols 动态生成格子按钮，填进 gridContainer（挂了 GridLayoutGroup 会自动排列）</summary>
    private void BuildGrid()
    {
        cellViews.Clear();
        // GridLayoutGroup 按固定列数排列，这里设置列数与本脚本一致
        GridLayoutGroup gl = gridContainer.GetComponent<GridLayoutGroup>();
        if (gl != null)
        {
            gl.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gl.constraintCount = cols;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GridPos pos = new GridPos(r, c);
                GridCellView view = CreateCell(pos);
                cellViews.Add(pos, view);
            }
        }
    }

    /// <summary>遍历 ItemDatabase 生成物品选择按钮，填进 paletteContainer</summary>
    private void BuildPalette()
    {
        foreach (ItemDef def in ItemDatabase.Instance.All())
        {
            // 饮品是 combo 产物，不放进 palette 供手动摆放（保持"产出物只由 combo 生成"的口径）
            if (def.category == E_ItemCategory.Drink)
                continue;

            CreatePaletteButton(def);
        }
    }

    /// <summary>创建一个 palette 物品按钮（图标 + 名称，竖排）</summary>
    private void CreatePaletteButton(ItemDef def)
    {
        GameObject go = new GameObject($"Item_{def.id}", typeof(RectTransform));
        go.transform.SetParent(paletteContainer, false);

        Image bg = go.AddComponent<Image>();
        bg.color = colorFilled;

        Button btn = go.AddComponent<Button>();
        string capturedId = def.id;
        btn.onClick.AddListener(() => OnPaletteClicked(capturedId));

        // 竖排布局：图标在上、名字在下
        VerticalLayoutGroup vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(4, 4, 4, 4);

        GameObject iconGo = new GameObject("Icon", typeof(RectTransform));
        iconGo.transform.SetParent(go.transform, false);
        TMP_Text icon = iconGo.AddComponent<TextMeshProUGUI>();
        icon.alignment = TextAlignmentOptions.Center;
        icon.enableAutoSizing = true;
        icon.fontSizeMin = 16;
        icon.fontSizeMax = 36;
        icon.text = GetIcon(def.id);
        icon.raycastTarget = false;

        GameObject nameGo = new GameObject("Name", typeof(RectTransform));
        nameGo.transform.SetParent(go.transform, false);
        TMP_Text nameText = nameGo.AddComponent<TextMeshProUGUI>();
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.fontSize = 20;
        nameText.text = def.displayName;
        nameText.raycastTarget = false;
    }

    // ---------------------------------------------------------------- 交互

    /// <summary>点 palette 物品：记为当前选中</summary>
    private void OnPaletteClicked(string itemId)
    {
        selectedItemId = itemId;
        UpdateSelectionText();
    }

    /// <summary>创建一个格子按钮（背景 Image + Button + 居中图标文本）</summary>
    private GridCellView CreateCell(GridPos pos)
    {
        GameObject go = new GameObject($"Cell_{pos.row}_{pos.col}", typeof(RectTransform));
        go.transform.SetParent(gridContainer, false);

        Image bg = go.AddComponent<Image>();
        bg.color = colorEmpty;

        Button btn = go.AddComponent<Button>();
        GridPos captured = pos; // 闭包捕获
        btn.onClick.AddListener(() => OnCellClicked(captured));

        // 添加右键旋转功能（使用 PointerDown 事件并检查右键）
        EventTrigger trigger = go.AddComponent<EventTrigger>();
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => OnCellPointerDown(captured));
        trigger.triggers.Add(pointerDownEntry);

        // 图标文本（子节点，居中铺满）
        GameObject iconGo = new GameObject("Icon", typeof(RectTransform));
        iconGo.transform.SetParent(go.transform, false);
        RectTransform iconRt = iconGo.GetComponent<RectTransform>();
        iconRt.anchorMin = Vector2.zero;
        iconRt.anchorMax = Vector2.one;
        iconRt.offsetMin = Vector2.zero;
        iconRt.offsetMax = Vector2.zero;

        TMP_Text icon = iconGo.AddComponent<TextMeshProUGUI>();
        icon.alignment = TextAlignmentOptions.Center;
        icon.enableAutoSizing = true;      // 按格子大小自动缩放，长词短词都不溢出
        icon.fontSizeMin = 18;
        icon.fontSizeMax = 44;
        icon.text = "";
        icon.raycastTarget = false; // 别挡住格子按钮点击

        return new GridCellView { pos = pos, button = btn, icon = icon, background = bg };
    }

    /// <summary>点网格格子：空格摆放选中物品，占用格则移除</summary>
    private void OnCellClicked(GridPos pos)
    {
        BarGrid grid = BarGridMgr.Instance.Grid;
        PlacedItem existing = grid.GetItem(pos);

        if (existing != null)
        {
            // 已占用 → 移除（内部会重算并广播，界面通过事件刷新）
            BarGridMgr.Instance.Remove(pos);
            return;
        }

        // 空格 → 摆放当前选中物品
        if (string.IsNullOrEmpty(selectedItemId))
        {
            selectionText.text = "请先在下方选择一个物品，再点格子摆放";
            return;
        }

        PlacedItem placed = BarGridMgr.Instance.Place(selectedItemId, pos);
        if (placed == null)
            selectionText.text = "放不下：这个物品的形状会越界或和已有物品重叠，换个位置试试";
    }

    /// <summary>指针按下：检查是否右键旋转</summary>
    private void OnCellPointerDown(GridPos pos)
    {
        if (Input.GetMouseButtonDown(1)) // 右键
        {
            BarGrid grid = BarGridMgr.Instance.Grid;
            PlacedItem existing = grid.GetItem(pos);
            if (existing == null)
                return;

            bool rotated = BarGridMgr.Instance.Rotate(pos);
            if (!rotated)
                selectionText.text = "无法旋转：旋转后会越界或与其他物品重叠";
            else
                selectionText.text = $"已旋转物品：{existing.def.displayName}";
        }
    }

    // ---------------------------------------------------------------- 刷新

    /// <summary>combo 结算事件回调：每次布局变化都会被调用</summary>
    private void OnComboResolved(ComboResult result)
    {
        RefreshAll(result);
    }

    /// <summary>根据当前网格与结算结果，刷新格子显示、数值栏、combo 列表</summary>
    private void RefreshAll(ComboResult result)
    {
        BarGrid grid = BarGridMgr.Instance.Grid;

        // 收集本轮命中 combo 的发起格；多格物品要把它的所有占用格都算进高亮
        HashSet<GridPos> comboCells = new HashSet<GridPos>();
        if (result != null)
        {
            foreach (ComboHit hit in result.hits)
            {
                PlacedItem src = grid.GetItem(hit.sourcePos);
                if (src != null)
                {
                    foreach (GridPos gp in src.occupiedCells)
                        comboCells.Add(gp);
                }
                else
                {
                    comboCells.Add(hit.sourcePos);
                }
            }
        }

        // 刷新每个格子：多格物品只在锚点格显示图标，其它占用格只填色（避免图标重复）
        foreach (KeyValuePair<GridPos, GridCellView> kv in cellViews)
        {
            GridCellView view = kv.Value;
            PlacedItem item = grid.GetItem(kv.Key);
            if (item == null)
            {
                view.icon.text = "";
                view.background.color = colorEmpty;
            }
            else
            {
                bool isAnchor = kv.Key.Equals(item.pos);
                view.icon.text = isAnchor ? GetIcon(item.def.id) : "";
                view.background.color = comboCells.Contains(kv.Key) ? colorComboHi : colorFilled;
            }
        }

        // 刷新数值栏
        if (result != null)
        {
            statsText.text =
                $"白天·出杯效率 <color=#F4B942>{result.totalBrewValue}</color>    " +
                $"下午·攻击 <color=#E5533C>{result.totalAttack}</color>    " +
                $"防御 <color=#4A90D9>{result.totalDefense}</color>";
        }
        else
        {
            statsText.text = "白天·出杯效率 0    下午·攻击 0    防御 0";
        }

        // 刷新 combo 列表
        RefreshComboList(result);
    }

    private void RefreshComboList(ComboResult result)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>触发的 combo</b>");
        if (result != null && result.HasAnyCombo)
        {
            foreach (ComboHit hit in result.hits)
            {
                if (hit.rule.effect == E_ComboEffect.ProduceDrink)
                    sb.AppendLine($"· {hit.rule.ruleName} @ {hit.sourcePos} → 产出「{hit.producedDrink.displayName}」");
                else
                    sb.AppendLine($"· {hit.rule.ruleName} @ {hit.sourcePos} → 附加「{hit.rule.buffTag}」");
            }
        }
        else
        {
            sb.AppendLine("<color=#888888>（还没有 combo，试试把相关物品摆到相邻格）</color>");
        }
        comboListText.text = sb.ToString();
    }

    private void UpdateSelectionText()
    {
        if (string.IsNullOrEmpty(selectedItemId))
        {
            selectionText.text = "当前未选择物品 · 点下方物品选择，再点格子摆放；点已占用格可移除";
            return;
        }
        ItemDef def = ItemDatabase.Instance.Get(selectedItemId);
        string name = def != null ? def.displayName : selectedItemId;
        selectionText.text = $"已选择：{GetIcon(selectedItemId)} {name} · 点空格摆放，点占用格移除";
    }

    /// <summary>取物品的占位图标，没配置就按类别给默认符号</summary>
    private string GetIcon(string id)
    {
        if (id != null && iconMap.TryGetValue(id, out string icon))
            return icon;
        return "▢";
    }
}