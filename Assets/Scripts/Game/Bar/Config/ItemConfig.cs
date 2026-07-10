using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 物品配置（ItemConfig）—— ScriptableObject 版的物品定义
/// ----------------------------------------------------------------------------
/// 对应策划案"数据驱动"目标：物品的所有属性（含形状）都存在 .asset 资源里，
/// 策划用编辑器点一点就能改，运行时 ItemDatabase 读它转成 ItemDef 使用。
///
/// 与 ItemDef（运行时纯数据类）的关系：
///   ItemConfig 是"磁盘上的配置资源"（可在 Inspector / 形状编辑器里编辑）；
///   ItemDef 是"运行时内存里的模板"。ItemDatabase 负责把前者转成后者。
///   这样运行时逻辑（BarGrid / ComboResolver）完全不依赖 ScriptableObject，
///   保持纯 C#、可测试。
///
/// 新增的核心字段是 shapeCells —— 物品占用的格子形状（相对偏移列表）。
/// ============================================================================
/// </summary>
[CreateAssetMenu(fileName = "Item_", menuName = "早咖晚酒/物品配置", order = 0)]
public class ItemConfig : ScriptableObject
{
    [Header("基础")]
    [Tooltip("唯一 id，规则表和存档都靠它引用，例如 coffee_machine")]
    public string id;
    [Tooltip("显示名，例如 咖啡机")]
    public string displayName;
    [Tooltip("物品类别，参与 combo 判定")]
    public E_ItemCategory category;
    [Tooltip("占位/正式图标资源路径（Resources 下），可空")]
    public string iconPath;

    [Header("经营数值（白天咖啡店）")]
    [Tooltip("作为饮品出售时的售价；非饮品填 0")]
    public int sellPrice;
    [Tooltip("出杯效率贡献值：越高说明这份布局白天越忙得过来")]
    public int brewValue;

    [Header("战斗数值（下午地牢）")]
    [Tooltip("攻击力（饮品当作攻击手段时使用）")]
    public int attack;
    [Tooltip("防御力（器具当作防御时使用）")]
    public int defense;

    [Header("形状（相对格子偏移，由物品形状编辑器生成）")]
    [Tooltip("物品占用的格子集合，以锚点为 (0,0)。\n" +
             "单格 = [(0,0)]；1×3 竖条 = [(0,0),(1,0),(2,0)]；2×2 = [(0,0),(0,1),(1,0),(1,1)]。\n" +
             "为空时运行时按单格处理（向后兼容）。x=行偏移，y=列偏移。")]
    public List<Vector2Int> shapeCells = new List<Vector2Int> { new Vector2Int(0, 0) };

    /// <summary>返回有效形状：为空时回退为单格，保证运行时永远有至少一个占用格</summary>
    public List<Vector2Int> GetShapeOrSingle()
    {
        if (shapeCells == null || shapeCells.Count == 0)
            return new List<Vector2Int> { new Vector2Int(0, 0) };
        return shapeCells;
    }
}
