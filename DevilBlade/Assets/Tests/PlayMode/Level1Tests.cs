using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace DevilBlade.Tests
{
    /// <summary>Kiểm thử khói cho màn 1: vật lý, di chuyển, chém, hóa quỷ, hồi sinh, đấu boss.</summary>
    public class Level1Tests
    {
        PlayerController _player;
        GameManager _gm;

        [UnitySetUp]
        public IEnumerator LoadLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Level1");
            yield return null;
            yield return null;
            _player = Object.FindAnyObjectByType<PlayerController>();
            _gm = GameManager.Instance;
            Assert.IsNotNull(_player, "Thiếu Player trong scene");
            Assert.IsNotNull(_gm, "Thiếu GameManager trong scene");
        }

        static IEnumerator Wait(float seconds)
        {
            for (float t = 0; t < seconds; t += Time.deltaTime) yield return null;
        }

        void Teleport(Vector2 pos)
        {
            _player.transform.position = pos;
            _player.GetComponent<Rigidbody2D>().position = pos;
            _player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        [UnityTest]
        public IEnumerator Player_LandsOnGround()
        {
            yield return Wait(1.5f);
            Assert.IsTrue(_player.Grounded, "Người chơi phải đứng trên mặt đất");
            Assert.AreEqual(2f, _player.transform.position.y, 0.15f);
        }

        [UnityTest]
        public IEnumerator Player_RunsRight_And_Jumps()
        {
            yield return Wait(0.8f);
            var x0 = _player.transform.position.x;
            _player.SimulateMove(1f);
            yield return Wait(0.6f);
            Assert.Greater(_player.transform.position.x, x0 + 2.5f, "Phải chạy sang phải");
            var y0 = _player.transform.position.y;
            _player.SimulateJump();
            yield return Wait(0.25f);
            Assert.Greater(_player.transform.position.y, y0 + 1.5f, "Phải nhảy lên");
            _player.SimulateMove(0f);
        }

        [UnityTest]
        public IEnumerator Player_KillsImp_And_GainsRage()
        {
            var imp = Object.FindObjectsByType<ImpEnemy>(FindObjectsSortMode.None)
                .Where(i => i.gameObject.activeInHierarchy).OrderBy(i => i.transform.position.x).First();
            yield return Wait(0.5f);
            Teleport(new Vector2(imp.transform.position.x - 1.2f, 2.2f));
            _player.SimulateMove(1f);
            yield return null;
            _player.SimulateMove(0f);
            for (float t = 0; t < 6f && !imp.IsDead; t += 0.2f)
            {
                // giữ khoảng cách và quay mặt về phía imp
                var dx = imp.transform.position.x - _player.transform.position.x;
                _player.SimulateMove(Mathf.Abs(dx) > 1.2f ? Mathf.Sign(dx) : 0f);
                if (Mathf.Abs(dx) <= 1.6f) _player.SimulateAttack();
                yield return Wait(0.2f);
            }
            _player.SimulateMove(0f);
            Assert.IsTrue(imp.IsDead, "Imp phải chết sau vài nhát chém");
            Assert.Greater(_player.Rage, 0f, "Chém trúng phải tăng nộ khí");
        }

        [UnityTest]
        public IEnumerator Rage_Transform_To_Demon_And_Back()
        {
            yield return Wait(1f);
            _player.demonDuration = 1.5f;
            _player.AddRage(_player.rageMax);
            Assert.IsTrue(_player.RageFull);
            _player.SimulateTransform();
            yield return null;
            Assert.IsTrue(_player.IsTransforming, "Phải vào trạng thái biến hình");
            yield return Wait(1.0f);
            Assert.IsTrue(_player.IsDemon, "Phải ở dạng quỷ sau khi biến hình");
            yield return Wait(1.8f);
            Assert.IsFalse(_player.IsDemon, "Hết nộ khí phải trở lại hình người");
            Assert.AreEqual(0f, _player.Rage, 0.01f);
        }

        [UnityTest]
        public IEnumerator FallingIntoPit_Respawns()
        {
            yield return Wait(0.5f);
            Teleport(new Vector2(32.5f, 5f));
            yield return Wait(3.5f);
            Assert.AreEqual(1, _gm.Deaths, "Rơi vực phải tính là một lần ngã");
            Assert.IsFalse(_player.Health.IsDead, "Phải hồi sinh");
            Assert.Less(_player.transform.position.x, 10f, "Hồi sinh ở điểm xuất phát (chưa qua checkpoint)");
        }

        /// <summary>Bot chạy từ đầu màn tới đấu trường: kiểm tra màn chơi đi qua được (vực, bậc, gờ cao).</summary>
        [UnityTest]
        public IEnumerator Bot_CanTraverse_Level_To_Arena()
        {
            yield return Wait(0.5f);
            _player.Health.Invulnerable = true; // chỉ kiểm tra địa hình
            var rb = _player.GetComponent<Rigidbody2D>();
            var deathAt = "";
            _player.Health.Died += () => deathAt += $" ({_player.transform.position.x:0.0},{_player.transform.position.y:0.0}) state={_player.IsTransforming}";
            var stuckTimer = 0f;
            var lastX = _player.transform.position.x;
            for (float t = 0; t < 90f && _gm.State != GameManager.Phase.BossFight; t += Time.deltaTime)
            {
                var p = (Vector2)_player.transform.position;
                _player.SimulateMove(1f);
                var wallAhead = Physics2D.Raycast(p + Vector2.up * 0.5f, Vector2.right, 1.0f, Layers.GroundMask).collider != null;
                var pitAhead = Physics2D.Raycast(p + new Vector2(1.0f, 0.3f), Vector2.down, 3f, Layers.GroundMask).collider == null;
                if (_player.Grounded && (wallAhead || pitAhead || stuckTimer > 0.4f)) _player.SimulateJump();
                if (!pitAhead && Physics2D.OverlapBox(p + new Vector2(1.2f, 0.8f), new Vector2(2f, 1.6f), 0, Layers.EnemyMask)) _player.SimulateAttack();
                stuckTimer = Mathf.Abs(p.x - lastX) < 0.01f ? stuckTimer + Time.deltaTime : 0f;
                lastX = p.x;
                yield return null;
            }
            _player.SimulateMove(0f);
            Assert.AreEqual(GameManager.Phase.BossFight, _gm.State,
                $"Bot phải tới được đấu trường (dừng ở {(Vector2)_player.transform.position}, grounded={_player.Grounded}, v={rb.linearVelocity}, ngã {_gm.Deaths} lần{deathAt})");
            Assert.AreEqual(0, _gm.Deaths, "Bot không được rơi vực, ngã tại" + deathAt);
            _ = rb;
        }

        [UnityTest]
        public IEnumerator Checkpoint_Then_BossFight_And_Victory()
        {
            yield return Wait(0.5f);
            // chạm checkpoint trước đấu trường
            Teleport(new Vector2(94.5f, 4.3f));
            yield return Wait(0.5f);
            // đi bộ vào đấu trường (nhảy xuống từ gờ)
            _player.SimulateMove(1f);
            yield return Wait(1.5f);
            _player.SimulateMove(0f);
            Assert.AreEqual(GameManager.Phase.BossFight, _gm.State, "Bước vào đấu trường phải bắt đầu đánh boss");
            Assert.IsTrue(GameObject.Find("ArenaGate").activeInHierarchy, "Cổng đấu trường phải đóng");

            var boss = Object.FindAnyObjectByType<DemonKnightBoss>();
            Assert.IsTrue(boss.Active);
            var bossHealth = boss.GetComponent<Health>();
            _player.Health.Invulnerable = true; // test chỉ kiểm tra luồng thắng
            while (!bossHealth.IsDead)
            {
                bossHealth.TakeDamage(40, 1f);
                yield return Wait(0.5f);
            }
            yield return Wait(0.5f);
            Assert.AreEqual(GameManager.Phase.Won, _gm.State, "Hạ boss phải thắng màn");
            Assert.Greater(Object.FindObjectsByType<ImpEnemy>(FindObjectsSortMode.None).Count(i => i.name.StartsWith("Imp_SummonTemplate(Clone)")), 0,
                "Giai đoạn 2 boss phải triệu hồi imp");
        }
    }
}
