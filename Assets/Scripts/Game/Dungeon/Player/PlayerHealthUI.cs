using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game.Dungeon.Combat;

namespace Game.Dungeon.Player
{
    /// <summary>
    /// 玩家屏幕血条 UI + 受击闪红 + 死亡处理。
    ///
    /// ==== 设计说明 ====
    /// 挂在屏幕空间 Canvas 下的血条根物体上。通过 fillImage(Image, type=Filled)的
    /// fillAmount 显示玩家血量比例。数据来源是玩家身上的 Health 组件。
    ///
    /// 事件驱动：既直接订阅玩家 Health 的 OnDamaged/OnDeath（拿到确切血量比例），
    /// 也兼容项目已有的 E_Player_Hit/E_Player_Dead 事件（Enemy.cs 命中玩家时会广播）。
    /// 优先用 Health 直连；找不到玩家 Health 时退化为纯事件驱动（无法显示精确比例则忽略）。
    ///
    /// 受击闪红：命中时给全屏红色遮罩（damageFlash Image）一个瞬时 alpha 再淡出，
    /// 是常见的低成本受击反馈。死亡：显示 deathPanel（结算/失败占位）。
    ///
    /// 玩家用 Tag=Player 查找，与 Enemy 一致。
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("血条")]
        [Tooltip("填充式血条 Image（Image Type = Filled, Horizontal）")]
        [SerializeField] private Image fillImage;

        [Header("受击闪红")]
        [Tooltip("全屏红色遮罩 Image（初始 alpha=0）")]
        [SerializeField] private Image damageFlash;
        [Tooltip("受击时红屏峰值 alpha")]
        [SerializeField] private float flashAlpha = 0.4f;
        [Tooltip("红屏淡出时长（秒）")]
        [SerializeField] private float flashFadeTime = 0.35f;

        [Header("死亡")]
        [Tooltip("死亡时显示的面板（结算/失败占位），可留空")]
        [SerializeField] private GameObject deathPanel;

        private Health _playerHealth;
        private Coroutine _flashCo;

        private void Start()
        {
            AcquirePlayerHealth();

            // 兼容事件驱动：即便没直连 Health，也能靠事件触发闪红/死亡
            EventCenter.Instance.AddEventListener<int>(E_EventType.E_Player_Hit, OnPlayerHitEvent);
            EventCenter.Instance.AddEventListener(E_EventType.E_Player_Dead, OnPlayerDeadEvent);

            if (deathPanel != null) deathPanel.SetActive(false);
            RefreshBar();
        }

        private void OnDestroy()
        {
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_Player_Hit, OnPlayerHitEvent);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Player_Dead, OnPlayerDeadEvent);

            if (_playerHealth != null)
            {
                _playerHealth.OnDamaged -= OnPlayerDamaged;
                _playerHealth.OnDeath -= OnPlayerDeath;
            }
        }

        /// <summary>
        /// 查找玩家 Health 并直连（拿精确血量）。找不到则仅靠事件工作。
        /// </summary>
        private void AcquirePlayerHealth()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            Health h = p.GetComponent<Health>();
            if (h == null) return;

            _playerHealth = h;
            _playerHealth.OnDamaged += OnPlayerDamaged;
            _playerHealth.OnDeath += OnPlayerDeath;
        }

        private void OnPlayerDamaged(Damage _) { RefreshBar(); Flash(); }
        private void OnPlayerDeath() => ShowDeath();

        // 事件版（当没直连 Health，或额外触发闪红）
        private void OnPlayerHitEvent(int dmg) { RefreshBar(); Flash(); }
        private void OnPlayerDeadEvent() => ShowDeath();

        /// <summary>
        /// 按玩家 Health 刷新血条比例。无直连 Health 时保持不变。
        /// </summary>
        private void RefreshBar()
        {
            if (fillImage == null || _playerHealth == null) return;
            fillImage.fillAmount = _playerHealth.MaxHP > 0
                ? (float)_playerHealth.CurrentHP / _playerHealth.MaxHP
                : 0f;
        }

        /// <summary>
        /// 受击闪红：峰值 alpha 后淡出。用 unscaledDeltaTime 兼容顿帧。
        /// </summary>
        private void Flash()
        {
            if (damageFlash == null) return;
            if (_flashCo != null) StopCoroutine(_flashCo);
            _flashCo = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            Color c = damageFlash.color;
            c.a = flashAlpha;
            damageFlash.color = c;

            float t = 0f;
            while (t < flashFadeTime)
            {
                t += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(flashAlpha, 0f, t / flashFadeTime);
                damageFlash.color = c;
                yield return null;
            }
            c.a = 0f;
            damageFlash.color = c;
            _flashCo = null;
        }

        private void ShowDeath()
        {
            RefreshBar();
            if (deathPanel != null) deathPanel.SetActive(true);
        }
    }
}
