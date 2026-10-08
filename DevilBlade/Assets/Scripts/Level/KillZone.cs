using UnityEngine;

namespace DevilBlade
{
    /// <summary>Vùng rơi xuống vực: người chơi chết ngay, quái biến mất.</summary>
    [RequireComponent(typeof(Collider2D))]
    public class KillZone : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponentInParent<Health>();
            if (health != null) health.Kill();
        }
    }
}
