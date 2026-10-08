using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DevilBlade
{
    /// <summary>Điều phối màn chơi: checkpoint, hồi sinh, đấu boss, thắng màn, tạm dừng / chơi lại.</summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public PlayerController player;
        public HUD hud;
        public float respawnDelay = 1.6f;

        public enum Phase { Playing, Dead, BossFight, Won }
        public Phase State { get; private set; } = Phase.Playing;
        public int Deaths { get; private set; }
        public float ElapsedTime { get; private set; }

        Vector3 _checkpoint;
        DemonKnightBoss _boss;
        bool _paused;

        void Awake()
        {
            Instance = this;
            Physics2D.IgnoreLayerCollision(Layers.Player, Layers.Enemy, true);
            Physics2D.IgnoreLayerCollision(Layers.Enemy, Layers.Enemy, true);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Time.timeScale = 1f;
        }

        void Start()
        {
            _checkpoint = player.transform.position;
            player.Health.Died += OnPlayerDied;
            player.RageBecameFull += () => hud?.ShowMessage("NỘ KHÍ ĐẦY — nhấn K để HÓA QUỶ", 3f);
            var audio = AudioManager.Instance;
            if (audio != null)
            {
                audio.PlayMusic(audio.musicLevel, 0.1f);
                audio.Voice(audio.voiceIntro);
            }
            hud?.ShowMessage("A/D di chuyển · Space nhảy · J chém · K hóa quỷ", 5f);
        }

        void Update()
        {
            if (State != Phase.Won) ElapsedTime += Time.deltaTime;
            var kb = Keyboard.current;
            if (kb == null) return;
            if (State == Phase.Won && (kb.enterKey.wasPressedThisFrame || kb.rKey.wasPressedThisFrame)) Restart();
            if (kb.escapeKey.wasPressedThisFrame && State != Phase.Won) TogglePause();
            if (_paused && kb.rKey.wasPressedThisFrame) Restart();
        }

        void TogglePause()
        {
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;
            hud?.SetPaused(_paused);
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SetCheckpoint(Vector3 position)
        {
            _checkpoint = position;
            hud?.ShowMessage("Đã lưu điểm hồi sinh", 2f);
        }

        void OnPlayerDied()
        {
            if (State == Phase.Won) return;
            Deaths++;
            StartCoroutine(RespawnRoutine());
        }

        IEnumerator RespawnRoutine()
        {
            var wasBoss = State == Phase.BossFight;
            State = Phase.Dead;
            hud?.ShowMessage("NGÃ XUỐNG...", respawnDelay);
            yield return new WaitForSeconds(respawnDelay);
            player.Respawn(_checkpoint);
            State = wasBoss ? Phase.BossFight : Phase.Playing;
        }

        public void OnBossFightStarted(DemonKnightBoss boss)
        {
            _boss = boss;
            State = Phase.BossFight;
            // hồi sinh ngay trước cửa đấu trường
            _checkpoint = player.transform.position;
            boss.Defeated += OnBossDefeated;
            hud?.ShowBoss(boss);
            var audio = AudioManager.Instance;
            if (audio != null)
            {
                audio.PlayMusic(audio.musicBoss, 0.8f);
                audio.Voice(audio.voiceBossTaunt);
            }
        }

        void OnBossDefeated()
        {
            State = Phase.Won;
            player.ControlsEnabled = false;
            hud?.ShowBoss(null);
            var audio = AudioManager.Instance;
            if (audio != null) audio.PlayMusic(audio.musicLevel, 2f);
            hud?.ShowVictory(ElapsedTime, Deaths);
        }
    }
}
