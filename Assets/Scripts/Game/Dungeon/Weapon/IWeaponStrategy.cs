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
        /// 当前是否有可以打到的目标。没有就不开火，也不播攻击动作。
        ///
        /// 为什么要分开：定时器只知道"到点了"，不知道"够不够得着"。
        /// 两者的定义按武器类型不同——近战看目标是否进了挥砍范围，
        /// 远程可能只要敌人在场就射。所以放在策略里，由各武器自己回答。
        /// </summary>
        /// <param name="owner">武器持有者（玩家根节点）</param>
        /// <param name="cfg">武器配置</param>
        bool CanFire(Transform owner, WeaponConfig cfg);

        /// <summary>
        /// 执行一次开火。
        /// </summary>
        /// <param name="owner">武器持有者（玩家根节点）</param>
        /// <param name="cfg">武器配置（间隔之外的判定/伤害参数都在这里）</param>
        void Fire(Transform owner, WeaponConfig cfg);
    }
}
