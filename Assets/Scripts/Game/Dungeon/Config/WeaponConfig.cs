using UnityEngine;

namespace Game.Dungeon.Config
{
    /// <summary>
    /// 武器数值配置（ScriptableObject）。
    ///
    /// ==== 设计说明 ====
    /// 照 EnemyConfig 的模板：数值放 .asset，代码只读不写死（规范②）。
    /// 攻击间隔 / 判定范围 / 伤害倍率全部外置，M3 做多种武器时新增一个 .asset 即可，
    /// 战斗核心（WeaponRunner / MeleeSwing）一行不用改。
    /// 没有 .asset 时 WeaponRunner 用自身 Inspector 默认值兜底（同 Enemy 的约定）。
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "早咖晚酒/武器配置")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("身份")]
        [Tooltip("武器标识 id")]
        public string weaponId = "ladle_swing";
        [Tooltip("显示名（打蛋器重刃）")]
        public string displayName = "打蛋器重刃";

        [Header("节奏")]
        [Tooltip("两次攻击的间隔（秒）——自动攻击全靠它，改这里即时生效")]
        public float interval = 0.8f;

        [Header("判定")]
        [Tooltip("判定圆相对玩家的前方偏移（沿攻击方向）")]
        public float range = 0.9f;
        [Tooltip("判定圆半径")]
        public float radius = 0.7f;
        [Tooltip("伤害倍率（乘玩家攻击力）")]
        public float damageMul = 1f;
        [Tooltip("击退力度")]
        public float knockbackForce = 4f;

        [Header("打击手感")]
        [Tooltip("命中顿帧时长（秒）")]
        public float hitStopDuration = 0.06f;
        [Tooltip("命中屏幕震动强度")]
        public float shakeIntensity = 1f;

        [Header("命中特效")]
        [Tooltip("Resources 下命中特效预设名（挂 PoolObj），留空则不生成")]
        public string hitVfxResName = "DungeonHitVfx";
    }
}
