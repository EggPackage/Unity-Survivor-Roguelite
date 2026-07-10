#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 物品形状编辑器（EditorWindow）
/// ----------------------------------------------------------------------------
/// 菜单：早咖晚酒/物品形状编辑器
///
/// 用途：可视化编辑每个物品在吧台上占用的格子形状。
///   左侧：从 Resources/ItemConfigs 读出的所有物品列表，点选切换。
///   右侧：一个 N×N 点阵，点格子切换"占用/空"，实时预览；显示占用格数。
///   底部：保存 → 把点亮的格子归一化成相对偏移，写回 ItemConfig.shapeCells。
///
/// 归一化：以点亮格子的最小行、最小列为原点 (0,0)，保证形状与摆放位置无关。
/// 保存后运行游戏，ItemDatabase 会加载新形状，摆放时即按此形状占格。
/// ============================================================================
/// </summary>
public class ItemShapeEditorWindow : EditorWindow
{
    private const int GridSize = 5;                 // 编辑用点阵大小 5×5
    private const float CellPixel = 44f;            // 每格像素

    private List<ItemConfig> configs = new List<ItemConfig>();
    private int selectedIndex = -1;
    private bool[,] painted = new bool[GridSize, GridSize]; // 当前涂格状态
    private Vector2 listScroll;

    [MenuItem("早咖晚酒/物品形状编辑器")]
    public static void Open()
    {
        ItemShapeEditorWindow win = GetWindow<ItemShapeEditorWindow>("物品形状编辑器");
        win.minSize = new Vector2(560, 420);
        win.ReloadConfigs();
        win.Show();
    }

    private void OnEnable()
    {
        ReloadConfigs();
    }

    /// <summary>从 Resources/ItemConfigs 重新加载所有物品配置</summary>
    private void ReloadConfigs()
    {
        configs.Clear();
        // 编辑器下用 AssetDatabase 查找，比 Resources.LoadAll 更可靠（能拿到未打包资源）
        string[] guids = AssetDatabase.FindAssets("t:ItemConfig");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemConfig cfg = AssetDatabase.LoadAssetAtPath<ItemConfig>(path);
            if (cfg != null)
                configs.Add(cfg);
        }
        configs.Sort((a, b) => string.Compare(a.id, b.id, System.StringComparison.Ordinal));

        if (configs.Count > 0)
            SelectItem(0);
        else
            selectedIndex = -1;
    }

    /// <summary>选中某物品，把它的形状读进 painted 点阵</summary>
    private void SelectItem(int index)
    {
        selectedIndex = index;
        ClearPainted();
        if (index < 0 || index >= configs.Count)
            return;

        ItemConfig cfg = configs[index];
        foreach (Vector2Int off in cfg.GetShapeOrSingle())
        {
            // 形状偏移可能为负或超界，编辑时先归一到点阵内展示（保存时会重新归一化）
            if (off.x >= 0 && off.x < GridSize && off.y >= 0 && off.y < GridSize)
                painted[off.x, off.y] = true;
        }
        // 若形状为空，至少点亮 (0,0)
        if (cfg.GetShapeOrSingle().Count == 0)
            painted[0, 0] = true;
    }

    private void ClearPainted()
    {
        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                painted[r, c] = false;
    }

    private void OnGUI()
    {
        if (configs.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "没找到任何物品配置（Resources/ItemConfigs 下的 ItemConfig 资源）。\n" +
                "请先点菜单「早咖晚酒/生成初始物品配置」生成。", MessageType.Warning);
            if (GUILayout.Button("刷新"))
                ReloadConfigs();
            return;
        }

        EditorGUILayout.BeginHorizontal();

        // ---- 左侧物品列表 ----
        EditorGUILayout.BeginVertical(GUILayout.Width(180));
        EditorGUILayout.LabelField("物品列表", EditorStyles.boldLabel);
        listScroll = EditorGUILayout.BeginScrollView(listScroll);
        for (int i = 0; i < configs.Count; i++)
        {
            ItemConfig cfg = configs[i];
            bool isSel = (i == selectedIndex);
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = isSel ? FontStyle.Bold : FontStyle.Normal
            };
            Color prev = GUI.backgroundColor;
            if (isSel) GUI.backgroundColor = new Color(0.5f, 0.8f, 1f);
            if (GUILayout.Button($"{cfg.displayName} ({cfg.id})", style))
                SelectItem(i);
            GUI.backgroundColor = prev;
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("刷新列表"))
            ReloadConfigs();
        EditorGUILayout.EndVertical();

        // ---- 右侧形状编辑区 ----
        EditorGUILayout.BeginVertical();
        if (selectedIndex >= 0 && selectedIndex < configs.Count)
            DrawShapeEditor(configs[selectedIndex]);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawShapeEditor(ItemConfig cfg)
    {
        EditorGUILayout.LabelField($"编辑形状：{cfg.displayName}", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("点击格子切换 占用/空。以点亮区域的左上为原点，保存时自动归一化。", MessageType.Info);

        // 画 N×N 点阵
        Rect gridRect = GUILayoutUtility.GetRect(GridSize * CellPixel, GridSize * CellPixel,
            GUILayout.Width(GridSize * CellPixel), GUILayout.Height(GridSize * CellPixel));

        for (int r = 0; r < GridSize; r++)
        {
            for (int c = 0; c < GridSize; c++)
            {
                Rect cell = new Rect(
                    gridRect.x + c * CellPixel,
                    gridRect.y + r * CellPixel,
                    CellPixel - 2, CellPixel - 2);

                Color col = painted[r, c] ? new Color(0.85f, 0.62f, 0.25f) : new Color(0.25f, 0.25f, 0.28f);
                EditorGUI.DrawRect(cell, col);

                // 点击切换
                if (Event.current.type == EventType.MouseDown && cell.Contains(Event.current.mousePosition))
                {
                    painted[r, c] = !painted[r, c];
                    Event.current.Use();
                    Repaint();
                }
            }
        }

        GUILayout.Space(8);
        int count = CountPainted();
        EditorGUILayout.LabelField($"当前占用格数：{count}");

        GUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();

        using (new EditorGUI.DisabledScope(count == 0))
        {
            if (GUILayout.Button("保存形状", GUILayout.Height(30)))
                SaveShape(cfg);
        }
        if (GUILayout.Button("清空", GUILayout.Height(30), GUILayout.Width(80)))
        {
            ClearPainted();
            Repaint();
        }
        if (GUILayout.Button("设为单格", GUILayout.Height(30), GUILayout.Width(90)))
        {
            ClearPainted();
            painted[0, 0] = true;
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
    }

    private int CountPainted()
    {
        int n = 0;
        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                if (painted[r, c]) n++;
        return n;
    }

    /// <summary>把点阵归一化成相对偏移列表，写回 ItemConfig 并保存资源</summary>
    private void SaveShape(ItemConfig cfg)
    {
        // 找点亮区域的最小行列做原点
        int minR = int.MaxValue, minC = int.MaxValue;
        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                if (painted[r, c])
                {
                    if (r < minR) minR = r;
                    if (c < minC) minC = c;
                }

        if (minR == int.MaxValue)
        {
            EditorUtility.DisplayDialog("提示", "至少要点亮一个格子。", "好的");
            return;
        }

        List<Vector2Int> shape = new List<Vector2Int>();
        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                if (painted[r, c])
                    shape.Add(new Vector2Int(r - minR, c - minC));

        Undo.RecordObject(cfg, "编辑物品形状");
        cfg.shapeCells = shape;
        EditorUtility.SetDirty(cfg);
        AssetDatabase.SaveAssets();

        Debug.Log($"[ItemShapeEditor] 已保存「{cfg.displayName}」形状：{shape.Count} 格。");
        ShowNotification(new GUIContent($"已保存 {cfg.displayName}（{shape.Count} 格）"));
    }
}
#endif
