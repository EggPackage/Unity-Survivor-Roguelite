#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ============================================================================
/// 吧台 Demo 场景一键生成器（编辑器工具）
/// ----------------------------------------------------------------------------
/// 菜单：早咖晚酒/生成吧台Demo场景
///
/// 为什么要用脚本生成、而不是手拖 / MCP 实时建：
///   之前用 MCP 实时搭 UI 时，出现"层级/位置混乱"——子元素父节点挂错、
///   用世界坐标而非 anchoredPosition、锚点用默认值、缺布局组件。
///   本脚本自顶向下、每一步显式指定父节点，容器一律用"锚点拉伸 + LayoutGroup"，
///   位置全部交给布局系统自动算，从而可重复生成且层级绝不会乱。
///
/// 生成的层级结构：
///   Canvas (Screen Space-Overlay, CanvasScaler, GraphicRaycaster)
///     └─ Root (铺满, VerticalLayoutGroup)
///          ├─ StatsBar     顶部数值栏（TMP 文本）
///          ├─ SelectionBar 当前选中提示（TMP 文本）
///          ├─ GridArea     网格区（GridLayoutGroup，格子运行时填充）
///          ├─ ComboList    combo 结算列表（TMP 文本）
///          └─ Palette      物品选择栏（HorizontalLayoutGroup，按钮运行时填充）
///   EventSystem
///   Main Camera
/// BarDemoUI 挂在 Root 上，各容器引用在生成时连好。
/// ============================================================================
/// </summary>
public static class BarDemoSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/BarDemo.unity";

    [MenuItem("早咖晚酒/生成吧台Demo场景")]
    public static void BuildScene()
    {
        // 新建一个空场景（会提示保存当前场景）
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. 摄像机
        GameObject camGo = new GameObject("Main Camera");
        Camera cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.10f, 0.10f, 0.12f, 1f);
        cam.orthographic = true;
        camGo.tag = "MainCamera";

        // 2. EventSystem（UI 点击必需）
        GameObject esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
        esGo.AddComponent<StandaloneInputModule>();

        // 3. Canvas
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // 4. Root：铺满 Canvas，纵向排列各功能区
        GameObject root = CreateUIChild("Root", canvasGo.transform);
        StretchFull(root.GetComponent<RectTransform>());
        VerticalLayoutGroup rootVlg = root.AddComponent<VerticalLayoutGroup>();
        rootVlg.padding = new RectOffset(40, 40, 40, 40);
        rootVlg.spacing = 20;
        rootVlg.childAlignment = TextAnchor.UpperCenter;
        rootVlg.childControlWidth = true;
        rootVlg.childControlHeight = true;
        rootVlg.childForceExpandWidth = true;
        rootVlg.childForceExpandHeight = false;

        // 4.1 顶部数值栏
        GameObject statsBar = CreateUIChild("StatsBar", root.transform);
        AddLayoutElement(statsBar, 60);
        TMP_Text statsText = AddText(statsBar, "白天·出杯效率 0    下午·攻击 0    防御 0", 34, TextAlignmentOptions.Center);

        // 4.2 当前选中提示栏
        GameObject selBar = CreateUIChild("SelectionBar", root.transform);
        AddLayoutElement(selBar, 40);
        TMP_Text selText = AddText(selBar, "当前未选择物品", 24, TextAlignmentOptions.Center);

        // 4.3 网格区（GridLayoutGroup，格子由 BarDemoUI 运行时填充）
        GameObject gridArea = CreateUIChild("GridArea", root.transform);
        AddLayoutElement(gridArea, 560);
        GridLayoutGroup grid = gridArea.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(160, 160);
        grid.spacing = new Vector2(12, 12);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;

        // 4.4 combo 列表
        GameObject comboArea = CreateUIChild("ComboList", root.transform);
        AddLayoutElement(comboArea, 200);
        TMP_Text comboText = AddText(comboArea, "触发的 combo", 24, TextAlignmentOptions.TopLeft);

        // 4.5 palette 物品选择栏（HorizontalLayoutGroup，按钮由 BarDemoUI 运行时填充）
        GameObject palette = CreateUIChild("Palette", root.transform);
        AddLayoutElement(palette, 180);
        HorizontalLayoutGroup hlg = palette.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        // 给 palette 里的按钮一个统一宽度：用 ContentSizeFitter 不好控，改由按钮自带 LayoutElement，
        // 这里让 BarDemoUI 生成按钮时自行处理；容器只负责横向排列。

        // 5. 挂 BarDemoUI 到 Root，连引用
        BarDemoUI ui = root.AddComponent<BarDemoUI>();
        ui.gridContainer = gridArea.GetComponent<RectTransform>();
        ui.paletteContainer = palette.GetComponent<RectTransform>();
        ui.statsText = statsText;
        ui.comboListText = comboText;
        ui.selectionText = selText;
        ui.rows = 3;
        ui.cols = 3;

        // 6. 保存场景并加入 Build Settings
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildSettings(ScenePath);

        EditorUtility.DisplayDialog("完成",
            "吧台 Demo 场景已生成：\n" + ScenePath +
            "\n\n直接点 Play 运行即可：\n点下方物品 → 点格子摆放 → 看 combo 与数值。", "好的");
        Debug.Log("[BarDemoSceneBuilder] 场景已生成并保存：" + ScenePath);
    }

    // ---------------------------------------------------------------- 工具方法

    /// <summary>创建一个带 RectTransform 的 UI 子物体，显式挂到指定父节点下</summary>
    private static GameObject CreateUIChild(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false); // false = 保持局部布局，交给 LayoutGroup
        return go;
    }

    /// <summary>把 RectTransform 设为铺满父节点（锚点拉伸 + 零边距）</summary>
    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>给一个 UI 区块加固定首选高度（配合父级 VerticalLayoutGroup）</summary>
    private static void AddLayoutElement(GameObject go, float preferredHeight)
    {
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = preferredHeight;
        le.flexibleHeight = 0;
    }

    /// <summary>在指定物体上加一个铺满的 TextMeshProUGUI 文本</summary>
    private static TMP_Text AddText(GameObject parent, string content, float fontSize, TextAlignmentOptions align)
    {
        GameObject go = CreateUIChild("Text", parent.transform);
        StretchFull(go.GetComponent<RectTransform>());
        TMP_Text text = go.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = align;
        text.color = Color.white;
        text.enableWordWrapping = true;
        return text;
    }

    /// <summary>把场景加入 Build Settings（若已存在则跳过）</summary>
    private static void AddSceneToBuildSettings(string path)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (var s in scenes)
        {
            if (s.path == path)
                return;
        }
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
#endif
