using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevilBlade
{
    /// <summary>
    /// Kael: chạy, nhảy (coyote time + jump buffer), chém, trúng đòn, thanh nộ khí và HÓA QUỶ.
    /// Hình dạng quỷ: mạnh gấp đôi, nhanh hơn, tầm chém rộng hơn; nộ khí cạn dần rồi trở lại hình người.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Di chuyển")]
        public float moveSpeed = 6.5f;
        public float jumpVelocity = 15.5f;
        public float coyoteTime = 0.1f;
        public float jumpBuffer = 0.12f;
        public float jumpCutMultiplier = 0.45f;

        [Header("Chiến đấu")]
        public int damage = 20;
        public Vector2 attackOffset = new(1.0f, 0.9f);
        public Vector2 attackSize = new(1.9f, 1.5f);
        public int attackHitFrame = 2;
        public float knockback = 7f;

        [Header("Nộ khí / Hóa quỷ")]
        public float rageMax = 100f;
        public float rageOnHit = 9f;
        public float rageOnHurt = 12f;
        public float demonDuration = 12f;
        public float demonDamageMultiplier = 2f;
        public float demonSpeedMultiplier = 1.25f;
        public Vector2 demonAttackOffset = new(1.3f, 1.1f);
        public Vector2 demonAttackSize = new(2.6f, 2.0f);
        public int demonHealOnTransform = 25;

        public float Rage { get; private set; }
        public bool RageFull => Rage >= rageMax;
        public bool IsDemon { get; private set; }
        public bool IsTransforming => _state == State.Transforming;
        public bool Grounded { get; private set; }
        public int Facing { get; private set; } = 1;
        public bool ControlsEnabled { get; set; } = true;
        public Health Health { get; private set; }

        public event Action RageBecameFull;
        public event Action<bool> DemonChanged;

        enum State { Normal, Attacking, Hurt, Transforming, Dead }
        State _state;

        Rigidbody2D _rb;
        Collider2D _col;
        SpriteAnimator _anim;
        SpriteRenderer _sr;
        HitFlash _flash;

        InputAction _move, _jump, _attack, _transform;
        float _simMove = float.NaN;
        bool _simJump, _simAttack, _simTransform;

        float _lastGroundedTime = -1f, _lastJumpPressed = -1f, _stateUntil, _demonUntil;
        bool _hitDone, _wasGrounded;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();
            Health = GetComponent<Health>();
            _anim = GetComponentInChildren<SpriteAnimator>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            _flash = GetComponent<HitFlash>();
            CreateInput();
            Health.Damaged += OnDamaged;
            Health.Died += OnDied;
        }

        void CreateInput()
        {
            _move = new InputAction("Move", InputActionType.Value);
            _move.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
            _move.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");
            _move.AddBinding("<Gamepad>/leftStick/x");
            _move.AddBinding("<Gamepad>/dpad/x");
            _jump = new InputAction("Jump", InputActionType.Button);
            _jump.AddBinding("<Keyboard>/space"); _jump.AddBinding("<Keyboard>/w"); _jump.AddBinding("<Keyboard>/upArrow");
            _jump.AddBinding("<Gamepad>/buttonSouth");
            _attack = new InputAction("Attack", InputActionType.Button);
            _attack.AddBinding("<Keyboard>/j"); _attack.AddBinding("<Mouse>/leftButton"); _attack.AddBinding("<Gamepad>/buttonWest");
            _transform = new InputAction("Transform", InputActionType.Button);
            _transform.AddBinding("<Keyboard>/k"); _transform.AddBinding("<Mouse>/rightButton"); _transform.AddBinding("<Gamepad>/buttonNorth");
        }

        void OnEnable() { _move.Enable(); _jump.Enable(); _attack.Enable(); _transform.Enable(); }
        void OnDisable() { _move.Disable(); _jump.Disable(); _attack.Disable(); _transform.Disable(); }
        void OnDestroy() { _move.Dispose(); _jump.Dispose(); _attack.Dispose(); _transform.Dispose(); }

        // ---- API cho test tự động ----
        public void SimulateMove(float x) => _simMove = x;
        public void SimulateJump() => _simJump = true;
        public void SimulateAttack() => _simAttack = true;
        public void SimulateTransform() => _simTransform = true;

        float MoveInput => !float.IsNaN(_simMove) ? _simMove : _move.ReadValue<float>();

        void Update()
        {
            UpdateGrounded();
            var jumpPressed = _jump.WasPressedThisFrame() || Consume(ref _simJump);
            var attackPressed = _attack.WasPressedThisFrame() || Consume(ref _simAttack);
            var transformPressed = _transform.WasPressedThisFrame() || Consume(ref _simTransform);
            if (!ControlsEnabled || _state == State.Dead) return;

            if (jumpPressed) _lastJumpPressed = Time.time;

            switch (_state)
            {
                case State.Normal:
                    if (transformPressed && RageFull && !IsDemon) { BeginTransform(); break; }
                    if (attackPressed) { BeginAttack(); break; }
                    TryJump();
                    if (_jump.WasReleasedThisFrame() && _rb.linearVelocity.y > 0)
                        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * jumpCutMultiplier);
                    break;
                case State.Attacking:
                    UpdateAttack();
                    // bấm tiếp ở frame cuối => nối đòn; nhảy sau khi đòn đã ra => hủy hồi chiêu để né / qua vực
                    if (attackPressed && _anim.Frame >= attackHitFrame + 1) BeginAttack();
                    else if (_hitDone && Time.time - _lastJumpPressed <= jumpBuffer)
                    {
                        _state = State.Normal;
                        TryJump();
                    }
                    break;
                case State.Hurt:
                    if (Time.time >= _stateUntil) _state = State.Normal;
                    break;
                case State.Transforming:
                    if (Time.time >= _stateUntil) EndTransform();
                    break;
            }

            if (IsDemon)
            {
                Rage = Mathf.Max(0f, rageMax * (_demonUntil - Time.time) / demonDuration);
                if (Time.time >= _demonUntil && _state != State.Transforming) RevertToHuman();
            }
            UpdateAnimation();
        }

        static bool Consume(ref bool flag)
        {
            var v = flag;
            flag = false;
            return v;
        }

        void FixedUpdate()
        {
            if (_state == State.Dead || _state == State.Transforming || _state == State.Hurt) return;
            var input = ControlsEnabled ? MoveInput : 0f;
            var speed = moveSpeed * (IsDemon ? demonSpeedMultiplier : 1f);
            if (_state == State.Attacking && Grounded) speed *= 0.25f;
            _rb.linearVelocity = new Vector2(input * speed, _rb.linearVelocity.y);
            if (Mathf.Abs(input) > 0.1f && _state != State.Attacking) Facing = input > 0 ? 1 : -1;
            _sr.flipX = Facing < 0;
        }

        void UpdateGrounded()
        {
            var feet = (Vector2)transform.position + new Vector2(0, -0.02f);
            Grounded = _rb.linearVelocity.y <= 0.05f && Physics2D.OverlapBox(feet, new Vector2(0.5f, 0.1f), 0f, Layers.GroundMask);
            if (Grounded) _lastGroundedTime = Time.time;
            if (Grounded && !_wasGrounded && _state != State.Dead) AudioManager.PlaySfx(a => a.land, 0.6f);
            _wasGrounded = Grounded;
        }

        void TryJump()
        {
            if (Time.time - _lastJumpPressed > jumpBuffer) return;
            if (Time.time - _lastGroundedTime > coyoteTime) return;
            _lastJumpPressed = -1f;
            _lastGroundedTime = -1f;

            // xuống + nhảy trên platform một chiều => rơi xuyên xuống
            if (MoveDownHeld() && TryDropThrough()) return;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpVelocity);
            AudioManager.PlaySfx(a => a.jump, 0.7f);
        }

        static bool MoveDownHeld()
        {
            var kb = Keyboard.current;
            var gp = Gamepad.current;
            return (kb != null && (kb.sKey.isPressed || kb.downArrowKey.isPressed)) || (gp != null && gp.leftStick.ReadValue().y < -0.6f);
        }

        bool TryDropThrough()
        {
            var hit = Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * 0.1f, new Vector2(0.5f, 0.2f), 0f, Layers.GroundMask);
            if (hit == null || hit.GetComponent<PlatformEffector2D>() == null) return false;
            Physics2D.IgnoreCollision(_col, hit, true);
            StartCoroutine(RestoreCollision(hit));
            return true;
        }

        System.Collections.IEnumerator RestoreCollision(Collider2D platform)
        {
            yield return new WaitForSeconds(0.35f);
            Physics2D.IgnoreCollision(_col, platform, false);
        }

        // ---------------- tấn công ----------------
        void BeginAttack()
        {
            _state = State.Attacking;
            _hitDone = false;
            var dir = MoveInput;
            if (Mathf.Abs(dir) > 0.1f) Facing = dir > 0 ? 1 : -1;
            _anim.Play(IsDemon ? "d_attack" : "attack", true);
            AudioManager.PlaySfx(a => a.slash);
        }

        void UpdateAttack()
        {
            if (!_hitDone && _anim.Frame >= attackHitFrame)
            {
                _hitDone = true;
                var off = IsDemon ? demonAttackOffset : attackOffset;
                var size = IsDemon ? demonAttackSize : attackSize;
                var center = (Vector2)transform.position + new Vector2(off.x * Facing, off.y);
                var dmg = Mathf.RoundToInt(damage * (IsDemon ? demonDamageMultiplier : 1f));
                var hits = Combat.HitBox(center, size, Layers.EnemyMask, dmg, Facing, gameObject);
                if (hits > 0)
                {
                    AudioManager.PlaySfx(a => a.hit);
                    HitFlash.HitStop(this, IsDemon ? 0.07f : 0.045f);
                    if (!IsDemon) AddRage(rageOnHit * hits);
                }
            }
            if (_anim.Finished) _state = State.Normal;
        }

        public void AddRage(float amount)
        {
            if (IsDemon) return;
            var wasFull = RageFull;
            Rage = Mathf.Min(rageMax, Rage + amount);
            if (!wasFull && RageFull)
            {
                AudioManager.PlaySfx(a => a.rageFull);
                RageBecameFull?.Invoke();
            }
        }

        // ---------------- hóa quỷ ----------------
        void BeginTransform()
        {
            _state = State.Transforming;
            _rb.linearVelocity = new Vector2(0, Mathf.Min(0, _rb.linearVelocity.y));
            Health.Invulnerable = true;
            _anim.Play("transform", true);
            _stateUntil = Time.time + _anim.Duration("transform");
            AudioManager.PlaySfx(a => a.transformBurst);
            if (AudioManager.Instance != null) AudioManager.Instance.Voice(AudioManager.Instance.voiceTransform);
        }

        void EndTransform()
        {
            IsDemon = true;
            _demonUntil = Time.time + demonDuration;
            Health.Invulnerable = false;
            Health.Heal(demonHealOnTransform);
            _state = State.Normal;
            if (_flash) _flash.SetBaseColor(new Color(1f, 0.92f, 0.88f));
            DemonChanged?.Invoke(true);
        }

        void RevertToHuman()
        {
            IsDemon = false;
            Rage = 0f;
            if (_flash) { _flash.SetBaseColor(Color.white); _flash.Flash(0.25f); }
            if (_state == State.Attacking) _state = State.Normal;
            DemonChanged?.Invoke(false);
        }

        // ---------------- trúng đòn / chết ----------------
        void OnDamaged(int amount, float fromDir)
        {
            if (_state == State.Dead) return;
            AddRage(rageOnHurt);
            AudioManager.PlaySfx(a => a.hurt);
            if (_flash) _flash.Flash(0.15f);
            if (IsDemon) return; // dạng quỷ không bị khựng
            _state = State.Hurt;
            _stateUntil = Time.time + 0.3f;
            _rb.linearVelocity = new Vector2(fromDir * knockback, 6f);
        }

        void OnDied()
        {
            _state = State.Dead;
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            _anim.Play("hurt", true);
        }

        public void Respawn(Vector3 position)
        {
            if (IsDemon) RevertToHuman();
            Rage = 0f;
            transform.position = position;
            _rb.linearVelocity = Vector2.zero;
            Health.ResetFull();
            Health.Invulnerable = false;
            _state = State.Normal;
            _anim.Play("idle", true);
        }

        // ---------------- hình ảnh ----------------
        void UpdateAnimation()
        {
            // nhấp nháy khi đang bất tử sau khi trúng đòn
            _sr.enabled = _state == State.Dead || !(Time.time - _lastHurtVisual < 0.8f && Mathf.Repeat(Time.time * 20f, 1f) < 0.3f);
            if (_state is State.Attacking or State.Transforming) return;
            if (_state is State.Hurt or State.Dead) { _anim.Play("hurt"); return; }
            if (IsDemon)
            {
                _anim.Play("d_idle");
                return;
            }
            if (!Grounded) _anim.Play(_rb.linearVelocity.y > 0 ? "jump" : "fall");
            else _anim.Play(Mathf.Abs(_rb.linearVelocity.x) > 0.2f ? "run" : "idle");
        }

        float _lastHurtVisual = -10f;

        void LateUpdate()
        {
            if (_state == State.Hurt) _lastHurtVisual = Time.time;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var f = Application.isPlaying ? Facing : 1;
            Gizmos.DrawWireCube(transform.position + new Vector3(attackOffset.x * f, attackOffset.y), attackSize);
        }
    }
}
