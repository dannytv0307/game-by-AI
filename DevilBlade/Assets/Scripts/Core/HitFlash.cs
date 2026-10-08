using System.Collections;
using UnityEngine;

namespace DevilBlade
{
    /// <summary>Nháy màu khi trúng đòn và tạm dừng khung hình ngắn (hit-stop) cho cảm giác va chạm.</summary>
    public class HitFlash : MonoBehaviour
    {
        public Color flashColor = new(1f, 0.35f, 0.35f);
        SpriteRenderer _sr;
        Color _base = Color.white;
        Coroutine _co;

        void Awake() => _sr = GetComponentInChildren<SpriteRenderer>();

        public void SetBaseColor(Color c)
        {
            _base = c;
            if (_co == null) _sr.color = c;
        }

        public void Flash(float duration = 0.12f)
        {
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Run(duration));
        }

        IEnumerator Run(float duration)
        {
            _sr.color = flashColor;
            yield return new WaitForSeconds(duration);
            _sr.color = _base;
            _co = null;
        }

        static float _stopUntil;

        public static void HitStop(MonoBehaviour host, float seconds = 0.05f)
        {
            if (Time.unscaledTime < _stopUntil) return;
            _stopUntil = Time.unscaledTime + seconds;
            host.StartCoroutine(HitStopRoutine(seconds));
        }

        static IEnumerator HitStopRoutine(float seconds)
        {
            var prev = Time.timeScale;
            if (prev == 0f) yield break;
            Time.timeScale = 0.05f;
            yield return new WaitForSecondsRealtime(seconds);
            if (Time.timeScale == 0.05f) Time.timeScale = prev;
        }
    }
}
