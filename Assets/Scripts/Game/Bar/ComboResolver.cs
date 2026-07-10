using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// combo 结算器（ComboResolver）—— 把“摆放”变成“效果”的规则引擎
/// ----------------------------------------------------------------------------
/// 职责：拿一份 BarGrid 布局，按规则表算出这轮触发了哪些 combo、产出什么、
/// 累计多少经营/战斗数值，打包成 ComboResult 返回。
///
/// 它是纯计算：输入布局 → 输出结果，不改动传入网格的物品构成（只会重算 runtimeBuffs），
/// 也不广播事件。这样地牢里可以拿它做“假想布局试算”（不落地就预览战力），
/// 广播和落地由 BarGridMgr 决定。
///
/// 用 BaseManager 单例，契合框架风格；规则表内置示例，后续可替换为配置文件加载。
/// ============================================================================
/// </summary>
public class ComboResolver : BaseManager<ComboResolver>
{
    /// <summary>全部 combo 规则</summary>
    private List<ComboRule> rules = new List<ComboRule>();

    private ComboResolver()
    {
        BuildDefaultRules();
    }

    /// <summary>
    /// 内置示例规则，对应 ItemDatabase 里的示例物品，凑齐垂直切片要的 2-3 条 combo：
    ///   咖啡机 + 牛奶     → 拿铁
    ///   咖啡机 + 咖啡豆   → 美式
    ///   酒架   + 冰       → 冰镇烈酒（同时给发起物附加“灼烧”buff，供地牢战斗读取）
    ///   摇酒器 + 冰       → 鸡尾酒
    /// 扩到首发的 20-30 物品时，只需往规则表里加条目。
    /// </summary>
    private void BuildDefaultRules()
    {
        rules.Add(new ComboRule("拿铁配方", "coffee_machine", "milk",
            E_ComboEffect.ProduceDrink, resultItemId: "latte"));

        rules.Add(new ComboRule("美式配方", "coffee_machine", "coffee_bean",
            E_ComboEffect.ProduceDrink, resultItemId: "americano"));

        rules.Add(new ComboRule("冰镇烈酒配方", "liquor_rack", "ice",
            E_ComboEffect.ProduceDrink, resultItemId: "iced_liquor"));

        // 附加型 combo：冰镇烈酒旁边有冰 → 灼烧（战斗中额外伤害标签）
        rules.Add(new ComboRule("灼烧强化", "iced_liquor", "ice",
            E_ComboEffect.AttachBuff, buffTag: "burning"));

        rules.Add(new ComboRule("鸡尾酒配方", "shaker", "ice",
            E_ComboEffect.ProduceDrink, resultItemId: "cocktail"));

        // 器具联动：摇酒器旁边有杯垫 → 摇酒器附加"稳固"buff（demo 里体现器具/防御流派的联动）
        rules.Add(new ComboRule("稳固摆放", "shaker", "coaster",
            E_ComboEffect.AttachBuff, buffTag: "steady"));
    }

    /// <summary>运行时追加一条规则（例如地牢里掉落了新配方）</summary>
    public void AddRule(ComboRule rule)
    {
        if (rule != null)
            rules.Add(rule);
    }

    /// <summary>
    /// 对整份网格做一次完整 combo 结算。
    /// 流程：
    ///   1. 清空所有物品上一轮的 runtimeBuffs（重新算，避免叠加脏数据）。
    ///   2. 遍历每个已摆放物品，作为“发起方”去匹配规则；
    ///      检查它的相邻里是否存在规则要求的物品。
    ///   3. 命中则按效果类型处理：产出饮品 / 附加 buff，并累计数值。
    ///   4. 汇总成 ComboResult 返回。
    ///
    /// 注意产出的新饮品是否落回网格由参数 placeProducedIntoGrid 控制：
    ///   - 白天经营：true，产出的饮品要真的出现在吧台上供出售；
    ///   - 地牢试算：可传 false，只想知道战力不想改布局。
    /// 产出的饮品放在发起格“之上”的逻辑格里可能冲突，这里策略是：
    /// 产出物只加入 result.producedDrinks 列表，是否占格交由上层决定，
    /// 从而不破坏 BarGrid 的“一格一物”约束。
    /// </summary>
    /// <param name="grid">要结算的吧台网格</param>
    /// <returns>结算结果</returns>
    public ComboResult Resolve(BarGrid grid)
    {
        ComboResult result = new ComboResult();
        if (grid == null)
        {
            Debug.LogError("[ComboResolver] 传入的网格为 null");
            return result;
        }

        // 1. 清空上一轮派生 buff
        foreach (PlacedItem item in grid.AllItems())
            item.runtimeBuffs.Clear();

        // 2. 遍历每个物品作为发起方
        foreach (PlacedItem source in grid.AllItems())
        {
            List<PlacedItem> neighbors = grid.GetNeighbors(source);

            foreach (ComboRule rule in rules)
            {
                // 发起物必须匹配规则的 sourceItemId
                if (source.def.id != rule.sourceItemId)
                    continue;

                // 相邻里必须存在规则要求的物品
                PlacedItem matchedNeighbor = FindNeighbor(neighbors, rule.requiredNeighborId);
                if (matchedNeighbor == null)
                    continue;

                // 3. 命中，按效果处理
                if (rule.effect == E_ComboEffect.ProduceDrink)
                {
                    ItemDef drinkDef = ItemDatabase.Instance.Get(rule.resultItemId);
                    if (drinkDef == null)
                    {
                        Debug.LogWarning($"[ComboResolver] 规则「{rule.ruleName}」产物 id 无效：{rule.resultItemId}");
                        continue;
                    }
                    // 产出物挂在发起格坐标（是否占格由上层决定，见方法注释）
                    PlacedItem drink = new PlacedItem(drinkDef, source.pos);
                    result.producedDrinks.Add(drink);
                    result.totalBrewValue += drinkDef.brewValue;
                    result.totalAttack += drinkDef.attack;
                    result.totalDefense += drinkDef.defense;
                    result.hits.Add(new ComboHit(rule, source.pos, drinkDef));
                }
                else // AttachBuff
                {
                    if (!source.runtimeBuffs.Contains(rule.buffTag))
                        source.runtimeBuffs.Add(rule.buffTag);
                    result.hits.Add(new ComboHit(rule, source.pos));
                }
            }
        }

        // 4. 把网格上“原本就摆着的”物品数值也计入战斗/经营总量
        //    （产出饮品的数值已在上面加过，这里只加已有物品，避免重复）
        foreach (PlacedItem item in grid.AllItems())
        {
            result.totalAttack += item.def.attack;
            result.totalDefense += item.def.defense;
            result.totalBrewValue += item.def.brewValue;
        }

        return result;
    }

    /// <summary>在相邻列表里找指定 id 的物品，找不到返回 null</summary>
    private PlacedItem FindNeighbor(List<PlacedItem> neighbors, string wantedId)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].def.id == wantedId)
                return neighbors[i];
        }
        return null;
    }
}
