using System;
using UnityEngine;

namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 通用血量组件，玩家与敌人共用。
    ///
    /// ==== 设计说明 ====
    /// 把"扣血 / 死亡"从具体的 Player、Enemy 逻辑里剥离出来，做成可复用组件：
    /// 谁需要能被打，就挂一个 Health。攻击方只认 Health，不关心对面是玩家还是怪，
    /// 这样近战判定、陷阱、AOE 都能统一走 TakeDamage。
    /// 死亡不在这里写具体表现（回收 / 播死亡动画），而是抛 OnDeath 回调 +
    /// 广播事件，由 Enemy / Player 各自决定后续，保持组件与业务解耦。
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Header("血量")]
        [Tooltip("最大血量")]
        [SerializeField] private int maxHP = 30;

        [Header("无敌帧")]
        [Tooltip("受击后的无敌时长（秒），0 表示无无敌帧。玩家建议 0.5")]
        [SerializeField] private float invulnerableTime = 0f;

        private float _invulnerableUntil;

        /// <summary>当前血量</summary>
        public int CurrentHP { get; private set; }

        /// <summary>最大血量</summary>
        public int MaxHP => maxHP;

        /// <summary>是否已死亡</summary>
        public bool IsDead { get; private set; }

        /// <summary>受击回调 —— 参数：本次 Damage。用于受击闪白、击退、飘字</summary>
        public event Action<Damage> OnDamaged;

        /// <summary>死亡回调。Enemy 用它回收进池，Player 用它触发失败流程</summary>
        public event Action OnDeath;

        private void OnEnable()
        {
            // 从对象池取出复用时会重新 OnEnable，这里重置状态，保证复用干净
            ResetHealth();
        }

        /// <summary>
        /// 重置血量到满值并复活。对象池复用、关卡重开时调用。
        /// </summary>
        public void ResetHealth()
        {
            CurrentHP = maxHP;
            IsDead = false;
            _invulnerableUntil = 0f;
        }

        /// <summary>
        /// 运行时设置最大血量（例如敌人配置驱动），并回满。
        /// </summary>
        public void SetMaxHP(int value)
        {
            maxHP = Mathf.Max(1, value);
            ResetHealth();
        }

        /// <summary>运行时设置无敌时长（玩家侧由 DungeonPlayerController 配 0.5s）。</summary>
        public void SetInvulnerableTime(float seconds)
        {
            invulnerableTime = Mathf.Max(0f, seconds);
        }

        /// <summary>
        /// 承受一次伤害。已死亡则忽略。
        /// </summary>
        public void TakeDamage(Damage damage)
        {
            if (IsDead) return;
            if (Time.time < _invulnerableUntil) return;   // 无敌期：忽略本次伤害

            CurrentHP -= Mathf.Max(0, damage.amount);
            OnDamaged?.Invoke(damage);

            // 受击后进入无敌帧（0 表示无）
            if (invulnerableTime > 0f)
                _invulnerableUntil = Time.time + invulnerableTime;

            if (CurrentHP <= 0)
            {
                CurrentHP = 0;
                IsDead = true;
                OnDeath?.Invoke();
            }
        }
    }
}
