using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// 命令行构建入口（T6 spike）。
    ///
    /// ==== 用途 ====
    /// 验证"能不能出包"，是 M1 计划第 5 节要求的构建能力，也是简历里
    /// "命令行打包 / CI" 这条的证据。菜单：早咖晚酒/构建/Windows 64 位。
    ///
    /// 命令行用法（-batchmode 时不要开 Unity 编辑器）：
    ///   Unity.exe -quit -batchmode -projectPath &lt;工程路径&gt;
    ///             -executeMethod Game.EditorTools.BuildCommand.BuildWindows
    ///             -logFile build.log
    ///
    /// 注意：本文件必须放在 Assets/Editor/ 下（编辑器专用程序集），否则会被打进包体。
    /// </summary>
    public static class BuildCommand
    {
        private const string OutputDir = "Builds/Windows";
        private const string OutputExe = "DawnBrewDuskSpirit.exe";

        [MenuItem("早咖晚酒/构建/Windows 64 位")]
        public static void BuildWindowsMenu()
        {
            BuildWindows();
        }

        /// <summary>供 -executeMethod 调用的静态方法（无参）。</summary>
        public static void BuildWindows()
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                Debug.LogError("[BuildCommand] 没有启用的场景（EditorBuildSettings.scenes 为空），构建中止。");
                return;
            }

            Directory.CreateDirectory(OutputDir);
            string outputPath = Path.Combine(OutputDir, OutputExe);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
                Debug.Log($"[BuildCommand] 构建成功：{outputPath}（{summary.totalSize} bytes）");
            else
                Debug.LogError($"[BuildCommand] 构建失败：{summary.result}");
        }
    }
}
