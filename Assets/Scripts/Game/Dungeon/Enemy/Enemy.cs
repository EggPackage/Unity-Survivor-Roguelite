using System.Collections;
using UnityEngine;
using Game.Dungeon.Combat;
using Game.Dungeon.Config;

namespace Game.Dungeon.Enemy
{
    /// <summary>
    /// 地牢敌人（"执念怪物"）：追击玩家 + 接触伤害 + 受击击退 + 死亡回收 + 视觉反馈。
    ///
    /// ==== 设计说明 ====
    /// 对应 GDD"地牢中危机的执念怪物"。原型只做最简 AI：朝玩家直线追。
    /// 走 PoolMgr 对象池（预设需挂 PoolObj），死亡不 Destroy 而是 PushObj 回收，
    /// 反复刷怪不产生 GC 峰值。数值优先读 EnemyConfig（数据驱动），
    /// 没配就用 Inspector 默认值兜底，保证无 .asset 也能跑。
    ///
    /// 血量交给通用 Health 组件，这里只订阅它的 OnDamaged / OnDeath 做表现与回收。
    ///
    /// 视觉反馈（本次新增）：
    ///   - 动画：占位 controller 无参数，照搬 DungeonPlayerController 的做法用
    ///     Animator.Play(状态名) 直接切 Idle/Run，状态名做成可配置字段。
    ///   - 朝向：横版精灵按移动方向的 x 分量翻转 Visual.flipX（朝玩家哪边就面向哪边）。
    ///   - 受击闪白：OnDamaged 时把 Visual 染白再用协程恢复，配合击退给打击反馈。
    ///   - 死亡特效：OnDeath 从对象池取死亡特效摆到自身位置，再回收自己。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour
    {
        [Header("配置（留空用下方默认值兜底）")]
        [SerializeField] private EnemyConfig config;

        [Header("默认数值（无配置时生效）")]
        [SerializeField] private int defaultMaxHP = 30;
        [SerializeField] private float defaultMoveSpeed = 2f;
        [SerializeField] private int defaultContactDamage = 10;
        [SerializeField] private float defaultAttackInterval = 1f;

        [Header("受击表现")]
        [Tooltip("受击击退时长（秒）")]
        [SerializeField] private float knockbackTime = 0.12f;
        [Tooltip("对象池预设名（须与 Resources 下预设名一致，勿改 gameObject.name）")]
        [SerializeField] private string poolResName = "Enemy";
        [Tooltip("受击闪白持续时间（秒）")]
        [SerializeField] private float flashTime = 0.08f;

        [Header("视觉引用（留空自动找 Visual 子节点）")]
        [Tooltip("显示用子物体的 SpriteRenderer")]
        [SerializeField] private SpriteRenderer visualRenderer;
        [Tooltip("显示用子物体的 Animator")]
        [SerializeField] private Animator animator;

        [Header("动画状态名（直接播放，不依赖 controller 参数）")]
        [Tooltip("待机动画状态名。占位素材：Warrior_Idle_Red")]
        [SerializeField] private string idleStateName = "Warrior_Idle_Red";
        [Tooltip("移动动画状态名。占位素材：Warrior_Run_Red")]
        [SerializeField] private string runStateName = "Warrior_Run_Red";

        [Header("死亡特效")]
        [Tooltip("Resources 下死亡特效预设名（挂 PoolObj），留空则不生成")]
        [SerializeField] private string deathVfxResName = "DungeonHitVfx";

        private Rigidbody2D _rb;
        private Health _health;
        private Transform _player;

        private float _moveSpeed;
        private int _contactDamage;
        private float _attackInterval;
        private float _lastContactTime;   // 上次对玩家造成接触伤害的时间
        private float _knockbackUntil;    // 击退结束时间（此前不主动追击）

        private int _curStateHash;        // 当前动画状态哈希，去重避免每帧重播
        private Color _baseColor = Color.white;
        private Coroutine _flashCo;
        private bool _isDying;             // 防止 Health.OnDeath 被多次触发导致重复回池

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _health = GetComponent<Health>();
            _rb.gravityScale = 0f;

            if (visualRenderer == null || animator == null)
            {
                Transform v = transform.Find("Visual");
                if (v != null)
                {
                    if (visualRenderer == null) visualRenderer = v.GetComponent<SpriteRenderer>();
                    if (animator == null) animator = v.GetComponent<Animator>();
                }
            }
            if (visualRenderer != null) _baseColor = visualRenderer.color;
        }

        private void OnEnable()
        {
            ApplyConfig();
            _health.OnDamaged += OnDamaged;
            _health.OnDeath += OnDeath;
            _lastContactTime = -999f;
            _knockbackUntil = 0f;

            // 池复用重置：清残留闪白协程、复位颜色与动画状态、解除死亡锁
            _flashCo = null;
            _curStateHash = 0;
            _isDying = false;
            if (visualRenderer != null) visualRenderer.color = _baseColor;

            AcquirePlayer();
        }

        private void OnDisable()
        {
            _health.OnDamaged -= OnDamaged;
            _health.OnDeath -= OnDeath;
            _flashCo = null; // 失活时协程已随之停止，清引用即可
        }

        /// <summary>
        /// 应用数值：优先配置，缺失用默认值。同步把血量写进 Health。
        /// </summary>
        private void ApplyConfig()
        {
            int maxHP = config != null ? config.maxHP : defaultMaxHP;
            _moveSpeed = config != null ? config.moveSpeed : defaultMoveSpeed;
            _contactDamage = config != null ? config.contactDamage : defaultContactDamage;
            _attackInterval = config != null ? config.attackInterval : defaultAttackInterval;
            _health.SetMaxHP(maxHP);
        }

        /// <summary>
        /// 找到玩家。原型用 Tag 查找，找不到不报错（玩家可能尚未生成）。
        /// </summary>
        private void AcquirePlayer()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
        }

        private void FixedUpdate()
        {
            if (_health.IsDead) return;
            if (Time.time < _knockbackUntil) return; // 击退期间不主动移动
            if (_player == null) { AcquirePlayer(); PlayState(idleStateName); return; }

            Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
            _rb.MovePosition(_rb.position + dir * _moveSpeed * Time.fixedDeltaTime);

            // 面向玩家（横版只区分左右）：默认素材朝右，朝左才 flipX
            if (visualRenderer != null && Mathf.Abs(dir.x) > 0.01f)
                visualRenderer.flipX = dir.x < 0f;

            PlayState(runStateName);
        }

        /// <summary>
        /// 直接播放动画状态（占位 controller 无参数，同 DungeonPlayerController 的做法）。
        /// </summary>
        private void PlayState(string stateName)
        {
            if (animator == null || string.IsNullOrEmpty(stateName)) return;
            int hash = Animator.StringToHash(stateName);
            if (hash == _curStateHash) return;
            _curStateHash = hash;
            animator.Play(hash, 0, 0f);
        }

        /// <summary>
        /// 接触玩家造成伤害（带冷却，避免每帧狂扣）。
        /// </summary>
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_health.IsDead) return;
            if (!collision.collider.CompareTag("Player")) return;
            if (Time.time - _lastContactTime < _attackInterval) return;

            Health playerHealth = collision.collider.GetComponent<Health>();
            if (playerHealth == null || playerHealth.IsDead) return;

            Vector2 dir = ((Vector2)collision.transform.position - _rb.position).normalized;
            playerHealth.TakeDamage(new Damage(_contactDamage, collision.transform.position, dir, 0f));
            EventCenter.Instance.EventTrigger(E_EventType.E_Player_Hit, _contactDamage);
            _lastContactTime = Time.time;
        }

        /// <summary>
        /// 受击：施加击退 + 闪白。
        /// </summary>
        private void OnDamaged(Damage dmg)
        {
            if (dmg.knockbackForce > 0f && dmg.knockbackDir != Vector2.zero)
            {
                _rb.velocity = Vector2.zero;
                _rb.AddForce(dmg.knockbackDir * dmg.knockbackForce, ForceMode2D.Impulse);
                _knockbackUntil = Time.time + knockbackTime;
            }

            if (visualRenderer != null && !_health.IsDead)
            {
                if (_flashCo != null) StopCoroutine(_flashCo);
                _flashCo = StartCoroutine(FlashRoutine());
            }
        }

        /// <summary>
        /// 受击闪白：染白后恢复原色。用 unscaledDeltaTime，兼容命中顿帧（timeScale=0）。
        /// </summary>
        private IEnumerator FlashRoutine()
        {
            visualRenderer.color = Color.white;
            float t = 0f;
            while (t < flashTime)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            visualRenderer.color = _baseColor;
            _flashCo = null;
        }

        /// <summary>
        /// 死亡：广播 E_Monster_Dead → 生成死亡特效 → 回收进池。
        /// 加 bool 保护避免 Health 多次触发 OnDeath 导致 PushObj 被重复调用。
        /// PushObj 抛异常时回退为 Destroy，避免敌人卡死在场上。
        /// </summary>
        private void OnDeath()
        {
            if (_isDying) return;
            _isDying = true;

            EventCenter.Instance.EventTrigger(E_EventType.E_Monster_Dead);
            if (_rb != null) _rb.velocity = Vector2.zero;

            if (visualRenderer != null) visualRenderer.color = _baseColor; // 回收前复位颜色

            try
            {
                SpawnDeathVfx();
                PoolMgr.Instance.PushObj(gameObject);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Enemy] 回收到对象池失败，回退为 Destroy：{ex.Message}");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 从对象池取死亡特效摆到自身位置。特效自带 AutoRecycleVfx 回收，异常不影响回池。
        /// </summary>
        private void SpawnDeathVfx()
        {
            if (string.IsNullOrEmpty(deathVfxResName)) return;
            try
            {
                GameObject vfx = PoolMgr.Instance.GetObj(deathVfxResName);
                if (vfx != null) vfx.transform.position = transform.position;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Enemy] 生成死亡特效失败（不影响回收）：{ex.Message}");
            }
        }
    }
}