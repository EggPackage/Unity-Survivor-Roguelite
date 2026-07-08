using System.Collections.Generic;

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
    /// <summary>它在网格上的位置</summary>
    public GridPos pos;

    /// <summary>本轮 combo 结算附加的临时增益标签，例如 "burning"（灼烧）。每次结算前清空。</summary>
    public List<string> runtimeBuffs = new List<string>();

    public PlacedItem(ItemDef def, GridPos pos)
    {
        this.def = def;
        this.pos = pos;
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
    /// 在指定格子摆放一个物品。
    /// 返回摆放成功的 PlacedItem；越界、格子已占用、物品 id 无效则返回 null。
    /// 注意：这里只改数据，不广播事件也不结算 combo——那是 BarGridMgr 的职责，
    /// 保持 BarGrid 的纯粹，方便单元测试和地牢里的“假想布局试算”。
    /// </summary>
    public PlacedItem Place(string itemId, GridPos pos)
    {
        if (!IsInBounds(pos))
            return null;
        if (!IsEmpty(pos))
            return null;

        ItemDef def = ItemDatabase.Instance.Get(itemId);
        if (def == null)
        {
            UnityEngine.Debug.LogWarning($"[BarGrid] 未知物品 id：{itemId}");
            return null;
        }

        PlacedItem item = new PlacedItem(def, pos);
        cells[pos] = item;
        return item;
    }

    /// <summary>
    /// 移除某格物品，返回被移除的实例；格子本就为空则返回 null。
    /// </summary>
    public PlacedItem Remove(GridPos pos)
    {
        if (cells.TryGetValue(pos, out PlacedItem item))
        {
            cells.Remove(pos);
            return item;
        }
        return null;
    }

    /// <summary>
    /// 把物品从 from 移动到 to。
    /// 只有 from 有物品、to 为空且都在界内时成功。用于玩家拖动重排布局。
    /// </summary>
    public bool Move(GridPos from, GridPos to)
    {
        if (!IsInBounds(to) || !IsEmpty(to))
            return false;
        PlacedItem item = Remove(from);
        if (item == null)
            return false;
        item.pos = to;
        cells[to] = item;
        return true;
    }

    /// <summary>
    /// 取某格子的所有相邻物品（跳过空格子和越界）。
    /// 这是 combo 判定的基础：ComboResolver 靠它知道“谁挨着谁”。
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

    /// <summary>遍历所有已摆放的物品（顺序不保证）。结算和存档都用它。</summary>
    public IEnumerable<PlacedItem> AllItems() => cells.Values;

    /// <summary>当前摆放的物品数量</summary>
    public int Count => cells.Count;

    /// <summary>清空整个网格</summary>
    public void Clear() => cells.Clear();
}
