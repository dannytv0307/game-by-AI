using System;
using UnityEngine;

namespace DevilBlade
{
    /// <summary>
    /// Boss màn 1 — Fallen Demon Knight.
    ///  - Chém mạnh (Cleaver): báo trước 0.8s (đỏ + rung), rồi chém vùng rộng trước mặt.
    ///  - Lao tới (Charge): báo trước, lao ngang đấu trường.
    ///  - Giai đoạn 2 (&lt; 50% máu): nhanh hơn và triệu hồi Imp.
    /// </summary>
    public class DemonKnightBoss : EnemyBase
    {
        public float walkSpeed = 1.8f;
        public int cleaverDamage = 26;
        public Vector2 cleaverOffset = new(2.0f, 1.4f);
        public Vector2 cleaverSize = new(3.6f, 2.8f);
        public float cleaverTelegraph = 0.8f;
        public int chargeDamage = 22;
        public float chargeSpeed = 9f;
        public float chargeTelegraph = 0.6f;
        public ImpEnemy impTemplate;
        public Transform[] summonPoints;

        public bool Active { get; private set; }
        public bool PhaseTwo => Health.Normalized <= 0.5f;
        public event Action Defeated;

        enum Mode { Walk, CleaverWindup, CleaverStrike, ChargeWindup, Charge, Recover, Summon }
        Mode _mode;
        float _until, _dir;
        bool _summoned;

        protected override void Awake()
        {
            base.Awake();
            knockbackTaken = 0f;
            stunOnHit = 0f;
        }

        public void Activate()
        {
            Active = true;
            _mode = Mode.Recover;
            _until = Time.time + 1.2f;
        }

        protected override void Update()
        {
            if (!Active) return;
            base.Update();
        }

        float Speedup => PhaseTwo ? 1.35f : 1f;

        protected override void Think()
        {
            Anim.Play("idle");
            // vừa xuống dưới 50% máu => triệu hồi ngay khi rảnh tay (không đợi hết chuỗi đòn)
            if (PhaseTwo && !_summoned && _mode is Mode.Walk or Mode.Recover) { StartSummon(); return; }
            switch (_mode)
            {
                case Mode.Walk:
                    Face(DirToPlayer);
                    Rb.linearVelocity = new Vector2(DirToPlayer * walkSpeed * Speedup, Rb.linearVelocity.y);
                    if (!PlayerAlive) break;
                    if (DistX < 3.2f) StartWindup(Mode.CleaverWindup, cleaverTelegraph);
                    else if (DistX > 6f || UnityEngine.Random.value < 0.004f) StartWindup(Mode.ChargeWindup, chargeTelegraph);
                    break;

                case Mode.CleaverWindup:
                case Mode.ChargeWindup:
                    Rb.linearVelocity = new Vector2(0, Rb.linearVelocity.y);
                    Sr.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), 0, 0);
                    if (Time.time < _until) break;
                    Sr.transform.localPosition = Vector3.zero;
                    if (Flash) Flash.SetBaseColor(Color.white);
                    if (_mode == Mode.CleaverWindup) DoCleaver();
                    else { _mode = Mode.Charge; _dir = Facing; _until = Time.time + 2.2f; }
                    break;

                case Mode.Charge:
                    Rb.linearVelocity = new Vector2(_dir * chargeSpeed * Speedup, Rb.linearVelocity.y);
                    TouchPlayer(chargeDamage);
                    if (Time.time >= _until || !CanWalk(_dir)) Recover(1.0f);
                    break;

                case Mode.CleaverStrike:
                    if (Time.time >= _until) Recover(0.9f);
                    break;

                case Mode.Summon:
                    Rb.linearVelocity = new Vector2(0, Rb.linearVelocity.y);
                    if (Time.time >= _until) Recover(0.6f);
                    break;

                case Mode.Recover:
                    Rb.linearVelocity = new Vector2(Mathf.MoveTowards(Rb.linearVelocity.x, 0, 40f * Time.deltaTime), Rb.linearVelocity.y);
                    if (Time.time >= _until) _mode = Mode.Walk;
                    break;
            }
        }

        void StartWindup(Mode mode, float time)
        {
            Face(DirToPlayer);
            _mode = mode;
            _until = Time.time + time / Speedup;
            if (Flash) Flash.SetBaseColor(new Color(1f, 0.45f, 0.4f));
        }

        void DoCleaver()
        {
            _mode = Mode.CleaverStrike;
            _until = Time.time + 0.25f;
            var center = (Vector2)transform.position + new Vector2(cleaverOffset.x * Facing, cleaverOffset.y);
            AudioManager.PlaySfx(a => a.slash, 1f);
            if (Combat.HitBox(center, cleaverSize, Layers.PlayerMask, cleaverDamage, Facing, gameObject) > 0)
                AudioManager.PlaySfx(a => a.hit);
            Rb.linearVelocity = new Vector2(Facing * 3f, Rb.linearVelocity.y); // bước dậm tới
        }

        void StartSummon()
        {
            _summoned = true;
            _mode = Mode.Summon;
            _until = Time.time + 1.0f;
            if (Flash) Flash.Flash(0.4f);
            AudioManager.PlaySfx(a => a.rageFull);
            if (impTemplate == null || summonPoints == null) return;
            foreach (var p in summonPoints)
            {
                var imp = Instantiate(impTemplate, p.position, Quaternion.identity, transform.parent);
                imp.gameObject.SetActive(true);
            }
        }

        void Recover(float time)
        {
            _mode = Mode.Recover;
            _until = Time.time + time / Speedup;
        }

        protected override void OnDied()
        {
            base.OnDied();
            Defeated?.Invoke();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(cleaverOffset.x, cleaverOffset.y), cleaverSize);
        }
    }
}
