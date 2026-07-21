using UnityEngine;

namespace Game.Dungeon.Enemy
{
    /// <summary>
    /// 简单刷怪器：定时在玩家周围生成敌人，维持场上数量上限。
    ///
    /// ==== 设计说明 ====
    /// 原型阶段的刷怪只求"持续有怪可打"，不做波次 / 精英 / Boss。
    /// 敌人从 PoolMgr 取（预设在 Resources 下、挂 PoolObj），死亡后自行回收，
    /// 这里只管"数量不足就补"。计数不自己维护死亡回调，而是每次刷怪前用
    /// 池的使用中数量判断——但 PoolMgr 未暴露该计数，故用最简单可靠的方式：
    /// 记录已生成、由敌人回收后场上活跃数下降，靠 maxAlive 限制在场峰值
    /// （PoolObj.maxNum 是硬上限，这里的 maxAlive 应 ≤ 它）。
    ///
    /// 生成位置：在玩家周围环形随机，避免糊脸；无玩家时用自身位置。
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("刷怪")]
        [Tooltip("对象池预设名（Resources 下、挂 PoolObj）")]
        [SerializeField] private string enemyResName = "Enemy";
        [Tooltip("刷怪间隔（秒）")]
        [SerializeField] private float spawnInterval = 1.5f;
        [Tooltip("场上同时存活上限（应 ≤ 预设 PoolObj.maxNum）")]
        [SerializeField] private int maxAlive = 8;

        [Header("生成位置")]
        [Tooltip("在玩家周围环形随机生成的最小 / 最大半径")]
        [SerializeField] private float minRadius = 4f;
        [SerializeField] private float maxRadius = 7f;

        private float _timer;
        private Transform _player;

        // 已生成且尚未回收的敌人，用于统计场上活跃数
        private readonly System.Collections.Generic.List<GameObject> _alive
            = new System.Collections.Generic.List<GameObject>();

        private void Start()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
            _timer = spawnInterval;
        }

        private void Update()
        {
            // 清理已被池回收（失活）的引用，得到真实活跃数
            for (int i = _alive.Count - 1; i >= 0; i--)
            {
                if (_alive[i] == null || !_alive[i].activeInHierarchy)
                    _alive.RemoveAt(i);
            }

            if (_alive.Count >= maxAlive) return;

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            _timer = spawnInterval;

            Spawn();
        }

        private void Spawn()
        {
            GameObject enemy = PoolMgr.Instance.GetObj(enemyResName);
            if (enemy == null) return;

            enemy.transform.position = GetSpawnPos();
            _alive.Add(enemy);
        }

        /// <summary>
        /// 玩家周围环形随机点；无玩家用自身位置。
        /// </summary>
        private Vector3 GetSpawnPos()
        {
            Vector3 center = _player != null ? _player.position : transform.position;
            float angle = Random.value * Mathf.PI * 2f;
            float radius = Random.Range(minRadius, maxRadius);
            return center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }
    }
}
