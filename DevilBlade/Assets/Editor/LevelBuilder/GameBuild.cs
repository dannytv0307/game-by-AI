using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DevilBlade.EditorTools
{
    /// <summary>Build bản Windows chơi được → Builds/Windows/DevilBlade.exe</summary>
    public static class GameBuild
    {
        public const string WindowsExe = "Builds/Windows/DevilBlade.exe";

        [MenuItem("DevilBlade/Build Windows Player")]
        public static void BuildWindows()
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { Level1Builder.ScenePathHD, Level1Builder.ScenePath }, // Tab trong game để đổi Pixel/HD
                locationPathName = WindowsExe,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            });
            var s = report.summary;
            Debug.Log($"[GameBuild] {s.result} — {s.totalSize / (1024 * 1024)} MB, {s.totalErrors} lỗi, {s.totalTime.TotalSeconds:0}s → {WindowsExe}");
            if (s.result != BuildResult.Succeeded) throw new UnityEditor.Build.BuildFailedException($"Build thất bại: {s.result}");
        }

        /// <summary>Headless: dựng lại màn 1 rồi build. -executeMethod DevilBlade.EditorTools.GameBuild.RebuildAndBuildHeadless</summary>
        public static void RebuildAndBuildHeadless()
        {
            try
            {
                Level1Builder.Build();
                BuildWindows();
                EditorApplication.Exit(0);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }
    }
}
