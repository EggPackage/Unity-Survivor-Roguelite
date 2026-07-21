using UnityEngine;

namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 世界空间跟随血条（敌人头顶）。
    ///
    /// ==== 设计说明 ====
    /// 用两个 SpriteRenderer（底条 + 填充条）拼出血条，跟随挂载对象显示血量。
    /// 不走 UGUI Canvas，直接用世界空间 Sprite，性能好、和像素素材风格一致
    /// （占位用 Tiny Swords 的 SmallBar_Base / SmallBar_Fill）。
    ///
    /// 填充用"缩放 + 左对齐位移"实现：fill 的 localScale.x 按血量比例收缩，
    /// 同时左移半个收缩量，让血条从右往左减、左端固定（常见血条观感）。
    ///
    /// 监听同物体（或父物体）的 Health.OnDamaged 刷新，无需每帧轮询。
    /// 满血时可选隐藏，减少画面干扰。对象池复用时 Health.OnEnable 会重置血量，
    /// 本组件在 OnEnable 重新订阅并刷新到满。
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("要显示的 Health。留空则向上级 GetComponentInParent 查找")]
        [SerializeField] private Health health;
        [Tooltip("填充条 SpriteRenderer（会被缩放）")]
        [SerializeField] private SpriteRenderer fill;

        [Header("表现")]
        [Tooltip("满血时隐藏血条")]
        [SerializeField] private bool hideWhenFull = true;
        private float _fillBaseScaleX;   // 填充条初始 x 缩放
        private float _fillBaseLocalX;   // 填充条初始 x 局部坐标（左端锚点参考）

        private void Awake()
        {
            if (health == null) health = GetComponentInParent<Health>();
            if (fill != null)
            {
                _fillBaseScaleX = fill.transform.localScale.x;
                _fillBaseLocalX = fill.transform.localPosition.x;
            }
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.OnDamaged += OnHealthChanged;
                health.OnDeath += OnDeathHide;
            }
            RefreshImmediate();
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnDamaged -= OnHealthChanged;
                health.OnDeath -= OnDeathHide;
            }
        }

        private void OnHealthChanged(Damage _) => RefreshImmediate();

        private void OnDeathHide()
        {
            // 不 SetActive(false)：血条子物体被显式失活后，敌人从池复用时不会自动恢复。
            // 只清空填充并隐藏 fill renderer，OnEnable 时 RefreshImmediate 会重新显示。
            if (fill != null)
            {
                SetFillRatio(0f);
                fill.enabled = false;
            }
        }

        /// <summary>
        /// 立即按当前血量刷新填充与显隐。
        /// </summary>
        public void RefreshImmediate()
        {
            if (health == null || fill == null) return;

            float ratio = health.MaxHP > 0 ? (float)health.CurrentHP / health.MaxHP : 0f;
            SetFillRatio(ratio);

            // 满血且开启 hideWhenFull 时隐藏，否则显示（也负责复用时从死亡态恢复显示）
            bool full = ratio >= 0.999f;
            fill.enabled = !(hideWhenFull && full);
        }

        /// <summary>
        /// 设置填充比例（0~1）：缩放 x + 左对齐位移。
        /// </summary>
        private void SetFillRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            Vector3 s = fill.transform.localScale;
            s.x = _fillBaseScaleX * ratio;
            fill.transform.localScale = s;

            // 左对齐：整条从中心缩放会两端一起收，这里补偿使左端固定
            Vector3 p = fill.transform.localPosition;
            float shrink = _fillBaseScaleX * (1f - ratio) * 0.5f;
            p.x = _fillBaseLocalX - shrink;
            fill.transform.localPosition = p;
        }
    }
}
