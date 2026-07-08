using System.Collections.Generic;

/// <summary>
/// ============================================================================
/// combo 效果类型
/// ----------------------------------------------------------------------------
/// 对应策划案 3.2 的两类相邻触发：
///   1. ProduceDrink —— “咖啡机旁放牛奶 → 自动产出拿铁”（生成一件新饮品）
///   2. AttachBuff   —— “烈酒旁放冰 → 冰镇烈酒，战斗中附带灼烧”（给已有物品加临时标签）
/// 拆成两种类型，是因为白天经营主要吃 ProduceDrink（出杯赚钱），
/// 下午地牢主要吃 AttachBuff（强化战斗数值），同一套规则表覆盖两个玩法。
/// ============================================================================
/// </summary>
public enum E_ComboEffect
{
    /// <summary>产出一件新饮品（在发起格生成一个 PlacedItem）</summary>
    ProduceDrink,
    /// <summary>给参与的物品附加一个 buff 标签</summary>
    AttachBuff,
}

/// <summary>
/// ============================================================================
/// combo 规则（ComboRule）
/// ----------------------------------------------------------------------------
/// 一条规则描述：当“发起物品”旁边相邻存在“所需物品”时，触发什么效果。
/// 规则用 id 匹配，写法刻意做得像“配方表”，方便策划直接填、以后搬进配置文件。
///
/// 例：
///   sourceItemId = "coffee_machine"（咖啡机）
///   requiredNeighborId = "milk"（牛奶）
///   effect = ProduceDrink, resultItemId = "latte"（产出拿铁）
///
///   sourceItemId = "iced_liquor" 或 "liquor_rack"
///   requiredNeighborId = "ice"
///   effect = AttachBuff, buffTag = "burning"（附加灼烧）
/// ============================================================================
/// </summary>
public class ComboRule
{
    /// <summary>规则可读名，纯展示/调试用，例如 "拿铁配方"</summary>
    public string ruleName;

    /// <summary>发起物品 id（combo 从它身上判定）</summary>
    public string sourceItemId;
    /// <summary>需要相邻存在的物品 id</summary>
    public string requiredNeighborId;

    /// <summary>触发的效果类型</summary>
    public E_ComboEffect effect;

    /// <summary>effect=ProduceDrink 时：产出的饮品 id</summary>
    public string resultItemId;
    /// <summary>effect=AttachBuff 时：附加的 buff 标签</summary>
    public string buffTag;

    public ComboRule(string ruleName, string sourceItemId, string requiredNeighborId,
                     E_ComboEffect effect, string resultItemId = null, string buffTag = null)
    {
        this.ruleName = ruleName;
        this.sourceItemId = sourceItemId;
        this.requiredNeighborId = requiredNeighborId;
        this.effect = effect;
        this.resultItemId = resultItemId;
        this.buffTag = buffTag;
    }
}

/// <summary>
/// ============================================================================
/// 单次触发记录（ComboHit）—— 结算结果里的一条“发生了什么”
/// ----------------------------------------------------------------------------
/// 供 UI 播特效、经营算钱、战斗算伤害时逐条读取。
/// ============================================================================
/// </summary>
public class ComboHit
{
    /// <summary>命中的规则</summary>
    public ComboRule rule;
    /// <summary>发起物品所在格</summary>
    public GridPos sourcePos;
    /// <summary>ProduceDrink 时产出的饮品定义；AttachBuff 时为 null</summary>
    public ItemDef producedDrink;

    public ComboHit(ComboRule rule, GridPos sourcePos, ItemDef producedDrink = null)
    {
        this.rule = rule;
        this.sourcePos = sourcePos;
        this.producedDrink = producedDrink;
    }
}

/// <summary>
/// ============================================================================
/// combo 结算结果（ComboResult）—— 一整轮结算的汇总
/// ----------------------------------------------------------------------------
/// 通过事件 E_Bar_ComboResolved 广播给三个玩法：
///   - 白天经营读 totalBrewValue / producedDrinks 算出杯效率与收入
///   - 下午地牢读 totalAttack / totalDefense 算战斗数值
///   - UI 读 hits 列表逐条播放 combo 连锁特效（策划案的短视频爽感卖点）
/// ============================================================================
/// </summary>
public class ComboResult
{
    /// <summary>本轮所有触发记录</summary>
    public List<ComboHit> hits = new List<ComboHit>();
    /// <summary>本轮新产出的饮品（ProduceDrink 的产物）</summary>
    public List<PlacedItem> producedDrinks = new List<PlacedItem>();

    /// <summary>出杯效率总和（白天经营用）</summary>
    public int totalBrewValue;
    /// <summary>攻击力总和（地牢战斗用）</summary>
    public int totalAttack;
    /// <summary>防御力总和（地牢战斗用）</summary>
    public int totalDefense;

    /// <summary>本轮是否有任何 combo 触发</summary>
    public bool HasAnyCombo => hits.Count > 0;
}
