using UnityEngine;

namespace Game.Dungeon
{
    /// <summary>
    /// 地牢场景引导器：进入场景时做一次性初始化。
    ///
    /// ==== 设计说明 ====
    /// 项目的 PoolMgr 是纯 C# 单例（BaseManager），不随 Play 模式停止而清空。
    /// 在编辑器里反复 进入/退出 Play，或从别的场景切进地牢时，池字典里可能残留
    /// 上一轮已被 Unity 销毁的 GameObject 引用；下次 GetObj 复用到它就会抛
    /// MissingReferenceException。项目本身的约定就是"切场景时 ClearPool"，
    /// 这里在地牢场景入口统一做这件事，保证每次进地牢都是干净的池。
    ///
    /// 放在最早执行（脚本执行顺序设为较小值，或靠 Awake 先于其它 Start）。
    /// </summary>
    public class DungeonBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // 兜底复位时间缩放：上一局结算冻结（timeScale=0）或命中顿帧残留时，
            // 重载场景后若不复位，新场景一出生就是冻结的、动不了。放在最前面。
            Time.timeScale = 1f;

            // 清空对象池，丢弃跨 Play 周期 / 跨场景的悬空引用
            PoolMgr.Instance.ClearPool();
        }
    }
}
