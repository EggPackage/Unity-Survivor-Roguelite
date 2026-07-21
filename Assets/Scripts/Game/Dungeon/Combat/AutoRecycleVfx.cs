using UnityEngine;

namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 一次性特效自动回收：激活后固定时长回收进对象池。
    ///
    /// ==== 设计说明 ====
    /// 命中特效（Tiny Swords 的 Explosion 动画）从 PoolMgr 取出后需要自己回家。
    /// 用固定时长而非监听动画结束，是因为占位动画帧数/帧率不固定，
    /// 固定时长最省心，将来换正式特效再按需改。
    /// 预设须挂 PoolObj，且 gameObject.name 不能被改（PoolMgr 靠名字找抽屉）。
    /// </summary>
    public class AutoRecycleVfx : MonoBehaviour
    {
        [Tooltip("存活时长（秒），到点回收进池")]
        [SerializeField] private float lifeTime = 0.5f;

        private float _timer;

        private void OnEnable()
        {
            _timer = lifeTime;
        }

        private void Update()
        {
            // 用 unscaled，命中顿帧（timeScale=0）时特效计时不被冻住
            _timer -= Time.unscaledDeltaTime;
            if (_timer <= 0f)
                PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
