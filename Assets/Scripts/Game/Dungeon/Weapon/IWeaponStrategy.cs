using UnityEngine;
using Game.Dungeon.Config;

namespace Game.Dungeon.Weapon
{
    /// <summary>
    /// 武器开火策略（策略模式）。
    ///
    /// M1 只有近战挥砍一种实现（MeleeSwing）。M3 加多种武器时，新增一个实现类
    /// 即可，WeaponRunner 与所有业务代码都不用改——这就是规范②"数据驱动 + 策略"
    /// 想达到的扩展性。
    /// </summary>
    public interface IWeaponStrategy
    {
        /// <summary>
        /// 执行一次开火。
        /// </summary>
        /// <param name="owner">武器持有者（玩家根节点）</param>
        /// <param name="cfg">武器配置（间隔之外的判定/伤害参数都在这里）</param>
        void Fire(Transform owner, WeaponConfig cfg);
    }
}
