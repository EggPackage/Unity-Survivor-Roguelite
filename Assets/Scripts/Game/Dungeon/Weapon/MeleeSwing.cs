using UnityEngine;
using Game.Dungeon.Combat;
using Game.Dungeon.Config;
using Game.Dungeon.Player;

namespace Game.Dungeon.Weapon
{
    /// <summary>
    /// 近战挥砍（M1 唯一武器实现）。
    ///
    /// ==== 设计说明 ====
    /// 复用 PlayerAttack 的判定思路（在玩家前方画圆命中敌人层），但做了三处升级，
    /// 全部对应规范⑦「GC 三禁」与 T3「目标从注册表取」：
    ///   1. OverlapCircleAll → OverlapCircleNonAlloc + 类级复用缓冲 _hits，攻击零 GC。
    ///   2. 方向不再依赖鼠标/朝向硬编码，而是优先指向 ActorRegistry 里最近的敌人；
    ///      没有敌人时才退化为玩家当前朝向（GetComponentInChildren 不做 Find）。
    ///   3. 命中后统一走 Health.TakeDamage（规范④），并触发顿帧/震屏/命中特效。
    ///
    /// 不负责：定时（那是 WeaponRunner 的事）、动画（M1 白盒不做攻击动画）。
    /// </summary>
    public class MeleeSwing : IWeaponStrategy
    {
        // 类级复用缓冲：避免每次攻击 new 数组（规范⑦）。32 足够覆盖 M1 同屏数量。
        private readonly Collider2D[] _hits = new Collider2D[32];
        private readonly LayerMask _enemyMask;

        public MeleeSwing(LayerMask enemyMask)
        {
            _enemyMask = enemyMask;
        }

        public void Fire(Transform owner, WeaponConfig cfg)
        {
            if (owner == null || cfg == null) return;

            PlayerStats stats = owner.GetComponent<PlayerStats>();
            int basePower = stats != null ? stats.AttackPower : 10;
            int damage = Mathf.Max(1, Mathf.RoundToInt(basePower * cfg.damageMul));

            // 方向：优先最近敌人（ActorRegistry），否则玩家朝向
            Vector2 dir = ActorRegistry.GetNearestEnemyDirection(owner.position);
            if (dir == Vector2.zero) dir = GetFacing(owner);

            Vector2 origin = (Vector2)owner.position + dir * cfg.range;

            // 零分配命中判定
            int count = Physics2D.OverlapCircleNonAlloc(origin, cfg.radius, _hits, _enemyMask);
            bool anyHit = false;

            for (int i = 0; i < count; i++)
            {
                Collider2D col = _hits[i];
                if (col == null) continue;

                Health health = col.GetComponent<Health>();
                if (health == null || health.IsDead) continue;

                Vector2 knockDir = ((Vector2)col.transform.position - (Vector2)owner.position).normalized;
                health.TakeDamage(new Damage(damage, col.transform.position, knockDir, cfg.knockbackForce));
                EventCenter.Instance.EventTrigger(E_EventType.E_Enemy_Hit, damage);
                SpawnHitVfx(cfg, col.transform.position);
                anyHit = true;
            }

            // 命中才给顿帧 + 震屏（空挥不触发，避免手感发闷）
            if (anyHit)
            {
                GameFeelManager.Instance.HitStop(cfg.hitStopDuration);
                GameFeelManager.Instance.ScreenShake(cfg.shakeIntensity);
            }
        }

        /// <summary>玩家当前朝向（横版只分左右）；根节点无 SpriteRenderer 时取子节点。</summary>
        private Vector2 GetFacing(Transform owner)
        {
            SpriteRenderer sr = owner.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sr.flipX) return Vector2.left;
            return Vector2.right;
        }

        /// <summary>从对象池取命中特效摆到命中点；特效异常不影响战斗主逻辑。</summary>
        private void SpawnHitVfx(WeaponConfig cfg, Vector2 pos)
        {
            if (string.IsNullOrEmpty(cfg.hitVfxResName)) return;
            try
            {
                GameObject vfx = PoolMgr.Instance.GetObj(cfg.hitVfxResName);
                if (vfx != null) vfx.transform.position = pos;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[MeleeSwing] 生成命中特效失败（不影响战斗）：{ex.Message}");
            }
        }
    }
}
