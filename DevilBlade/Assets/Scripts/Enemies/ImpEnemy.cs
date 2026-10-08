using UnityEngine;

namespace DevilBlade
{
    /// <summary>Imp: tuần tra, thấy người chơi thì lao tới và vồ (nhảy về phía trước, cào ở frame giữa).</summary>
    public class ImpEnemy : EnemyBase
    {
        public float patrolSpeed = 1.6f;
        public float chaseSpeed = 4.2f;
        public float sightRange = 7f;
        public float attackRange = 1.7f;
        public int attackDamage = 14;
        public float attackCooldown = 1.1f;
        public Vector2 attackSize = new(1.3f, 1.0f);
        public float patrolRadius = 3f;

        float _patrolDir = 1f, _nextAttack, _homeX;
        bool _attacking, _hitDone;

        protected override void Start()
        {
            base.Start();
            _homeX = transform.position.x;
        }

        protected override void Think()
        {
            if (_attacking)
            {
                if (!_hitDone && Anim.Frame >= 2)
                {
                    _hitDone = true;
                    var center = (Vector2)transform.position + new Vector2(Facing * 0.8f, 0.5f);
                    Combat.HitBox(center, attackSize, Layers.PlayerMask, attackDamage, Facing, gameObject);
                }
                if (Anim.Finished) { _attacking = false; Anim.Play("run"); }
                return;
            }

            var seesPlayer = PlayerAlive && DistX < sightRange && DistY < 2.5f;
            if (seesPlayer && DistX < attackRange && Time.time >= _nextAttack)
            {
                Face(DirToPlayer);
                _attacking = true;
                _hitDone = false;
                _nextAttack = Time.time + attackCooldown;
                Anim.Play("attack", true);
                Rb.linearVelocity = new Vector2(Facing * 3.5f, 4f); // vồ
                return;
            }

            float dir, speed;
            if (seesPlayer) { dir = DirToPlayer; speed = chaseSpeed; }
            else
            {
                var offset = transform.position.x - _homeX;
                if (!CanWalk(_patrolDir) || Mathf.Abs(offset) > patrolRadius && Mathf.Sign(offset) == _patrolDir) _patrolDir = -_patrolDir;
                dir = _patrolDir; speed = patrolSpeed;
            }
            if (!CanWalk(dir)) speed = 0f;
            Face(dir);
            Rb.linearVelocity = new Vector2(dir * speed, Rb.linearVelocity.y);
            Anim.Play("run");
        }
    }
}
