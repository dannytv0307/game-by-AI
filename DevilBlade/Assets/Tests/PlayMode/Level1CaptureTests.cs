using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace DevilBlade.Tests
{
    /// <summary>
    /// Chụp ảnh trong lúc chơi (có HUD) để duyệt hình ảnh khi chạy headless → Logs/Screens/play_*.png.
    /// Chạy riêng: -runTests -testPlatform PlayMode -testCategory Capture
    /// </summary>
    [Category("Capture")]
    public class Level1CaptureTests
    {
        static IEnumerator Wait(float s)
        {
            for (float t = 0; t < s; t += Time.deltaTime) yield return null;
        }

        static void Shot(string name)
        {
            var cam = Camera.main;
            var canvas = Object.FindAnyObjectByType<Canvas>();
            var prevMode = canvas.renderMode;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 1f;
            var rt = new RenderTexture(1280, 720, 24);
            cam.targetTexture = rt;
            Canvas.ForceUpdateCanvases();
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            tex.Apply();
            Directory.CreateDirectory("Logs/Screens");
            File.WriteAllBytes($"Logs/Screens/play_{name}.png", tex.EncodeToPNG());
            RenderTexture.active = null;
            cam.targetTexture = null;
            canvas.renderMode = prevMode;
            Object.Destroy(rt);
            Object.Destroy(tex);
        }

        [UnityTest]
        public IEnumerator Capture_Key_Moments()
        {
            SceneManager.LoadScene("Level1");
            yield return Wait(1.2f);
            var player = Object.FindAnyObjectByType<PlayerController>();
            Shot("01_start");

            player.SimulateMove(1f);
            yield return Wait(1.1f);
            player.SimulateMove(0f);
            player.SimulateAttack();
            yield return Wait(0.15f);
            Shot("02_attack");

            player.AddRage(player.rageMax);
            yield return Wait(0.3f);
            player.SimulateTransform();
            yield return Wait(0.35f);
            Shot("03_transforming");
            yield return Wait(0.6f);
            player.SimulateAttack();
            yield return Wait(0.15f);
            Shot("04_demon_attack");

            player.Health.Invulnerable = true;
            player.transform.position = new Vector3(99f, 4.2f, 0);
            player.SimulateMove(1f);
            yield return Wait(1.6f);
            player.SimulateMove(0f);
            yield return Wait(1.8f);
            Shot("05_boss");

            var boss = Object.FindAnyObjectByType<DemonKnightBoss>().GetComponent<Health>();
            while (!boss.IsDead) { boss.TakeDamage(50, 1f); yield return Wait(0.4f); }
            yield return Wait(1f);
            Shot("06_victory");
            Assert.Pass();
        }
    }
}
