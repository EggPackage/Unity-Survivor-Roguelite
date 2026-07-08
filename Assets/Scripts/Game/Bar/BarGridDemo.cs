using System.Text;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 吧台核心玩法验证入口（BarGridDemo）
/// ----------------------------------------------------------------------------
/// 对应策划案阶段0：“用占位美术验证核心循环好不好玩”。
/// 这个脚本不含任何美术，纯用 Debug.Log 打印，用来在编辑器里一键验证：
///   摆物品 → 相邻触发 combo → 产出饮品 / 附加 buff → 算出经营与战斗数值。
///
/// 用法：把本脚本挂到场景里任意 GameObject 上，运行即可在 Console 看到结果；
/// 或运行后在 Inspector 勾选 rerun 复跑。
///
/// 它演示的正是策划案“经营布局即战斗阵型”：同一份 3×3 布局，
/// 既能读出白天出杯效率（brewValue），又能读出下午战斗攻击力（attack）。
/// ============================================================================
/// </summary>
public class BarGridDemo : MonoBehaviour
{
    [Tooltip("勾选后在运行时复跑一次演示")]
    public bool rerun = false;

    private void Start()
    {
        // 监听结算事件，证明事件链路通畅（三个玩法就是这样接入核心的）
        EventCenter.Instance.AddEventListener<ComboResult>(E_EventType.E_Bar_ComboResolved, OnComboResolved);
        RunDemo();
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<ComboResult>(E_EventType.E_Bar_ComboResolved, OnComboResolved);
    }

    private void Update()
    {
        // Inspector 里勾 rerun 即可复跑，方便反复调布局看 combo 变化
        if (rerun)
        {
            rerun = false;
            RunDemo();
        }
    }

    /// <summary>
    /// 摆一套示例布局并结算：
    ///   (1,1) 咖啡机  —— 中心
    ///   (0,1) 牛奶    —— 咖啡机上方 → 触发“拿铁配方”
    ///   (1,0) 咖啡豆  —— 咖啡机左侧 → 触发“美式配方”
    ///   (2,2) 酒架, (2,1) 冰 —— 相邻 → 触发“冰镇烈酒配方”
    /// </summary>
    private void RunDemo()
    {
        BarGridMgr mgr = BarGridMgr.Instance;
        // 重建 3×3 干净网格（四邻判定）
        mgr.InitGrid(3, 3, useDiagonal: false);

        // 摆放（每次 Place 内部都会重算并广播，这里最后一次的结果就是完整布局的结果）
        mgr.Place("coffee_machine", new GridPos(1, 1));
        mgr.Place("milk", new GridPos(0, 1));
        mgr.Place("coffee_bean", new GridPos(1, 0));
        mgr.Place("liquor_rack", new GridPos(2, 2));
        mgr.Place("ice", new GridPos(2, 1));

        // 主动取最终结果打印一份布局总览（事件里已经打过每一步，这里做汇总）
        ComboResult result = mgr.LastResult;
        Debug.Log(BuildReport(mgr.Grid, result));
    }

    /// <summary>combo 结算事件回调：每次布局变化都会被调用一次</summary>
    private void OnComboResolved(ComboResult result)
    {
        if (result.HasAnyCombo)
            Debug.Log($"[BarGridDemo] combo 结算：触发 {result.hits.Count} 条，" +
                      $"出杯效率 {result.totalBrewValue}，攻击 {result.totalAttack}，防御 {result.totalDefense}");
    }

    /// <summary>拼一份可读的布局与结算报告</summary>
    private string BuildReport(BarGrid grid, ComboResult result)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("========== 吧台布局结算报告 ==========");
        sb.AppendLine($"网格 {grid.Rows}×{grid.Cols}，已摆放 {grid.Count} 个物品");

        sb.AppendLine("--- 触发的 combo ---");
        if (result.HasAnyCombo)
        {
            foreach (ComboHit hit in result.hits)
            {
                if (hit.rule.effect == E_ComboEffect.ProduceDrink)
                    sb.AppendLine($"  · {hit.rule.ruleName} @ {hit.sourcePos} → 产出「{hit.producedDrink.displayName}」");
                else
                    sb.AppendLine($"  · {hit.rule.ruleName} @ {hit.sourcePos} → 附加 buff「{hit.rule.buffTag}」");
            }
        }
        else
        {
            sb.AppendLine("  （无 combo 触发）");
        }

        sb.AppendLine("--- 数值汇总 ---");
        sb.AppendLine($"  白天经营 · 出杯效率总和：{result.totalBrewValue}");
        sb.AppendLine($"  下午地牢 · 攻击力总和：{result.totalAttack}");
        sb.AppendLine($"  下午地牢 · 防御力总和：{result.totalDefense}");
        sb.AppendLine($"  本轮新产出饮品数：{result.producedDrinks.Count}");
        sb.AppendLine("=====================================");
        return sb.ToString();
    }
}
