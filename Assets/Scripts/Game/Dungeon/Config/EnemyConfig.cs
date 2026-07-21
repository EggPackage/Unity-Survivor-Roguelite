using UnityEngine;

namespace Game.Dungeon.Config
{
    /// <summary>
    /// 敌人数值配置（ScriptableObject）。
    ///
    /// ==== 设计说明 ====
    /// 与 Bar 模块的 ItemConfig 一致，走数据驱动：数值放 .asset，代码只读不写死。
    /// 原型阶段 Enemy 若拿不到配置，会用自身 Inspector 上的默认值兜底（见 Enemy.cs），
    /// 所以没有 .asset 也能跑，符合项目"加载不到 SO 用内置回退"的约定。
    /// 菜单归到"早咖晚酒"，与现有 ItemConfig 同一命名空间层级。
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "早咖晚酒/敌人配置")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("身份")]
        [Tooltip("敌人标识 id")]
        public string enemyId = "obsession_grunt";
        [Tooltip("显示名（执念怪物）")]
        public string displayName = "执念杂兵";

        [Header("数值")]
        [Tooltip("最大血量")]
        public int maxHP = 30;
        [Tooltip("移动速度（单位/秒）")]
        public float moveSpeed = 2f;
        [Tooltip("接触玩家造成的伤害")]
        public int contactDamage = 10;
        [Tooltip("两次接触伤害的最小间隔（秒）")]
        public float attackInterval = 1f;
    }
}
