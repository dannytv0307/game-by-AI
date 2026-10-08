using UnityEngine;

namespace DevilBlade
{
    /// <summary>
    /// Chuyển động thủ tục cho sprite tư thế tĩnh (bản HD): thở khi đứng, nhún + nghiêng người khi chạy,
    /// nghiêng theo hướng bay khi nhảy. Bù cho việc AI chưa vẽ được animation chân thật đồng nhất từng frame.
    /// Gắn trên object con "Sprite" (pivot ở chân nên xoay/co giãn quanh bàn chân).
    /// </summary>
    public class ProceduralPoseMotion : MonoBehaviour
    {
        public float breathAmount = 0.012f;
        public float breathSpeed = 2.2f;
        public float runBob = 0.06f;
        public float runStepSpeed = 11f;
        public float runLean = 6f;
        public float airTilt = 4f;
        public float smoothing = 12f;

        Rigidbody2D _rb;
        SpriteRenderer _sr;
        Vector3 _basePos;
        float _phase, _lean;

        void Awake()
        {
            _rb = GetComponentInParent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            _basePos = transform.localPosition;
        }

        void LateUpdate()
        {
            if (_rb == null) return;
            var v = _rb.linearVelocity;
            var moving = Mathf.Abs(v.x) > 0.3f;
            var airborne = Mathf.Abs(v.y) > 0.5f;
            var facing = _sr.flipX ? -1f : 1f;

            float bob = 0f, scaleY = 1f, targetLean;
            if (airborne)
            {
                targetLean = Mathf.Clamp(-v.y * airTilt * 0.15f, -airTilt, airTilt) * facing;
            }
            else if (moving)
            {
                _phase += Time.deltaTime * runStepSpeed * Mathf.Clamp01(Mathf.Abs(v.x) / 6f);
                bob = Mathf.Abs(Mathf.Sin(_phase)) * runBob;
                targetLean = -runLean * Mathf.Sign(v.x);
            }
            else
            {
                _phase += Time.deltaTime * breathSpeed;
                scaleY = 1f + Mathf.Sin(_phase) * breathAmount;
                targetLean = 0f;
            }

            _lean = Mathf.Lerp(_lean, targetLean, 1f - Mathf.Exp(-smoothing * Time.deltaTime));
            transform.localPosition = _basePos + new Vector3(0, bob, 0);
            transform.localRotation = Quaternion.Euler(0, 0, _lean);
            transform.localScale = new Vector3(1f / Mathf.Sqrt(scaleY), scaleY, 1f);
        }
    }
}
