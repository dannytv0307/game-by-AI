using System.Collections;
using UnityEngine;

namespace DevilBlade
{
    /// <summary>Phần chung của quái: máu, trúng đòn (knockback + nháy), sát thương khi chạm, chết, tìm người chơi.</summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour
    {
        public int contactDamage = 10;
        public Vector2 contactSize = new(0.9f, 1.0f);
        public Vector2 contactOffset = new(0f, 0.5f);
        public float knockbackTaken = 4f;
        public float stunOnHit = 0.2f;
        public float rageReward = 0f; // nộ khí thưởng thêm khi giết

        protected Rigidbody2D Rb;
        protected Health Health;
        protected SpriteAnimator Anim;
        protected SpriteRenderer Sr;
        protected HitFlash Flash;
        protected PlayerController Player;
        protected float StunnedUntil;

        public bool IsDead => Health.IsDead;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Health = GetComponent<Health>();
            Anim = GetComponentInChildren<SpriteAnimator>();
            Sr = GetComponentInChildren<SpriteRenderer>();
            Flash = GetComponent<HitFlash>();
            Health.Damaged += OnDamaged;
            Health.Died += OnDied;
        }

        protected virtual void Start() => Player = FindAnyObjectByType<PlayerController>();

        protected virtual void Update()
        {
            if (IsDead || Player == null) return;
            if (Time.time >= StunnedUntil) Think();
            if (contactDamage > 0) TouchPlayer(contactDamage);
        }

        protected abstract void Think();

        protected float DirToPlayer => Mathf.Sign(Player.transform.position.x - transform.position.x);
        protected float DistX => Mathf.Abs(Player.transform.position.x - transform.position.x);
        protected float DistY => Mathf.Abs(Player.transform.position.y - transform.position.y);
        protected bool PlayerAlive => Player != null && !Player.Health.IsDead;

        protected void Face(float dir)
        {
            if (Mathf.Abs(dir) > 0.01f) Sr.flipX = dir < 0;
        }

        protected int Facing => Sr.flipX ? -1 : 1;

        /// <summary>Gây sát thương nếu thân quái chạm người chơi.</summary>
        protected void TouchPlayer(int dmg)
        {
            var center = (Vector2)transform.position + contactOffset;
            Combat.HitBox(center, contactSize, Layers.PlayerMask, dmg, DirToPlayer, gameObject);
        }

        /// <summary>Kiểm tra mép vực / tường phía trước để quái không tự lao xuống vực.</summary>
        protected bool CanWalk(float dir)
        {
            var pos = (Vector2)transform.position;
            var ahead = pos + new Vector2(dir * (contactSize.x * 0.5f + 0.2f), 0);
            var ground = Physics2D.Raycast(ahead + Vector2.up * 0.2f, Vector2.down, 0.8f, Layers.GroundMask);
            var wall = Physics2D.Raycast(pos + Vector2.up * 0.4f, new Vector2(dir, 0), contactSize.x * 0.5f + 0.15f, Layers.GroundMask);
            return ground.collider != null && wall.collider == null;
        }

        protected virtual void OnDamaged(int amount, float fromDir)
        {
            if (Flash) Flash.Flash();
            StunnedUntil = Time.time + stunOnHit;
            if (knockbackTaken > 0) Rb.linearVelocity = new Vector2(fromDir * knockbackTaken, Mathf.Max(Rb.linearVelocity.y, 2f));
        }

        protected virtual void OnDied()
        {
            AudioManager.PlaySfx(a => a.enemyDie);
            if (Player != null && rageReward > 0) Player.AddRage(rageReward);
            foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
            Rb.bodyType = RigidbodyType2D.Kinematic;
            Rb.linearVelocity = Vector2.zero;
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            var start = Sr.color;
            for (float t = 0; t < 0.6f; t += Time.deltaTime)
            {
                Sr.color = new Color(1f, 0.3f, 0.3f, 1f - t / 0.6f);
                transform.localScale = new Vector3(1f + t * 0.4f, 1f - t * 0.6f, 1f);
                yield return null;
            }
            Sr.color = start;
            gameObject.SetActive(false);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position + (Vector3)contactOffset, contactSize);
        }
    }
}
