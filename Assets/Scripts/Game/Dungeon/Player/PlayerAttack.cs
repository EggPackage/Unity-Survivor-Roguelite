using UnityEngine;
using Game.Dungeon.Combat;

namespace Game.Dungeon.Player
{
    /// <summary>
    /// 玩家近战挥砍判定（"拉花剑气"打蛋器重刃）。
    ///
    /// ⚠️ 已废弃（2026-10-28）：M1 改为自动攻击后，判定逻辑搬到
    /// `Game/Dungeon/Weapon/MeleeSwing.cs` 并由 `WeaponRunner` 定时驱动。
    /// 本组件已从玩家预制体移除，文件保留仅供对照阅读，不要再挂回去。
    ///
    /// ==== 设计说明 ====
    /// 对应 GDD 下午地牢的核心动作："挥舞出拉花剑气"。这里只负责一次挥砍的
    /// 命中判定与打击反馈，不管输入与状态机（那是 DungeonPlayerController 的事）。
    /// 判定用 Physics2D.OverlapCircle 在朝向前方画一个圆，命中敌人层的所有 Health
    /// 各扣一次攻击力，并触发顿帧 + 震屏 + 命中特效（从对象池取）。
    ///
    /// 攻击力实时读 PlayerStats.AttackPower，所以饮品 Buff 生效后攻击自动变强。
    /// 命中特效走 PoolMgr（预设需在 Resources 下且挂 PoolObj），无特效资源时安全跳过。
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        [Header("判定")]
        [Tooltip("挥砍中心相对玩家的前方距离")]
        [SerializeField] private float attackRange = 0.9f;
        [Tooltip("挥砍判定半径")]
        [SerializeField] private float attackRadius = 0.7f;
        [Tooltip("可被击中的层（敌人层）")]
        [SerializeField] private LayerMask enemyMask;

        [Header("打击手感")]
        [Tooltip("命中顿帧时长（秒）")]
        [SerializeField] private float hitStopDuration = 0.06f;
        [Tooltip("命中屏幕震动强度")]
        [SerializeField] private float shakeIntensity = 1f;
        [Tooltip("击退力度")]
        [SerializeField] private float knockbackForce = 4f;

        [Header("命中特效")]
        [Tooltip("Resources 下命中特效预设名（挂 PoolObj），留空则不生成")]
        [SerializeField] private string hitVfxResName = "DungeonHitVfx";

        private PlayerStats _stats;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
        }

        /// <summary>
        /// 执行一次挥砍。由 DungeonPlayerController 在进入 Attack 状态时调用。
        /// </summary>
        /// <param name="facingRight">当前朝向是否朝右（决定判定圆在左还是右）</param>
        public void DoAttack(bool facingRight)
        {
            Vector2 origin = (Vector2)transform.position
                             + new Vector2(facingRight ? attackRange : -attackRange, 0f);

            int attackPower = _stats != null ? _stats.AttackPower : 10;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRadius, enemyMask);

            bool anyHit = false;
            foreach (Collider2D col in hits)
            {
                Health health = col.GetComponent<Health>();
                if (health == null || health.IsDead) continue;

                Vector2 dir = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;
                Damage dmg = new Damage(attackPower, col.transform.position, dir, knockbackForce);
                health.TakeDamage(dmg);

                EventCenter.Instance.EventTrigger(E_EventType.E_Enemy_Hit, attackPower);
                SpawnHitVfx(col.transform.position);
                anyHit = true;
            }

            // 命中才给顿帧 + 震屏，空挥不触发，避免手感发闷
            if (anyHit)
            {
                GameFeelManager.Instance.HitStop(hitStopDuration);
                GameFeelManager.Instance.ScreenShake(shakeIntensity);
            }
        }

        /// <summary>
        /// 从对象池取命中特效并摆到命中点。特效预设自身负责播完回收。
        ///
        /// 特效是锦上添花，用 try-catch 与核心战斗隔离：即便对象池出现边界问题
        /// （如跨 Play 周期的悬空引用），也只吞掉特效、不中断扣血 / 顿帧 / 震屏主逻辑。
        /// </summary>
        private void SpawnHitVfx(Vector2 pos)
        {
            if (string.IsNullOrEmpty(hitVfxResName)) return;

            try
            {
                GameObject vfx = PoolMgr.Instance.GetObj(hitVfxResName);
                if (vfx != null)
                    vfx.transform.position = pos;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PlayerAttack] 生成命中特效失败（不影响战斗）：{ex.Message}");
            }
        }

        // 在 Scene 视图画出攻击判定范围，方便调参（朝右示意）
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 origin = (Vector2)transform.position + new Vector2(attackRange, 0f);
            Gizmos.DrawWireSphere(origin, attackRadius);
        }
    }
}
