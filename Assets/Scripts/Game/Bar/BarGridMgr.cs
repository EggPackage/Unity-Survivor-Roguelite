using UnityEngine;

/// <summary>
/// ============================================================================
/// 吧台网格管理器（BarGridMgr）—— 核心系统对外的唯一入口
/// ----------------------------------------------------------------------------
/// 对应策划案 3.2 的“唯一核心系统”。它把三块拼起来并对外统一：
///   BarGrid（数据）+ ComboResolver（规则计算）+ EventCenter（广播）
///
/// 分层理由：
///   - BarGrid 只管数据、ComboResolver 只管算，都不知道“游戏正在发生什么”；
///   - 真正“玩家摆了一个物品 → 重算 combo → 通知白天经营/地牢/UI”的编排放在这里。
/// 白天经营、下午地牢、夜晚酒馆、UI 面板都通过监听本管理器广播的事件来响应，
/// 从而实现“同一套摆放系统驱动三个玩法”。
///
/// 契合框架：BaseManager 反射单例 + EventCenter 事件广播（E_Bar_* 系列）。
/// ============================================================================
/// </summary>
public class BarGridMgr : BaseManager<BarGridMgr>
{
    /// <summary>当前吧台网格。默认 3×3，对应垂直切片规格；可用 InitGrid 重建。</summary>
    public BarGrid Grid { get; private set; }

    /// <summary>最近一次 combo 结算结果，供 UI/其它系统随时查询</summary>
    public ComboResult LastResult { get; private set; }

    private BarGridMgr()
    {
        // 默认建一个 3×3 网格（策划案阶段0 垂直切片规格）
        Grid = new BarGrid(3, 3);
    }

    /// <summary>
    /// 重建网格（例如养成解锁后从 3×3 扩到 5×5）。
    /// 会清空当前布局并重算一次 combo。
    /// </summary>
    public void InitGrid(int rows, int cols, bool useDiagonal = false)
    {
        Grid = new BarGrid(rows, cols) { UseDiagonal = useDiagonal };
        Resolve();
    }

    /// <summary>
    /// 摆放一个物品并立即重算 combo。
    /// 成功返回落位的 PlacedItem，失败（越界/占用/无效 id）返回 null。
    /// </summary>
    public PlacedItem Place(string itemId, GridPos pos, int rotation = 0)
    {
        PlacedItem item = Grid.Place(itemId, pos, rotation);
        if (item == null)
            return null;

        // 广播“摆放”事件（UI 更新格子显示等）
        EventCenter.Instance.EventTrigger(E_EventType.E_Bar_ItemPlaced, item);
        // 布局变了，重算 combo 并广播结果
        Resolve();
        return item;
    }

    /// <summary>
    /// 移除某格物品并重算 combo。
    /// 返回被移除的物品，格子为空则返回 null。
    /// </summary>
    public PlacedItem Remove(GridPos pos)
    {
        PlacedItem item = Grid.Remove(pos);
        if (item == null)
            return null;

        EventCenter.Instance.EventTrigger(E_EventType.E_Bar_ItemRemoved, item);
        Resolve();
        return item;
    }

    /// <summary>
    /// 移动物品（拖动重排）并重算 combo。返回是否成功。
    /// </summary>
    public bool Move(GridPos from, GridPos to)
    {
        bool ok = Grid.Move(from, to);
        if (ok)
            Resolve();
        return ok;
    }

    /// <summary>
    /// 将某格物品旋转90°（顺时针）并重算 combo。返回是否成功。
    /// </summary>
    public bool Rotate(GridPos pos, int rotations = 1)
    {
        bool ok = Grid.Rotate(pos, rotations);
        if (ok)
            Resolve();
        return ok;
    }

    /// <summary>
    /// 主动触发一次结算并广播结果。
    /// 三个玩法都监听 E_Bar_ComboResolved 拿到 ComboResult 后各取所需：
    ///   经营取 totalBrewValue、地牢取 totalAttack/Defense、UI 取 hits 播特效。
    /// </summary>
    public ComboResult Resolve()
    {
        LastResult = ComboResolver.Instance.Resolve(Grid);
        EventCenter.Instance.EventTrigger(E_EventType.E_Bar_ComboResolved, LastResult);
        return LastResult;
    }
}