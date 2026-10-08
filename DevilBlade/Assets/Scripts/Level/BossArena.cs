using UnityEngine;

namespace DevilBlade
{
    /// <summary>Bước vào đấu trường: đóng cổng sau lưng, đổi nhạc boss, boss lên tiếng và bắt đầu đánh.</summary>
    [RequireComponent(typeof(Collider2D))]
    public class BossArena : MonoBehaviour
    {
        public DemonKnightBoss boss;
        public GameObject gate;
        bool _started;

        void Start()
        {
            if (gate != null) gate.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_started || other.GetComponentInParent<PlayerController>() == null) return;
            Begin();
        }

        public void Begin()
        {
            if (_started) return;
            _started = true;
            if (gate != null) gate.SetActive(true);
            if (GameManager.Instance != null) GameManager.Instance.OnBossFightStarted(boss);
            boss.Activate();
        }
    }
}
