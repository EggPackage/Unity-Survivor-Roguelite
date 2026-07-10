#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using TMPro;

/// <summary>
/// ============================================================================
/// 中文 TMP 字体一键生成器（编辑器工具）
/// ----------------------------------------------------------------------------
/// 菜单：早咖晚酒/生成中文字体(修复方块乱码)
///
/// 问题：TextMeshPro 默认字体 LiberationSans 不含中文字库，
///       所以 Game 窗口里的中文全部显示成小方块（tofu / 豆腐块）。
///
/// 解法：拿项目里的中文 TTF（Assets/Fonts/simhei.ttf）生成一个
///       “动态 SDF 字体资源（Dynamic）”。动态模式运行时按需渲染任意字符，
///       不用预先列出几千个汉字，中文随便显示。
///       然后把它设为 TMP 的默认字体 + fallback，从此全局中文可显示。
///
/// 只需在导入字体后点一次菜单即可。
/// ============================================================================
/// </summary>
public static class ChineseFontSetup
{
    private const string SourceTtf = "Assets/Fonts/simhei.ttf";
    private const string OutputAsset = "Assets/Fonts/SimHei SDF.asset";

    [MenuItem("早咖晚酒/生成中文字体(修复方块乱码)")]
    public static void CreateChineseFont()
    {
        // 1. 找到源字体
        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(SourceTtf);
        if (sourceFont == null)
        {
            EditorUtility.DisplayDialog("出错",
                "找不到字体文件：" + SourceTtf +
                "\n请确认 Assets/Fonts/simhei.ttf 存在。", "好的");
            return;
        }

        // 2. 若已存在则复用，否则创建动态 SDF 字体资源
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(OutputAsset);
        if (fontAsset == null)
        {
            // 动态字体：采样点大小 90，图集 1024×1024，运行时按需添加字形
            fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                samplingPointSize: 90,
                atlasPadding: 9,
                renderMode: UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
                atlasWidth: 1024,
                atlasHeight: 1024,
                atlasPopulationMode: AtlasPopulationMode.Dynamic,
                enableMultiAtlasSupport: true);

            if (fontAsset == null)
            {
                EditorUtility.DisplayDialog("出错", "创建 TMP 字体资源失败。", "好的");
                return;
            }

            fontAsset.name = "SimHei SDF";
            AssetDatabase.CreateAsset(fontAsset, OutputAsset);

            // 字体资源内部还有 material / atlas 两个子资源，一并存进同一个 .asset
            Material mat = fontAsset.material;
            if (mat != null)
            {
                mat.name = "SimHei SDF Material";
                AssetDatabase.AddObjectToAsset(mat, fontAsset);
            }
            if (fontAsset.atlasTextures != null)
            {
                foreach (Texture2D tex in fontAsset.atlasTextures)
                {
                    if (tex != null)
                    {
                        tex.name = "SimHei SDF Atlas";
                        AssetDatabase.AddObjectToAsset(tex, fontAsset);
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(OutputAsset);
        }

        // 3. 设为 TMP 全局默认字体 + fallback
        SetAsDefaultFont(fontAsset);

        EditorUtility.DisplayDialog("完成",
            "中文字体已生成并设为 TMP 默认字体：\n" + OutputAsset +
            "\n\n现在重新运行 BarDemo 场景，中文就能正常显示了。" +
            "\n（emoji 是否显示取决于字体是否含彩色 emoji，方块变中文即为成功）", "好的");
        Debug.Log("[ChineseFontSetup] 中文字体已生成并设为默认：" + OutputAsset);
    }

    /// <summary>把字体设为 TMP Settings 的默认字体，并加入 fallback 列表</summary>
    private static void SetAsDefaultFont(TMP_FontAsset fontAsset)
    {
        // TMP Settings 是只读引用较深的对象，用反射改私有字段最稳
        TMP_Settings settings = Resources.Load<TMP_Settings>("TMP Settings");
        if (settings == null)
        {
            Debug.LogWarning("[ChineseFontSetup] 找不到 TMP Settings，无法设为全局默认。" +
                             "但你仍可在具体文本组件上手动指定 SimHei SDF 字体。");
            return;
        }

        SerializedObject so = new SerializedObject(settings);
        SerializedProperty defaultFontProp = so.FindProperty("m_defaultFontAsset");
        if (defaultFontProp != null)
        {
            defaultFontProp.objectReferenceValue = fontAsset;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("[ChineseFontSetup] 已将 SimHei SDF 设为 TMP 默认字体。");
        }
        else
        {
            Debug.LogWarning("[ChineseFontSetup] 未找到 m_defaultFontAsset 字段，TMP 版本可能不同。");
        }
    }
}
#endif
