using UnityEngine;
using UnityEngine.UI;

namespace DevilBlade
{
    /// <summary>Thanh máu, thanh nộ khí, máu boss, thông báo giữa màn hình, màn tạm dừng và màn chiến thắng.</summary>
    public class HUD : MonoBehaviour
    {
        public PlayerController player;
        public Image healthFill;
        public Image rageFill;
        public Text rageLabel;
        public GameObject bossPanel;
        public Image bossFill;
        public Text message;
        public GameObject pausePanel;
        public GameObject victoryPanel;
        public Text victoryStats;

        public Color rageColor = new(0.88f, 0.4f, 0.12f);
        public Color rageFullColor = new(1f, 0.83f, 0.42f);
        public Color demonColor = new(0.62f, 0.1f, 0.16f);

        DemonKnightBoss _boss;
        float _messageUntil;
        float _shownHealth = 1f;

        void Start()
        {
            bossPanel.SetActive(false);
            pausePanel.SetActive(false);
            victoryPanel.SetActive(false);
            message.text = "";
        }

        void Update()
        {
            if (player != null)
            {
                _shownHealth = Mathf.MoveTowards(_shownHealth, player.Health.Normalized, Time.unscaledDeltaTime * 1.5f);
                healthFill.fillAmount = _shownHealth;
                rageFill.fillAmount = player.Rage / player.rageMax;
                if (player.IsDemon)
                {
                    rageFill.color = demonColor;
                    rageLabel.text = "HÓA QUỶ";
                }
                else if (player.RageFull)
                {
                    rageFill.color = Mathf.Repeat(Time.unscaledTime * 3f, 1f) < 0.5f ? rageFullColor : rageColor;
                    rageLabel.text = "NỘ KHÍ [K]";
                }
                else
                {
                    rageFill.color = rageColor;
                    rageLabel.text = "NỘ KHÍ";
                }
            }
            if (_boss != null) bossFill.fillAmount = Mathf.MoveTowards(bossFill.fillAmount, _boss.GetComponent<Health>().Normalized, Time.unscaledDeltaTime);
            if (_messageUntil > 0 && Time.unscaledTime > _messageUntil)
            {
                message.text = "";
                _messageUntil = 0;
            }
        }

        public void ShowMessage(string text, float seconds)
        {
            message.text = text;
            _messageUntil = Time.unscaledTime + seconds;
        }

        public void ShowBoss(DemonKnightBoss boss)
        {
            _boss = boss;
            bossPanel.SetActive(boss != null);
            if (boss != null) bossFill.fillAmount = 1f;
        }

        public void SetPaused(bool paused) => pausePanel.SetActive(paused);

        public void ShowVictory(float seconds, int deaths)
        {
            victoryPanel.SetActive(true);
            var m = (int)(seconds / 60);
            var s = (int)(seconds % 60);
            victoryStats.text = $"Thời gian: {m:00}:{s:00}    Số lần ngã: {deaths}\n\nNhấn Enter để chơi lại";
        }
    }
}
