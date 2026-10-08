using System;
using UnityEngine;

namespace DevilBlade
{
    /// <summary>Máu dùng chung cho người chơi và quái. Có thời gian bất tử sau khi trúng đòn.</summary>
    public class Health : MonoBehaviour
    {
        public int max = 100;
        public float invulnerableAfterHit = 0f;

        public int Current { get; private set; }
        public bool IsDead => Current <= 0;
        public float Normalized => (float)Current / max;
        public bool Invulnerable { get; set; }

        /// <summary>(lượng sát thương, hướng đánh tới: -1 trái / +1 phải)</summary>
        public event Action<int, float> Damaged;
        public event Action Died;

        float _invulnerableUntil;

        void Awake() => Current = max;

        public bool TakeDamage(int amount, float fromDirection)
        {
            if (IsDead || Invulnerable || Time.time < _invulnerableUntil) return false;
            Current = Mathf.Max(0, Current - amount);
            _invulnerableUntil = Time.time + invulnerableAfterHit;
            Damaged?.Invoke(amount, fromDirection);
            if (Current == 0) Died?.Invoke();
            return true;
        }

        /// <summary>Chết ngay, bỏ qua bất tử (rơi xuống vực).</summary>
        public void Kill()
        {
            if (IsDead) return;
            Current = 0;
            Died?.Invoke();
        }

        public void Heal(int amount) => Current = Mathf.Min(max, Current + amount);

        public void ResetFull()
        {
            Current = max;
            _invulnerableUntil = 0f;
        }
    }
}
