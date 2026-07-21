using UnityEngine;

namespace Game.Dungeon.Player
{
    /// <summary>
    /// Buff 影响的属性类型。
    /// </summary>
    public enum E_StatType
    {
        /// <summary>最大生命</summary>
        MaxHP,
        /// <summary>移动速度</summary>
        MoveSpeed,
        /// <summary>攻击力</summary>
        AttackPower,
        /// <summary>冲刺冷却</summary>
        DashCooldown,
    }

    /// <summary>
    /// 玩家核心属性容器。
    ///
    /// ==== 设计说明 ====
    /// 对应 GDD"饮品即 Build"：白天在咖啡厅调配的饮品，会在进地牢前作为
    /// 增益修正加到这里（移速 +30%、暴击 +15% 等）。因此属性拆成
    /// "基础值 baseXxx" + "运行时最终值 Xxx"，ApplyBuff 只改最终值，
    /// 关卡结束 ResetToBase 还原，基础值永远不被 Buff 污染。
    ///
    /// 原型阶段 ApplyBuff 已能工作（加法/乘法修正），但真正的数据来源
    /// （Bar 模块的 ComboResult）尚未接入，接入点在文件末尾注释标注。
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("基础属性（不被 Buff 污染）")]
        [Tooltip("最大生命值")]
        [SerializeField] private int baseMaxHP = 100;
        [Tooltip("移动速度（单位/秒）")]
        [SerializeField] private float baseMoveSpeed = 5f;
        [Tooltip("攻击力")]
        [SerializeField] private int baseAttackPower = 10;
        [Tooltip("冲刺冷却（秒）")]
        [SerializeField] private float baseDashCooldown = 1f;

        /// <summary>当前最大生命值（含 Buff）</summary>
        public int MaxHP { get; private set; }
        /// <summary>当前移动速度（含 Buff）</summary>
        public float MoveSpeed { get; private set; }
        /// <summary>当前攻击力（含 Buff）</summary>
        public int AttackPower { get; private set; }
        /// <summary>当前冲刺冷却（含 Buff）</summary>
        public float DashCooldown { get; private set; }

        private void Awake()
        {
            ResetToBase();
        }

        /// <summary>
        /// 把最终属性还原为基础值。进地牢前先还原，再逐条 ApplyBuff。
        /// </summary>
        public void ResetToBase()
        {
            MaxHP = baseMaxHP;
            MoveSpeed = baseMoveSpeed;
            AttackPower = baseAttackPower;
            DashCooldown = baseDashCooldown;
        }

        /// <summary>
        /// 施加一条 Buff 修正（加法 + 乘法）。最终值 = (基础对应项已在 Reset 时写入) 后累加。
        ///
        /// 用法：先 ResetToBase()，再对每条饮品效果调用 ApplyBuff。
        /// 例："冰美式" → ApplyBuff(MoveSpeed, mul:0.30f)（移速 +30%）。
        /// </summary>
        /// <param name="stat">影响的属性</param>
        /// <param name="add">加法修正（绝对值），默认 0</param>
        /// <param name="mul">乘法修正（比例，0.3 表示 +30%），默认 0</param>
        public void ApplyBuff(E_StatType stat, float add = 0f, float mul = 0f)
        {
            switch (stat)
            {
                case E_StatType.MaxHP:
                    MaxHP = Mathf.RoundToInt((MaxHP + add) * (1f + mul));
                    break;
                case E_StatType.MoveSpeed:
                    MoveSpeed = (MoveSpeed + add) * (1f + mul);
                    break;
                case E_StatType.AttackPower:
                    AttackPower = Mathf.RoundToInt((AttackPower + add) * (1f + mul));
                    break;
                case E_StatType.DashCooldown:
                    // 冷却类：乘法按"缩短"处理更直觉，这里仍用统一公式，传负 mul 表示缩短
                    DashCooldown = Mathf.Max(0f, (DashCooldown + add) * (1f + mul));
                    break;
            }
        }

        // TODO(Build 联动)：将来在进入地牢前，从 Bar 模块的 ComboResult 读取当日饮品效果，
        // 遍历转换成若干次 ApplyBuff 调用。ComboResult 已带 totalAttack / totalDefense 等字段
        // （见 Assets/Scripts/Game/Bar/ComboRule.cs），届时在此对接，无需改动本类结构。
    }
}
