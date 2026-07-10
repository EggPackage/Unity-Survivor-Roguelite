#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 初始物品配置生成器（编辑器工具，迁移用）
/// ----------------------------------------------------------------------------
/// 菜单：早咖晚酒/生成初始物品配置
///
/// 作用：把原本写死在 ItemDatabase 代码里的 11 种物品，一键生成成
///       Resources/ItemConfigs/*.asset（ScriptableObject）。
///       生成后 ItemDatabase 会自动改从这些 .asset 加载（数据驱动），
///       之后就能用「物品形状编辑器」可视化改每个物品的形状。
///
/// 只需运行一次。若资源已存在则跳过（不覆盖你已经编辑过的形状）。
/// 想重置某个物品，删掉对应 .asset 再运行即可。
///
/// 附带了一份"默认形状示范"：咖啡机 2×2、酒架 1×3 竖条，其余单格，
/// 让你打开形状编辑器就能看到多格效果，也能随时改。
/// ============================================================================
/// </summary>
public static class ItemConfigBootstrap
{
    private const string Dir = "Assets/Resources/ItemConfigs";

    /// <summary>一份物品初始数据（含默认形状）。与旧 ItemDatabase 内置数据一致。</summary>
    private struct Seed
    {
        public string id, name;
        public E_ItemCategory cat;
        public int sell, brew, atk, def;
        public List<Vector2Int> shape;

        public Seed(string id, string name, E_ItemCategory cat,
                    int sell, int brew, int atk, int def, List<Vector2Int> shape)
        {
            this.id = id; this.name = name; this.cat = cat;
            this.sell = sell; this.brew = brew; this.atk = atk; this.def = def;
            this.shape = shape;
        }
    }

    // 形状快捷构造
    private static List<Vector2Int> Single() => new List<Vector2Int> { new Vector2Int(0, 0) };
    private static List<Vector2Int> Square2x2() => new List<Vector2Int>
        { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) };
    private static List<Vector2Int> Vertical1x3() => new List<Vector2Int>
        { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
    private static List<Vector2Int> Horizontal1x2() => new List<Vector2Int>
        { new Vector2Int(0, 0), new Vector2Int(0, 1) };

    [MenuItem("早咖晚酒/生成初始物品配置")]
    public static void GenerateConfigs()
    {
        Directory.CreateDirectory(Dir);

        List<Seed> seeds = new List<Seed>
        {
            // 设备（示范多格：咖啡机 2×2、酒架 1×3 竖条）
            new Seed("coffee_machine", "咖啡机", E_ItemCategory.Equipment, 0, 1, 0, 0, Square2x2()),
            new Seed("liquor_rack",    "酒架",   E_ItemCategory.Equipment, 0, 0, 1, 0, Vertical1x3()),
            // 原料（单格）
            new Seed("milk",        "牛奶",   E_ItemCategory.Material, 0, 0, 0, 0, Single()),
            new Seed("coffee_bean", "咖啡豆", E_ItemCategory.Material, 0, 0, 0, 0, Single()),
            new Seed("ice",         "冰",     E_ItemCategory.Material, 0, 0, 0, 0, Single()),
            // 器具（杯垫单格，摇酒器 1×2 示范）
            new Seed("shaker",  "摇酒器", E_ItemCategory.Utensil, 0, 0, 0, 3, Horizontal1x2()),
            new Seed("coaster", "杯垫",   E_ItemCategory.Utensil, 0, 0, 0, 2, Single()),
            // 饮品（combo 产物，单格；不进 palette 但仍需配置以便未来直接摆）
            new Seed("latte",       "拿铁",     E_ItemCategory.Drink, 18, 5, 2, 0, Single()),
            new Seed("americano",   "美式",     E_ItemCategory.Drink, 12, 4, 1, 0, Single()),
            new Seed("iced_liquor", "冰镇烈酒", E_ItemCategory.Drink, 25, 2, 7, 0, Single()),
            new Seed("cocktail",    "鸡尾酒",   E_ItemCategory.Drink, 30, 3, 6, 0, Single()),
        };

        int created = 0, skipped = 0;
        foreach (Seed s in seeds)
        {
            string path = $"{Dir}/Item_{s.id}.asset";
            if (AssetDatabase.LoadAssetAtPath<ItemConfig>(path) != null)
            {
                skipped++;
                continue; // 已存在，不覆盖用户编辑过的形状
            }

            ItemConfig cfg = ScriptableObject.CreateInstance<ItemConfig>();
            cfg.id = s.id;
            cfg.displayName = s.name;
            cfg.category = s.cat;
            cfg.sellPrice = s.sell;
            cfg.brewValue = s.brew;
            cfg.attack = s.atk;
            cfg.defense = s.def;
            cfg.shapeCells = s.shape;

            AssetDatabase.CreateAsset(cfg, path);
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成",
            $"物品配置生成完毕：\n新建 {created} 个，跳过 {skipped} 个（已存在）。\n\n" +
            $"位置：{Dir}\n\n" +
            "现在可用菜单「早咖晚酒/物品形状编辑器」可视化编辑每个物品的形状。", "好的");
        Debug.Log($"[ItemConfigBootstrap] 生成完成：新建 {created}，跳过 {skipped}。");
    }
}
#endif
