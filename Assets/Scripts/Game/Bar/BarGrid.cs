using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 网格坐标（GridPos）
/// ----------------------------------------------------------------------------
/// 用行列表示吧台上的一个格子。做成 struct（值类型）方便当字典 key、做相等比较。
/// ============================================================================
/// </summary>
public struct GridPos
{
    public int row;
    public int col;

    public GridPos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    // 重写相等，才能可靠地放进字典/集合并做坐标比较
    public override bool Equals(object obj)
    {
        if (obj is GridPos other)
            return row == other.row && col == other.col;
        return false;
    }

    public override int GetHashCode() => row * 397 ^ col;

    public override string ToString() => $"({row},{col})";
}

/// <summary>
/// ============================================================================
/// 已摆放物品（PlacedItem）
/// ----------------------------------------------------------------------------
/// “某个物品实例摆在某个格子上”的运行时数据。
/// 与 ItemDef 的区别：ItemDef 是模板（拿铁是什么），PlacedItem 是实例（第2行第1格摆着一杯拿铁）。
/// runtimeBuffs 记录本次 combo 结算给它附加的临时效果（例如“冰镇→灼烧”），
/// 每次重新结算前会被清空重算，所以它是“派生数据”，不进存档。
/// ============================================================================
/// </summary>
public class PlacedItem
{
    /// <summary>它是什么物品（引用数据库里的定义）</summary>
    public ItemDef def;
    /// <summary>锚点格坐标（形状 (0,0) 对应的格子）。多格物品的"主位置"。</summary>
    public GridPos pos;
    /// <summary>旋转角度（0=0°, 1=90°, 2=180°, 3=270°）</summary>
    public int rotation;

    /// <summary>
    /// 本物品实际占用的所有格子（含锚点）。单格物品就一个元素，
    /// 多格物品是锚点 + 形状偏移算出的全部格子。移除/相邻判定都基于它。
    /// </summary>
    public List<GridPos> occupiedCells = new List<GridPos>();

    /// <summary>本轮 combo 结算附加的临时增益标签，例如 "burning"（灼烧）。每次结算前清空。</summary>
    public List<string> runtimeBuffs = new List<string>();

    public PlacedItem(ItemDef def, GridPos pos, int rotation = 0)
    {
        this.def = def;
        this.pos = pos;
        this.rotation = rotation;
    }
}

/// <summary>
/// ============================================================================
/// 吧台网格（BarGrid）—— 整个游戏的唯一核心数据结构
/// ----------------------------------------------------------------------------
/// 对应策划案 3.2：“吧台网格是唯一的核心系统，三个玩法都复用它。”
/// 这个类只负责“格子上摆了什么、谁挨着谁”的纯数据管理，不含任何玩法规则、
/// 不含 Unity 场景逻辑。combo 怎么算交给 ComboResolver，事件广播交给 BarGridMgr。
/// 这样白天经营、下午地牢、夜晚酒馆读的是同一份 BarGrid，天然咬合。
///
/// 设计取舍：
///   - 用字典 pos->PlacedItem 存放，稀疏存储，空格子不占内存，遍历只遍历有物品的格子。
///   - 相邻查询支持四邻/八邻切换（策划待定项之一：combo 判定用四邻还是八邻）。
/// ============================================================================
/// </summary>
public class BarGrid
{
    /// <summary>网格行数</summary>
    public int Rows { get; private set; }
    /// <summary>网格列数</summary>
    public int Cols { get; private set; }

    /// <summary>true=八邻（含斜角），false=四邻（上下左右）。默认四邻，符合大多数摆放 combo 直觉。</summary>
    public bool UseDiagonal { get; set; } = false;

    /// <summary>格子内容：只记录“有物品”的格子</summary>
    private Dictionary<GridPos, PlacedItem> cells = new Dictionary<GridPos, PlacedItem>();

    // 四邻方向偏移
    private static readonly int[,] dir4 = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
    // 八邻方向偏移（四邻 + 四个斜角）
    private static readonly int[,] dir8 =
    {
        { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 },
        { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }
    };

    public BarGrid(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
    }

    /// <summary>坐标是否在网格范围内</summary>
    public bool IsInBounds(GridPos pos)
        => pos.row >= 0 && pos.row < Rows && pos.col >= 0 && pos.col < Cols;

    /// <summary>该格子是否为空（可摆放）</summary>
    public bool IsEmpty(GridPos pos) => !cells.ContainsKey(pos);

    /// <summary>取某格上的物品，空则返回 null</summary>
    public PlacedItem GetItem(GridPos pos)
    {
        cells.TryGetValue(pos, out PlacedItem item);
        return item;
    }

    /// <summary>
    /// 将单个形状偏移点按指定角度旋转（顺时针）
    /// </summary>
    private static Vector2Int RotateOffset(Vector2Int offset, int rotation)
    {
        switch (rotation % 4)
        {
            case 0: // 0°
                return offset;
            case 1: // 90°
                return new Vector2Int(offset.y, -offset.x);
            case 2: // 180°
                return new Vector2Int(-offset.x, -offset.y);
            case 3: // 270°
                return new Vector2Int(-offset.y, offset.x);
            default:
                return offset;
        }
    }

    /// <summary>
    /// 按物品形状算出以 anchor 为锚点时实际占用的所有格子（支持旋转）。
    /// 形状偏移 (dx,dy) 中 x 加到行、y 加到列。
    /// </summary>
    public static List<GridPos> GetCellsForShape(ItemDef def, GridPos anchor, int rotation = 0)
    {
        List<GridPos> result = new List<GridPos>();
        List<Vector2Int> shape = (def != null && def.shapeCells != null && def.shapeCells.Count > 0)
            ? def.shapeCells
            : new List<Vector2Int> { new Vector2Int(0, 0) };
        foreach (Vector2Int off in shape)
        {
            Vector2Int rotated = RotateOffset(off, rotation);
            result.Add(new GridPos(anchor.row + rotated.x, anchor.col + rotated.y));
        }
        return result;
    }

    /// <summary>
    /// 判断把某物品摆在 anchor 处是否合法：所有占用格都在界内、且都为空。
    /// ignore 参数用于 Move 时排除物品自身占用的格子（不算冲突）。
    /// </summary>
    public bool CanPlace(ItemDef def, GridPos anchor, PlacedItem ignore = null, int rotation = 0)
    {
        if (def == null)
            return false;
        foreach (GridPos gp in GetCellsForShape(def, anchor, rotation))
        {
            if (!IsInBounds(gp))
                return false;
            PlacedItem occ = GetItem(gp);
            if (occ != null && occ != ignore)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 在 anchor 处摆放一个物品（按其形状占用多个格子，支持旋转）。
    /// 返回摆放成功的 PlacedItem；越界、任一占用格被占、物品 id 无效则返回 null。
    /// 只改数据，不广播事件也不结算 combo——那是 BarGridMgr 的职责。
    /// </summary>
    public PlacedItem Place(string itemId, GridPos anchor, int rotation = 0)
    {
        ItemDef def = ItemDatabase.Instance.Get(itemId);
        if (def == null)
        {
            UnityEngine.Debug.LogWarning($"[BarGrid] 未知物品 id：{itemId}");
            return null;
        }
        if (!CanPlace(def, anchor, null, rotation))
            return null;

        PlacedItem item = new PlacedItem(def, anchor, rotation);
        item.occupiedCells = GetCellsForShape(def, anchor, rotation);
        // 每个占用格都指向同一个 PlacedItem，这样点任意一格都能找到它
        foreach (GridPos gp in item.occupiedCells)
            cells[gp] = item;
        return item;
    }

    /// <summary>
    /// 移除某格所属的物品（多格物品会整体移除所有占用格），返回被移除的实例；
    /// 格子本就为空则返回 null。
    /// </summary>
    public PlacedItem Remove(GridPos pos)
    {
        if (!cells.TryGetValue(pos, out PlacedItem item))
            return null;
        // 按该物品的占用格整体清除（点它任意一格都能移除整体）
        foreach (GridPos gp in item.occupiedCells)
            cells.Remove(gp);
        return item;
    }

    /// <summary>
    /// 把 from 格所属物品整体移动，使其锚点落到 toAnchor。
    /// 需要目标形状占用的所有格都在界内且为空（排除自身当前占用格）。
    /// 用于玩家拖动重排布局。
    /// </summary>
    public bool Move(GridPos from, GridPos toAnchor)
    {
        if (!cells.TryGetValue(from, out PlacedItem item))
            return false;
        if (!CanPlace(item.def, toAnchor, ignore: item, rotation: item.rotation))
            return false;

        // 先清除旧占用格，再按新锚点写入
        foreach (GridPos gp in item.occupiedCells)
            cells.Remove(gp);
        item.pos = toAnchor;
        item.occupiedCells = GetCellsForShape(item.def, toAnchor, item.rotation);
        foreach (GridPos gp in item.occupiedCells)
            cells[gp] = item;
        return true;
    }

    /// <summary>
    /// 将某格所属物品旋转90°（顺时针）。
    /// 需要旋转后的形状占用格都在界内且为空（排除自身当前占用格）。
    /// </summary>
    /// <param name="pos">物品所在的任意一格</param>
    /// <param name="rotations">旋转次数（每次90°，默认为1次）</param>
    /// <returns>是否旋转成功</returns>
    public bool Rotate(GridPos pos, int rotations = 1)
    {
        if (!cells.TryGetValue(pos, out PlacedItem item))
            return false;

        int newRotation = (item.rotation + rotations) % 4;
        if (!CanPlace(item.def, item.pos, ignore: item, rotation: newRotation))
            return false;

        // 先清除旧占用格，再按新旋转写入
        foreach (GridPos gp in item.occupiedCells)
            cells.Remove(gp);
        item.rotation = newRotation;
        item.occupiedCells = GetCellsForShape(item.def, item.pos, item.rotation);
        foreach (GridPos gp in item.occupiedCells)
            cells[gp] = item;
        return true;
    }

    /// <summary>
    /// 取某个格子的所有相邻物品（跳过空格子和越界）。
    /// 单格判定用；多格物品请用 GetNeighbors(PlacedItem)。
    /// </summary>
    public List<PlacedItem> GetNeighbors(GridPos pos)
    {
        List<PlacedItem> result = new List<PlacedItem>();
        int[,] dirs = UseDiagonal ? dir8 : dir4;
        int count = dirs.GetLength(0);
        for (int i = 0; i < count; i++)
        {
            GridPos n = new GridPos(pos.row + dirs[i, 0], pos.col + dirs[i, 1]);
            if (!IsInBounds(n))
                continue;
            PlacedItem item = GetItem(n);
            if (item != null)
                result.Add(item);
        }
        return result;
    }

    /// <summary>
    /// 取某个物品的所有相邻物品：遍历它的每一个占用格的外部邻居，
    /// 去重并排除物品自身（自己的其它格子不算邻居）。
    /// 这是多格物品 combo 判定的正确基础。
    /// </summary>
    public List<PlacedItem> GetNeighbors(PlacedItem self)
    {
        List<PlacedItem> result = new List<PlacedItem>();
        if (self == null)
            return result;

        HashSet<PlacedItem> seen = new HashSet<PlacedItem>();
        int[,] dirs = UseDiagonal ? dir8 : dir4;
        int count = dirs.GetLength(0);

        foreach (GridPos cell in self.occupiedCells)
        {
            for (int i = 0; i < count; i++)
            {
                GridPos n = new GridPos(cell.row + dirs[i, 0], cell.col + dirs[i, 1]);
                if (!IsInBounds(n))
                    continue;
                PlacedItem item = GetItem(n);
                if (item == null || item == self)
                    continue;
                if (seen.Add(item))
                    result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// 遍历所有已摆放的物品（顺序不保证，且已去重）。结算和存档都用它。
    /// 注意：多格物品在 cells 里有多个 key 指向同一实例，这里用 HashSet 去重，
    /// 避免 ComboResolver 把同一个物品算多次。
    /// </summary>
    public IEnumerable<PlacedItem> AllItems()
    {
        HashSet<PlacedItem> seen = new HashSet<PlacedItem>();
        foreach (PlacedItem item in cells.Values)
        {
            if (seen.Add(item))
                yield return item;
        }
    }

    /// <summary>当前摆放的物品数量（按物品实例去重，不是占用格数）</summary>
    public int Count
    {
        get
        {
            HashSet<PlacedItem> seen = new HashSet<PlacedItem>();
            foreach (PlacedItem item in cells.Values)
                seen.Add(item);
            return seen.Count;
        }
    }

    /// <summary>清空整个网格</summary>
    public void Clear() => cells.Clear();
}