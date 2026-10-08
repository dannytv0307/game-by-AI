using UnityEngine;

namespace DevilBlade
{
    /// <summary>Hellhound: đứng rình, thấy người chơi thì gầm (báo trước bằng màu đỏ) rồi lao thẳng một đoạn dài.</summary>
    public class HellhoundEnemy : EnemyBase
    {
        public float sightRange = 10f;
        public float telegraphTime = 0.5f;
        public float chargeSpeed = 11f;
        public float chargeTime = 1.0f;
        public float cooldown = 1.4f;
        public int chargeDamage = 20;

        enum Mode { Wait, Telegraph, Charge, Recover }
        Mode _mode;
        float _until, _dir;

        protected override void Awake()
        {
            base.Awake();
            contactDamage = 8;
        }

        protected override void Think()
        {
            switch (_mode)
            {
                case Mode.Wait:
                    Rb.linearVelocity = new Vector2(0, Rb.linearVelocity.y);
                    Anim.Play("idle");
                    if (PlayerAlive && DistX < sightRange && DistY < 3f)
                    {
                        _dir = DirToPlayer;
                        Face(_dir);
                        _mode = Mode.Telegraph;
                        _until = Time.time + telegraphTime;
                        if (Flash) Flash.SetBaseColor(new Color(1f, 0.55f, 0.45f));
                    }
                    break;
                case Mode.Telegraph:
                    // rung nhẹ khi gầm
                    Sr.transform.localPosition = new Vector3(Random.Range(-0.04f, 0.04f), 0, 0);
                    if (Time.time >= _until)
                    {
                        Sr.transform.localPosition = Vector3.zero;
                        if (Flash) Flash.SetBaseColor(Color.white);
                        _mode = Mode.Charge;
                        _until = Time.time + chargeTime;
                    }
                    break;
                case Mode.Charge:
                    Anim.Play("run");
                    Rb.linearVelocity = new Vector2(_dir * chargeSpeed, Rb.linearVelocity.y);
                    TouchPlayer(chargeDamage);
                    if (Time.time >= _until || !CanWalk(_dir))
                    {
                        _mode = Mode.Recover;
                        _until = Time.time + cooldown;
                    }
                    break;
                case Mode.Recover:
                    Rb.linearVelocity = new Vector2(Mathf.MoveTowards(Rb.linearVelocity.x, 0, 30f * Time.deltaTime), Rb.linearVelocity.y);
                    Anim.Play("idle");
                    if (Time.time >= _until) _mode = Mode.Wait;
                    break;
            }
        }
    }
}
