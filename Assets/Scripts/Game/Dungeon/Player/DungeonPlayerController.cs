using UnityEngine;
using Game.Dungeon.Combat;

namespace Game.Dungeon.Player
{
    /// <summary>
    /// 玩家状态。
    /// </summary>
    public enum E_PlayerState
    {
        /// <summary>待机</summary>
        Idle,
        /// <summary>移动</summary>
        Move,
        /// <summary>攻击（攻击中锁移动）</summary>
        Attack,
    }

    /// <summary>
    /// 地牢玩家控制器：轻量 FSM + 移动 + 面向鼠标翻转 + 攻击触发。
    ///
    /// ==== 设计说明 ====
    /// GDD 下午地牢：顶视角 3/4 动作砍杀。用一个 enum + switch 的轻量状态机
    /// （项目没有 FSM 基类，Bar 模块也是直接判断，保持一致，不引重型框架）。
    ///
    /// 输入：复用项目自研 InputMgr（旧版 Input）。
    ///   - 移动：InputMgr 每帧广播 E_Input_Horizontal / E_Input_Vertical(float)，
    ///     本类监听并缓存输入向量，在 FixedUpdate 里用 Rigidbody2D.MovePosition 做
    ///     物理位移（不直接改 transform，避免穿墙 / 与碰撞打架）。
    ///   - 攻击：把 E_Input_Skill1 映射到鼠标左键，监听其无参事件进入 Attack。
    ///
    /// 朝向：Tiny Swords 是横版精灵，只区分左右。每帧看鼠标在玩家哪一侧，
    ///   翻转 Visual 子节点的 SpriteRenderer.flipX。只翻 Visual，不翻根节点，
    ///   这样刚体、碰撞体、攻击判定原点都不受影响。
    ///
    /// 与现有 PlayerController2D 的区别：那个是白天 demo 用旧版 transform 位移 + 8 方向
    /// BlendTree；本类是下午地牢用的物理驱动 + 横版翻转，两者独立互不影响。
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

        [Header("攻击")]
        [Tooltip("一次攻击的持续时间（秒），期间锁移动")]
        [SerializeField] private float attackDuration = 0.35f;

        [Header("动画状态名（直接播放，不依赖 controller 参数）")]
        [Tooltip("待机动画状态名。占位素材：Warrior_Idle_Blue")]
        [SerializeField] private string idleStateName = "Warrior_Idle_Blue";
        [Tooltip("移动动画状态名。占位素材：Warrior_Run_Blue")]
        [SerializeField] private string runStateName = "Warrior_Run_Blue";
        [Tooltip("攻击动画状态名。占位素材：Warrior_Attack_Blue")]
        [SerializeField] private string attackStateName = "Warrior_Attack_Blue";

        private Rigidbody2D _rb;
        private PlayerStats _stats;
        private PlayerAttack _attack;

        private E_PlayerState _state = E_PlayerState.Idle;
        private Vector2 _moveInput;         // 缓存的输入向量（-1~1）
        private bool _facingRight = true;   // 当前朝向
        private float _attackTimer;         // 攻击剩余锁定时间

        private int _curStateHash;          // 当前正在播放的动画状态哈希，避免重复 Play 打断自身

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
            _attack = GetComponent<PlayerAttack>();

            if (visual == null) visual = transform.Find("Visual");
            if (visual != null)
            {
                if (visualRenderer == null) visualRenderer = visual.GetComponent<SpriteRenderer>();
                if (animator == null) animator = visual.GetComponent<Animator>();
            }

            // 顶视角，无重力
            _rb.gravityScale = 0f;
        }

        /// <summary>
        /// 直接播放指定动画状态（不经 controller 参数 / transition）。
        ///
        /// ==== 设计说明 ====
        /// 占位 controller（Tiny Swords）没有任何参数与状态切换连线，只是把各段动画
        /// 摆成独立状态。所以这里用 Animator.Play 按状态名直接切，状态名做成可配置字段，
        /// 换正式 controller 时改 Inspector 即可，无需动代码。
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

        private void OnEnable()
        {
            InputMgr.Instance.StartOrCloseInputMgr(true);
            // 攻击键映射到键盘 J，按下触发无参 E_Input_Skill1
            InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Skill1, KeyCode.J, InputInfo.E_InputType.Down);

            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, OnInputHorizontal);
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Vertical, OnInputVertical);
            EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill1, OnAttackPressed);
        }

        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Horizontal, OnInputHorizontal);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Vertical, OnInputVertical);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Skill1, OnAttackPressed);
        }

        private void OnInputHorizontal(float h) => _moveInput.x = h;
        private void OnInputVertical(float v) => _moveInput.y = v;

        /// <summary>
        /// 攻击键按下：非攻击状态才可触发，进入 Attack 并执行一次挥砍。
        /// </summary>
        private void OnAttackPressed()
        {
            if (_state == E_PlayerState.Attack) return;

            _state = E_PlayerState.Attack;
            _attackTimer = attackDuration;

            PlayState(attackStateName);
            if (_attack != null) _attack.DoAttack(_facingRight);
        }

        private void Update()
        {
            UpdateFacing();
            UpdateState();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            // 攻击中锁移动，制造挥砍的"扎马步"顿感
            if (_state == E_PlayerState.Attack)
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            float speed = _stats != null ? _stats.MoveSpeed : 5f;
            Vector2 dir = _moveInput.sqrMagnitude > 1f ? _moveInput.normalized : _moveInput;
            _rb.MovePosition(_rb.position + dir * speed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// 根据水平输入的方向翻转 Visual：向右走朝右，向左走朝左，不动保持。
        /// 只翻显示层，不动根节点。
        /// </summary>
        private void UpdateFacing()
        {
            // 水平输入的 x 分量 > 0 表示向右，< 0 向左；=0 保持原朝向避免抖动
            if (_moveInput.x > 0.01f) _facingRight = true;
            else if (_moveInput.x < -0.01f) _facingRight = false;

            if (visualRenderer != null)
                visualRenderer.flipX = !_facingRight; // 素材默认朝右，朝左才翻
        }

        /// <summary>
        /// 状态推进：Attack 计时结束后按是否有输入回到 Move / Idle。
        /// </summary>
        private void UpdateState()
        {
            if (_state == E_PlayerState.Attack)
            {
                _attackTimer -= Time.deltaTime;
                if (_attackTimer > 0f) return;
                _state = E_PlayerState.Idle; // 落回 Idle，下面按输入再切 Move
            }

            _state = _moveInput.sqrMagnitude > 0.001f ? E_PlayerState.Move : E_PlayerState.Idle;
        }

        private void UpdateAnimator()
        {
            // 攻击状态的动画在 OnAttackPressed 时已播放，这里只管 Idle / Move 两态
            if (_state == E_PlayerState.Attack) return;
            PlayState(_state == E_PlayerState.Move ? runStateName : idleStateName);
        }
    }
}