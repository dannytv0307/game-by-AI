using System.IO;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DevilBlade.EditorTools
{
    /// <summary>
    /// Chụp ảnh màn 1 ở nhiều vị trí (không cần Play) để duyệt bố cục.
    /// Headless: -executeMethod DevilBlade.EditorTools.LevelScreenshots.CaptureHeadless  → Logs/Screens/*.png (Temp/ bị xóa khi Editor thoát)
    /// </summary>
    public static class LevelScreenshots
    {
        static readonly float[] Xs = { 6, 22, 42, 66, 88, 112, 124 };

        [MenuItem("DevilBlade/Capture Level 1 Screenshots")]
        public static void Capture()
        {
            EditorSceneManager.OpenScene(Level1Builder.ScenePath);
            var cam = Camera.main;
            var brain = cam.GetComponent<CinemachineBrain>();
            if (brain) brain.enabled = false;
            var ppc = cam.GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();
            if (ppc) ppc.enabled = false;
            cam.orthographicSize = 5.625f;

            var dir = Path.Combine("Logs", "Screens");
            Directory.CreateDirectory(dir);
            var rt = new RenderTexture(1280, 720, 24);
            cam.targetTexture = rt;
            foreach (var x in Xs)
            {
                cam.transform.position = new Vector3(x, 5.5f, -10);
                foreach (var p in Object.FindObjectsByType<Parallax>(FindObjectsSortMode.None))
                    SimulateParallax(p, cam.transform.position);
                cam.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();
                File.WriteAllBytes(Path.Combine(dir, $"level1_x{x:000}.png"), tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
            }
            RenderTexture.active = null;
            cam.targetTexture = null;
            Object.DestroyImmediate(rt);
            Debug.Log($"[LevelScreenshots] Saved to {Path.GetFullPath(dir)}");
        }

        static void SimulateParallax(Parallax p, Vector3 cam)
        {
            var x = cam.x * p.factorX;
            var offset = Mathf.Round((cam.x - x) / p.width) * p.width;
            p.transform.position = new Vector3(x + offset, p.baseY + (cam.y - p.baseY) * p.factorY, 0);
        }

        public static void CaptureHeadless()
        {
            try
            {
                Capture();
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
