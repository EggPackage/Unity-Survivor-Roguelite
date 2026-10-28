using System.Collections.Generic;
using UnityEngine;

namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 场上角色注册表：玩家 Transform + 存活敌人列表。
    ///
    /// ==== 设计说明 ====
    /// 替代每帧 FindGameObjectWithTag / FindObjectsOfType（规范⑦ GC 三禁）。
    /// 原 Enemy.AcquirePlayer 每帧 FindGameObjectWithTag、武器选目标想用
    /// FindObjectsOfType，M4 千怪同屏时都是明确开销点。改成本注册表：
    ///   - 玩家在 DungeonPlayerController.OnEnable 注册；
    ///   - 敌人在 Enemy.OnEnable 注册、OnDisable 注销（池复用天然成对）。
    /// 静态持有健康引用，查最近敌人是纯内存遍历，零分配、零场景扫描。
    ///
    /// 注意：注册表是静态的，跨场景存活。对象销毁/回池时务必在 OnDisable 注销，
    /// 否则会留下悬空引用（GetNearestEnemyDirection 会跳过 null/dead 兜底）。
    /// </summary>
    public static class ActorRegistry
    {
        /// <summary>当前玩家 Transform（可能为 null）</summary>
        public static Transform Player { get; private set; }

        private static readonly List<Health> _enemies = new List<Health>();

        /// <summary>存活敌人列表（只读，供调试/扩展）</summary>
        public static IReadOnlyList<Health> Enemies => _enemies;

        public static void RegisterPlayer(Transform player)
        {
            Player = player;
        }

        public static void UnregisterPlayer(Transform player)
        {
            if (Player == player) Player = null;
        }

        public static void RegisterEnemy(Health health)
        {
            if (health != null && !_enemies.Contains(health)) _enemies.Add(health);
        }

        public static void UnregisterEnemy(Health health)
        {
            if (health != null) _enemies.Remove(health);
        }

        /// <summary>
        /// 最近存活敌人到 from 的距离平方；没有存活敌人返回 float.MaxValue。
        /// 用平方值比较，省掉每次开方（判定只需要 &lt;= reach*reach）。
        /// 供近战武器判断"目标是否已经进入可及范围"，避免对着远处的敌人空挥。
        /// </summary>
        public static float GetNearestEnemySqrDistance(Vector2 from)
        {
            float minSqr = float.MaxValue;

            for (int i = 0; i < _enemies.Count; i++)
            {
                Health h = _enemies[i];
                if (h == null || h.IsDead) continue;

                float sqr = ((Vector2)h.transform.position - from).sqrMagnitude;
                if (sqr < minSqr) minSqr = sqr;
            }
            return minSqr;
        }

        /// <summary>
        /// 最近存活敌人的方向（单位向量）；没有存活敌人返回 Vector2.zero。
        /// 用于武器决定朝哪挥砍，避免 FindObjectsOfType。
        /// </summary>
        public static Vector2 GetNearestEnemyDirection(Vector2 from)
        {
            Vector2 best = Vector2.zero;
            float minSqr = float.MaxValue;

            for (int i = 0; i < _enemies.Count; i++)
            {
                Health h = _enemies[i];
                if (h == null || h.IsDead) continue;

                Vector2 p = h.transform.position;
                float sqr = (p - from).sqrMagnitude;
                if (sqr < minSqr)
                {
                    minSqr = sqr;
                    best = (p - from).normalized;
                }
            }
            return best;
        }

        /// <summary>清空（测试/特殊重载时可用；正常重载靠 OnDisable 注销）。</summary>
        public static void Clear()
        {
            Player = null;
            _enemies.Clear();
        }
    }
}
