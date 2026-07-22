using UnityEngine;

namespace Game.Dungeon.Enemy
{
    /// <summary>
    /// 地牢刷怪器：定时在玩家周围生成敌人，维持场上数量上限。
    ///
    /// ==== 对象池约定 ====
    /// - 预设体必须放在 Assets/Resources/ 下（路径与 enemyResName 完全一致）
    /// - 预设体必须挂载 PoolObj 脚本，PoolObj.maxNum ≥ 本脚本 maxAlive
    /// - 本脚本通过 PoolMgr.Instance.GetObj(enemyResName) 取对象
    /// - 敌人死亡自行 PushObj 回池，本脚本只负责补充
    ///
    /// ==== 生成位置约定 ====
    /// 玩家周围环形随机 + 碰撞检测：
    ///   1. 生成点本身不能撞墙（OverlapCircle）
    ///   2. 从玩家到生成点之间不能有墙（Linecast）——敌人要能追过来
    ///   3. 撞墙就换角度重试；全部失败兜底放玩家身旁安全区
    ///
    /// ==== 攻击/朝向 ====
    /// - 玩家攻击键：键盘 J（DungeonPlayerController.OnEnable 里绑定）
    /// - 玩家朝向：根据水平输入自动翻转，不再依赖鼠标
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("对象池")]
        [Tooltip("对象池预设名（Resources 下路径 + 名字，必须与预设文件名完全一致）")]
        [SerializeField] private string enemyResName = "Enemy";

        [Header("刷怪节奏")]
        [Tooltip("刷怪间隔（秒）")]
        [SerializeField] private float spawnInterval = 3f;
        [Tooltip("场上同时存活上限（必须 ≤ 预设体上 PoolObj.maxNum）")]
        [SerializeField] private int maxAlive = 6;

        [Header("生成位置")]
        [Tooltip("生成环形的最小 / 最大半径（以玩家为中心）")]
        [SerializeField] private float minRadius = 3f;
        [SerializeField] private float maxRadius = 6f;
        [Tooltip("撞墙后的最大换角度重试次数")]
        [SerializeField] private int maxSpawnRetries = 12;
        [Tooltip("检测是否撞墙的小圆半径（≈ 敌人碰撞体大小）")]
        [SerializeField] private float blockCheckRadius = 0.5f;

        private float _timer;
        private Transform _player;
        private Rigidbody2D _playerRb;
        private int _enemyLayer = -1;

        // 已生成且尚未回收的敌人
        private readonly System.Collections.Generic.List<GameObject> _alive
            = new System.Collections.Generic.List<GameObject>();

        private void Start()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                _player = p.transform;
                _playerRb = p.GetComponent<Rigidbody2D>();
            }
            _enemyLayer = LayerMask.NameToLayer("Enemy");
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

        /// <summary>
        /// 从对象池取出一个敌人 → 重置状态 → 放到合法生成点。
        ///
        /// 严格按照 PoolMgr 约定：
        ///   - GetObj 失败（预设体缺失 / 没挂 PoolObj）直接 return，不报错崩溃
        ///   - 取出后立即 Reset 物理状态（position/rotation/velocity），
        ///     避免从上一次死亡位置"飘"过来
        /// </summary>
        private void Spawn()
        {
            // 1. 从对象池取
            GameObject enemy = PoolMgr.Instance.GetObj(enemyResName);
            if (enemy == null)
            {
                Debug.LogWarning($"[EnemySpawner] PoolMgr.GetObj(\"{enemyResName}\") 返回 null，" +
                                 $"检查预设是否在 Resources/ 下并且挂了 PoolObj");
                return;
            }

            // 2. 计算合法生成点
            Vector3 spawnPos = GetSpawnPos();

            // 3. 重置物理状态 + 放到生成点
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            enemy.transform.rotation = Quaternion.identity;
            enemy.transform.position = spawnPos;

            _alive.Add(enemy);
        }

        /// <summary>
        /// 玩家周围环形随机 + 双保险碰撞检测：
        ///   - 小圆检测：生成点不能直接撞墙
        ///   - 射线检测：玩家到生成点之间不能有墙（敌人要能走过来）
        /// 撞墙则换角度重试；全部失败 → 在玩家 2 格范围内兜底 → 终极兜底放玩家位置。
        /// </summary>
        private Vector3 GetSpawnPos()
        {
            Vector3 center = _player != null ? _player.position : transform.position;

            // 阶段 1：环形随机 + 双保险检测
            for (int i = 0; i < maxSpawnRetries; i++)
            {
                float angle = Random.value * Mathf.PI * 2f;
                float radius = Random.Range(minRadius, maxRadius);
                Vector3 pos = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f);

                if (IsSafeSpawnPoint(center, pos))
                    return pos;
            }

            // 阶段 2：兜底：在玩家 1.5~maxRadius 范围内以 12 等分布逐圈找
            for (float r = 1.5f; r <= maxRadius; r += 1f)
            {
                for (int i = 0; i < 12; i++)
                {
                    float angle = (i / 12f) * Mathf.PI * 2f;
                    Vector3 pos = center + new Vector3(
                        Mathf.Cos(angle) * r,
                        Mathf.Sin(angle) * r,
                        0f);

                    if (IsSafeSpawnPoint(center, pos))
                        return pos;
                }
            }

            // 终极兜底：玩家自身位置（玩家能站的地方肯定不撞墙）
            return center;
        }

        /// <summary>
        /// 双保险检测：生成点自身不撞墙 且 玩家到生成点之间无墙阻挡。
        /// </summary>
        private bool IsSafeSpawnPoint(Vector3 from, Vector3 to)
        {
            // 检测 1：生成点小圆不能撞墙（排除玩家和敌人自身）
            Collider2D[] hits = Physics2D.OverlapCircleAll(to, blockCheckRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D c = hits[i];
                if (c == null) continue;
                if (c.CompareTag("Player")) continue;
                if (_enemyLayer != -1 && c.gameObject.layer == _enemyLayer) continue;
                // 其他 collider（墙/地面/装饰）→ 挡住
                return false;
            }

            // 检测 2：玩家到生成点之间不能有墙（敌人要能直线走过来追击）
            // 从玩家稍微偏移一点，避免碰到玩家自身 collider
            Vector3 dir = (to - from).normalized;
            Vector3 start = from + dir * 0.3f;
            float dist = Vector3.Distance(start, to);
            if (dist > 0.1f)
            {
                RaycastHit2D hit = Physics2D.Raycast(start, dir, dist);
                if (hit.collider != null)
                {
                    // 碰到了玩家或敌人，不视为阻挡；碰到其他就是墙
                    if (!hit.collider.CompareTag("Player") &&
                        (_enemyLayer == -1 || hit.collider.gameObject.layer != _enemyLayer))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}