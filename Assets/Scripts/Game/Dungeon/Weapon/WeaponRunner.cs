using UnityEngine;
using Game.Dungeon.Config;

namespace Game.Dungeon.Weapon
{
    /// <summary>
    /// 自动攻击驱动器：按 interval 定时调用武器策略开火。
    ///
    /// ==== 设计说明 ====
    /// M1 的核心新增就是"自动攻击"——站着不动也会按间隔挥砍，不需要攻击键
    /// （规范：自动攻击不配输入键，避免养成手动攻击的路径依赖）。
    ///
    /// 定时用 Time.deltaTime 累加（规范⑥：受 timeScale 影响，命中顿帧时会跟着停，
    /// 这正是想要的）。间隔/范围/倍率全部读 WeaponConfig(SO)，改 SO 即时生效；
    /// 没有 SO 时用自身 Inspector 默认值兜底（规范②，与 Enemy 一致）。
    ///
    /// 场景未手动挂本组件时，DungeonPlayerController.Awake 会自动补一个，
    /// 保证"打开 DungeonDemo 直接 Play 就能自动攻击"。
    /// </summary>
    public class WeaponRunner : MonoBehaviour
    {
        [Header("配置（留空用下方默认值兜底）")]
        [SerializeField] private WeaponConfig config;

        [Header("默认数值（无配置时生效）")]
        [SerializeField] private float defaultInterval = 0.8f;
        [SerializeField] private float defaultRange = 0.9f;
        [SerializeField] private float defaultRadius = 0.7f;
        [SerializeField] private float defaultDamageMul = 1f;
        [SerializeField] private float defaultKnockbackForce = 4f;

        [Header("判定层（留空自动取 Enemy 层）")]
        [SerializeField] private LayerMask enemyMask;

        private IWeaponStrategy _strategy;
        private WeaponConfig _fallback;   // 无 SO 时的一次性兜底配置，避免每次攻击 new
        private float _timer;

        private void Awake()
        {
            if (enemyMask.value == 0)
            {
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                if (enemyLayer >= 0) enemyMask = 1 << enemyLayer;
            }

            _strategy = new MeleeSwing(enemyMask);

            if (config == null)
            {
                _fallback = ScriptableObject.CreateInstance<WeaponConfig>();
                _fallback.interval = defaultInterval;
                _fallback.range = defaultRange;
                _fallback.radius = defaultRadius;
                _fallback.damageMul = defaultDamageMul;
                _fallback.knockbackForce = defaultKnockbackForce;
            }
        }

        private void OnEnable()
        {
            _timer = 0f;   // 复用时立刻从零计时
        }

        private void OnDestroy()
        {
            if (_fallback != null) Destroy(_fallback);
        }

        private void Update()
        {
            WeaponConfig cfg = config != null ? config : _fallback;
            if (cfg == null || cfg.interval <= 0f) return;

            _timer += Time.deltaTime;
            if (_timer < cfg.interval) return;

            _timer -= cfg.interval;      // 保留溢出，节奏稳定
            _strategy?.Fire(transform, cfg);
        }
    }
}
