using UnityEngine;

namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 一次伤害的数据描述。
    ///
    /// ==== 设计说明 ====
    /// 用结构体而不是裸 int 传伤害，是为了给"拉花剑气"这类肉鸽攻击留扩展空间：
    /// 后续可以往里加暴击、击退方向、元素类型（对应 GDD 的火/冰属性 Build），
    /// 而不用改动 Health.TakeDamage 的签名。原型阶段只用 amount + 命中点 + 击退。
    /// </summary>
    public struct Damage
    {
        /// <summary>伤害数值（已结算好，Health 直接扣）</summary>
        public int amount;

        /// <summary>命中世界坐标，用于生成命中特效 / 伤害飘字的位置</summary>
        public Vector2 hitPoint;

        /// <summary>击退方向（单位向量，从攻击者指向被击者），零向量表示不击退</summary>
        public Vector2 knockbackDir;

        /// <summary>击退力度（0 表示不击退）</summary>
        public float knockbackForce;

        public Damage(int amount, Vector2 hitPoint, Vector2 knockbackDir = default, float knockbackForce = 0f)
        {
            this.amount = amount;
            this.hitPoint = hitPoint;
            this.knockbackDir = knockbackDir;
            this.knockbackForce = knockbackForce;
        }
    }
}
