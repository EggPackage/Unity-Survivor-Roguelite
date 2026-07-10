using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 物品类别枚举
/// ----------------------------------------------------------------------------
/// 对应策划案 3.2：吧台格子里能摆放的几大类物品。
/// 类别本身会参与 combo 判定（例如“咖啡机 + 原料”才产出饮品），
/// 所以把类别独立成枚举，方便规则表按类别写触发条件，而不用枚举每一个具体物品。
/// ============================================================================
/// </summary>
public enum E_ItemCategory
{
    /// <summary>设备：咖啡机、酒架等，本身不消耗，是 combo 的“发起方”</summary>
    Equipment,
    /// <summary>原料：牛奶、咖啡豆、冰等，被设备/配方消耗，是 combo 的“材料方”</summary>
    Material,
    /// <summary>饮品：拿铁、冰镇烈酒等，combo 的产物，白天用来卖、地牢里当攻击/增益</summary>
    Drink,
    /// <summary>器具：摇酒器、杯垫等，提供联动或防御，多在地牢里发挥作用</summary>
    Utensil,
}

/// <summary>
/// ============================================================================
/// 物品静态定义（ItemDefinition）
/// ----------------------------------------------------------------------------
/// 描述“一种物品是什么”，是不变的模板数据（区别于 PlacedItem——那是“摆在某个格子上的一份实例”）。
/// 设计要点：
///   1. 纯数据类，不含任何 Unity 逻辑，方便以后从 JSON / ScriptableObject 读取，
///      呼应策划案“数据驱动、加内容不改核心代码”的目标。
///   2. 战斗数值（attack / defense）和经营数值（sellPrice / brewValue）都挂在这里，
///      因为策划案的灵魂是“同一份布局，白天算生意、下午算战斗”，同一个物品要同时具备两套数值。
/// ============================================================================
/// </summary>
public class ItemDef
{
    /// <summary>唯一 id，规则表和存档都靠它引用物品，例如 "coffee_machine"</summary>
    public string id;
    /// <summary>显示名，例如 "咖啡机"</summary>
    public string displayName;
    /// <summary>物品类别，参与 combo 判定</summary>
    public E_ItemCategory category;

    /// <summary>占位美术 / 正式图标的资源路径（Resources 下），核心逻辑不依赖它是否存在</summary>
    public string iconPath;

    // ---- 经营数值（白天咖啡店用）----
    /// <summary>作为饮品出售时的售价；非饮品填 0</summary>
    public int sellPrice;
    /// <summary>出杯效率贡献值：combo 顺畅度的度量，越高说明这份布局白天越忙得过来</summary>
    public int brewValue;

    // ---- 战斗数值（下午地牢用）----
    /// <summary>战斗中的攻击力（饮品当作攻击手段时使用）</summary>
    public int attack;
    /// <summary>战斗中的防御力（器具当作防御时使用）</summary>
    public int defense;

    /// <summary>
    /// 形状：物品占用的格子集合，以锚点为 (0,0) 的相对偏移（x=行、y=列）。
    /// 单格 = [(0,0)]；由 ItemConfig 配置而来，为空时按单格处理。
    /// </summary>
    public List<Vector2Int> shapeCells;

    public ItemDef(string id, string displayName, E_ItemCategory category,
                   int sellPrice = 0, int brewValue = 0, int attack = 0, int defense = 0,
                   string iconPath = null, List<Vector2Int> shapeCells = null)
    {
        this.id = id;
        this.displayName = displayName;
        this.category = category;
        this.sellPrice = sellPrice;
        this.brewValue = brewValue;
        this.attack = attack;
        this.defense = defense;
        this.iconPath = iconPath;
        // 形状为空时回退单格，保证运行时永远有至少一个占用格
        this.shapeCells = (shapeCells != null && shapeCells.Count > 0)
            ? shapeCells
            : new List<Vector2Int> { new Vector2Int(0, 0) };
    }
}

/// <summary>
/// ============================================================================
/// 物品数据库
/// ----------------------------------------------------------------------------
/// 用 id 索引所有 ItemDef 的中心表。
/// 现在用代码内置几个示例物品（对应策划案“阶段0 垂直切片：5-6 种可摆放物品”），
/// 后续要扩到 20-30 种时，只需把 BuildDefaultDatabase 换成从配置文件读取即可，
/// 上层的网格 / combo 逻辑完全不用改——这就是数据驱动的意义。
/// ============================================================================
/// </summary>
public class ItemDatabase : BaseManager<ItemDatabase>
{
    /// <summary>id -> 物品定义</summary>
    private Dictionary<string, ItemDef> dict = new Dictionary<string, ItemDef>();

    // 私有构造：契合框架 BaseManager 的反射单例约定
    private ItemDatabase()
    {
        // 优先从 ScriptableObject 配置加载（数据驱动，含形状）；
        // 加载不到（如迁移前还没生成 .asset）则回退内置数据，保证任何时候都能跑。
        int loaded = LoadFromConfigs();
        if (loaded == 0)
        {
            UnityEngine.Debug.Log("[ItemDatabase] 未找到 Resources/ItemConfigs 下的配置，使用内置默认数据。" +
                                  "可用菜单「早咖晚酒/生成初始物品配置」生成 SO 资源。");
            BuildDefaultDatabase();
        }
    }

    /// <summary>
    /// 从 Resources/ItemConfigs 加载所有 ItemConfig（ScriptableObject），转成运行时 ItemDef。
    /// 返回加载到的数量。这是"数据驱动"的落地：物品与形状都由 .asset 配置，改数据不改代码。
    /// </summary>
    private int LoadFromConfigs()
    {
        ItemConfig[] configs = Resources.LoadAll<ItemConfig>("ItemConfigs");
        if (configs == null || configs.Length == 0)
            return 0;

        foreach (ItemConfig cfg in configs)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.id))
                continue;
            // 复制一份形状列表，避免运行时误改到资源
            List<Vector2Int> shape = new List<Vector2Int>(cfg.GetShapeOrSingle());
            Register(new ItemDef(cfg.id, cfg.displayName, cfg.category,
                cfg.sellPrice, cfg.brewValue, cfg.attack, cfg.defense, cfg.iconPath, shape));
        }
        return dict.Count;
    }

    /// <summary>
    /// 内置的示例物品表。对应垂直切片需要的最小集合：
    /// 咖啡机 / 牛奶 / 咖啡豆 / 烈酒 / 冰 / 摇酒器，
    /// 足以演示 3-4 条 combo（拿铁、美式、冰镇烈酒、鸡尾酒）。
    ///
    /// 数值设计（demo 阶段，为让"白天赚钱 vs 下午打架"的取舍看得出来）：
    ///   - 咖啡系（拿铁/美式）：偏 brewValue，白天出杯流派
    ///   - 酒系（冰镇烈酒/鸡尾酒）：偏 attack，下午战斗流派
    ///   - 器具（摇酒器/杯垫）：偏 defense，防御向
    ///   - 设备本身也给一点基础值，避免空布局全 0 看不出差异
    /// 这套数值只用于手感验证，正式平衡后续再调（策划案 7.3）。
    /// </summary>
    private void BuildDefaultDatabase()
    {
        // ---- 设备（combo 发起方，给一点基础出杯效率，代表"这台机器本身能干活"）----
        Register(new ItemDef("coffee_machine", "咖啡机", E_ItemCategory.Equipment, brewValue: 1));
        Register(new ItemDef("liquor_rack", "酒架", E_ItemCategory.Equipment, attack: 1));

        // ---- 原料（本身无数值，靠 combo 才产生价值）----
        Register(new ItemDef("milk", "牛奶", E_ItemCategory.Material));
        Register(new ItemDef("coffee_bean", "咖啡豆", E_ItemCategory.Material));
        Register(new ItemDef("ice", "冰", E_ItemCategory.Material));

        // ---- 器具（防御向，主要在地牢发挥作用）----
        Register(new ItemDef("shaker", "摇酒器", E_ItemCategory.Utensil, defense: 3));
        Register(new ItemDef("coaster", "杯垫", E_ItemCategory.Utensil, defense: 2));

        // ---- 饮品（combo 的产物，也可直接手动摆上做占位）----
        // 咖啡系：高 brewValue、低 attack —— 白天流派
        Register(new ItemDef("latte", "拿铁", E_ItemCategory.Drink, sellPrice: 18, brewValue: 5, attack: 2));
        Register(new ItemDef("americano", "美式", E_ItemCategory.Drink, sellPrice: 12, brewValue: 4, attack: 1));
        // 酒系：低 brewValue、高 attack —— 下午战斗流派
        Register(new ItemDef("iced_liquor", "冰镇烈酒", E_ItemCategory.Drink, sellPrice: 25, brewValue: 2, attack: 7));
        Register(new ItemDef("cocktail", "鸡尾酒", E_ItemCategory.Drink, sellPrice: 30, brewValue: 3, attack: 6));
    }

    /// <summary>注册一个物品定义；重复 id 会覆盖并给出警告</summary>
    public void Register(ItemDef def)
    {
        if (def == null || string.IsNullOrEmpty(def.id))
        {
            UnityEngine.Debug.LogError("[ItemDatabase] 尝试注册空物品或空 id");
            return;
        }
        if (dict.ContainsKey(def.id))
            UnityEngine.Debug.LogWarning($"[ItemDatabase] 物品 id 重复，将覆盖：{def.id}");
        dict[def.id] = def;
    }

    /// <summary>按 id 取物品定义，取不到返回 null（调用方需自行判空）</summary>
    public ItemDef Get(string id)
    {
        if (id != null && dict.TryGetValue(id, out ItemDef def))
            return def;
        return null;
    }

    /// <summary>是否存在某个 id</summary>
    public bool Has(string id) => id != null && dict.ContainsKey(id);

    /// <summary>遍历所有已注册的物品定义（供 UI palette、图鉴等使用，顺序不保证）</summary>
    public IEnumerable<ItemDef> All() => dict.Values;
}
