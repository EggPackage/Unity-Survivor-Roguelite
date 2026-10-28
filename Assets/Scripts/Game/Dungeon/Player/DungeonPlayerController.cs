using UnityEngine;
using Game.Dungeon.Combat;
using Game.Dungeon.Input;
using Game.Dungeon.Weapon;

namespace Game.Dungeon.Player
{
    /// <summary>
    /// 玩家状态（M1 精简：自动攻击不再锁移动，只保留待机/移动两态）。
    /// </summary>
    public enum E_PlayerState
    {
        /// <summary>待机</summary>
        Idle,
        /// <summary>移动</summary>
        Move,
    }

    /// <summary>
    /// 地牢玩家控制器：轻量 FSM + 物理移动 + 面向翻转 + 自动攻击挂载。
    ///
    /// ==== 设计说明 ====
    /// 顶视角 3/4 动作砍杀。用一个 enum + switch 的轻量状态机（项目没有 FSM 基类，
    /// Bar 模块也是直接判断，保持一致，不引重型框架）。
    ///
    /// 输入（M1 起改用 Unity 新 Input System，经 DungeonInput 意图层接入）：
    ///   - 移动：订阅 DungeonInput.OnMove(Vector2)，缓存输入向量，在 FixedUpdate 里
    ///     用 Rigidbody2D.MovePosition 做物理位移（不直接改 transform，避免穿墙）。
    ///     已支持键盘 WASD + 手柄左摇杆，业务代码不认识设备细节。
    ///
    /// 攻击（M1 改为自动攻击）：
    ///   - 本类不再处理攻击键；Awake 时确保场景里有 WeaponRunner 组件（没有就补一个），
    ///     由它按 WeaponConfig.interval 定时调用武器策略开火。攻击方向优先指向
    ///     ActorRegistry 里最近的敌人，业务层不参与。
    ///   - 开火后 WeaponRunner 会抛 OnFired，本类订阅它播一段攻击动作（两段交替），
    ///     并在动作时长内屏蔽 Idle/Run 覆盖，否则动画会被每帧打断。
    ///     时长从 Animator 的剪辑列表里读，不在代码里写死。
    ///
    /// 无敌帧（M1）：
    ///   - Awake 时把自己的 Health.invulnerableTime 设成 playerInvulnerableTime（默认 0.5s），
    ///     避免被多个敌人贴身时一帧内暴毙。
    ///
    /// 朝向：Tiny Swords 是横版精灵，只区分左右。每帧看水平输入翻转 Visual 的
    ///   SpriteRenderer.flipX。只翻 Visual，不翻根节点，保证刚体/碰撞/攻击原点不受影响。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class DungeonPlayerController : MonoBehaviour
    {
        [Header("组件引用")]
        [Tooltip("显示用子物体（挂 SpriteRenderer / Animator）。留空自动找名为 Visual 的子物体")]
        [SerializeField] private Transform visual;
        [Tooltip("留空则从 visual 上自动获取")]
        [SerializeField] private SpriteRenderer visualRenderer;
        [Tooltip("留空则从 visual 上自动获取")]
        [SerializeField] private Animator animator;

        [Header("无敌帧")]
        [Tooltip("玩家受击后的无敌时长（秒）")]
        [SerializeField] private float playerInvulnerableTime = 0.5f;

        [Header("动画状态名（直接播放，不依赖 controller 参数）")]
        [Tooltip("待机动画状态名。占位素材：Warrior_Idle_Blue")]
        [SerializeField] private string idleStateName = "Warrior_Idle_Blue";
        [Tooltip("移动动画状态名。占位素材：Warrior_Run_Blue")]
        [SerializeField] private string runStateName = "Warrior_Run_Blue";

        [Header("攻击动画（自动攻击开火时播放，播完回 Idle/Run）")]
        [Tooltip("攻击状态名（第一段）。占位素材：Warrior_Attack_Blue")]
        [SerializeField] private string attackStateNameA = "Warrior_Attack_Blue";
        [Tooltip("攻击状态名（第二段，与第一段交替，避免每次同一动作）")]
        [SerializeField] private string attackStateNameB = "Warrior_Attack2_Blue";
        [Tooltip("第一段状态对应的剪辑名，用于取时长（状态名与剪辑名不一定相同）")]
        [SerializeField] private string attackClipNameA = "Warrior_Attack1_Blue";
        [Tooltip("第二段状态对应的剪辑名，用于取时长")]
        [SerializeField] private string attackClipNameB = "Warrior_Attack2_Blue";
        [Tooltip("取不到剪辑长度时的兜底时长（秒）")]
        [SerializeField] private float attackFallbackDuration = 0.4f;

        private Rigidbody2D _rb;
        private PlayerStats _stats;
        private Health _health;
        private WeaponRunner _weapon;

        private E_PlayerState _state = E_PlayerState.Idle;
        private Vector2 _moveInput;         // 缓存的输入向量（-1~1）
        private bool _facingRight = true;   // 当前朝向

        private int _curStateHash;          // 当前动画状态哈希，避免重复 Play 打断自身

        private int _attackHashA, _attackHashB;
        private float _attackDurationA, _attackDurationB;
        private float _attackUntil;         // 攻击动作播到这个时刻之前，不覆盖动画
        private bool _attackToggle;         // 两段攻击交替标记

        private void Reset()
        {
            visual = transform.Find("Visual");
            if (visual != null)
            {
                visualRenderer = visual.GetComponent<SpriteRenderer>();
                animator = visual.GetComponent<Animator>();
            }
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _stats = GetComponent<PlayerStats>();
            _health = GetComponent<Health>();

            if (visual == null) visual = transform.Find("Visual");
            if (visual != null)
            {
                if (visualRenderer == null) visualRenderer = visual.GetComponent<SpriteRenderer>();
                if (animator == null) animator = visual.GetComponent<Animator>();
            }

            // 顶视角，无重力
            _rb.gravityScale = 0f;

            // T4：玩家无敌帧
            if (_health != null) _health.SetInvulnerableTime(playerInvulnerableTime);

            // 攻击动画：状态哈希与剪辑时长一次性缓存，不在开火路径上算/分配
            _attackHashA = Animator.StringToHash(attackStateNameA);
            _attackHashB = Animator.StringToHash(attackStateNameB);
            CacheAttackDurations();

            // T2：自动攻击驱动器（场景未挂则运行时补上，保证直接 Play 就能打怪）
            _weapon = GetComponent<WeaponRunner>();
            if (_weapon == null) _weapon = gameObject.AddComponent<WeaponRunner>();
        }

        private void OnEnable()
        {
            // M1：新 Input System 意图层
            DungeonInput.EnsureEnabled();
            DungeonInput.OnMove += OnMoveInput;

            // 自动攻击开火 -> 播攻击动作
            if (_weapon != null) _weapon.OnFired += OnWeaponFired;

            // T3：注册为玩家，供敌人/武器查询，替代 FindGameObjectWithTag
            ActorRegistry.RegisterPlayer(transform);
        }

        private void OnDisable()
        {
            DungeonInput.OnMove -= OnMoveInput;
            if (_weapon != null) _weapon.OnFired -= OnWeaponFired;
            ActorRegistry.UnregisterPlayer(transform);
        }

        private void OnMoveInput(Vector2 v) => _moveInput = v;

        private void Update()
        {
            UpdateFacing();
            UpdateState();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            float speed = _stats != null ? _stats.MoveSpeed : 5f;
            Vector2 dir = _moveInput.sqrMagnitude > 1f ? _moveInput.normalized : _moveInput;
            _rb.MovePosition(_rb.position + dir * speed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// 根据水平输入翻转 Visual：向右走朝右，向左走朝左，不动保持。
        /// 只翻显示层，不动根节点。
        /// </summary>
        private void UpdateFacing()
        {
            if (_moveInput.x > 0.01f) _facingRight = true;
            else if (_moveInput.x < -0.01f) _facingRight = false;

            if (visualRenderer != null)
                visualRenderer.flipX = !_facingRight; // 素材默认朝右，朝左才翻
        }

        private void UpdateState()
        {
            _state = _moveInput.sqrMagnitude > 0.001f ? E_PlayerState.Move : E_PlayerState.Idle;
        }

        private void UpdateAnimator()
        {
            // 攻击动作播放期间不覆盖：否则 Idle/Run 每帧把攻击动画打断在第一帧
            if (Time.time < _attackUntil) return;

            PlayState(_state == E_PlayerState.Move ? runStateName : idleStateName);
        }

        /// <summary>
        /// 武器开火回调：播一段攻击动作，两段交替。
        /// 用 _attackUntil 标记占位时长，期间 UpdateAnimator 不覆盖。
        /// </summary>
        private void OnWeaponFired()
        {
            if (animator == null) return;

            _attackToggle = !_attackToggle;
            int hash = _attackToggle ? _attackHashA : _attackHashB;
            float duration = _attackToggle ? _attackDurationA : _attackDurationB;

            animator.Play(hash, 0, 0f);
            _curStateHash = hash;                        // 让 PlayState 的去重逻辑认它
            _attackUntil = Time.time + Mathf.Max(0.01f, duration);
        }

        /// <summary>
        /// 从 Animator 控制器的剪辑列表取两段攻击的时长，只在 Awake 跑一次。
        /// 状态名和剪辑名在本素材里不一致（Attack_Blue 对应 Attack1_Blue），
        /// 所以拆成两组字段，避免靠字符串猜。取不到就用 Inspector 兜底值。
        /// </summary>
        private void CacheAttackDurations()
        {
            _attackDurationA = attackFallbackDuration;
            _attackDurationB = attackFallbackDuration;

            if (animator == null || animator.runtimeAnimatorController == null) return;

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];
                if (clip == null) continue;

                if (clip.name == attackClipNameA) _attackDurationA = clip.length;
                else if (clip.name == attackClipNameB) _attackDurationB = clip.length;
            }
        }

        /// <summary>
        /// 直接播放指定动画状态（占位 controller 无参数，按状态名直接切）。
        /// 用 _curStateHash 去重，避免同一状态每帧重播导致动画卡在第一帧。
        /// </summary>
        private void PlayState(string stateName)
        {
            if (animator == null || string.IsNullOrEmpty(stateName)) return;
            int hash = Animator.StringToHash(stateName);
            if (hash == _curStateHash) return;
            _curStateHash = hash;
            animator.Play(hash, 0, 0f);
        }
    }
}
