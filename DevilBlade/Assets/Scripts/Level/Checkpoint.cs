using UnityEngine;

namespace DevilBlade
{
    /// <summary>Miếu chuông: chạm vào để lưu điểm hồi sinh và hồi đầy máu.</summary>
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        public Vector3 spawnOffset = new(0, 0.1f, 0);
        bool _activated;

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponentInParent<PlayerController>();
            if (player == null || _activated) return;
            _activated = true;
            player.Health.Heal(player.Health.max);
            if (GameManager.Instance != null) GameManager.Instance.SetCheckpoint(transform.position + spawnOffset);
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 0.85f, 0.6f);
        }
    }
}
