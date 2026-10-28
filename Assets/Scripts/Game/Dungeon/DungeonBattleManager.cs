using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game.Dungeon.Combat;
using Game.Dungeon.Enemy;
using Game.Dungeon.Input;

namespace Game.Dungeon
{
    /// <summary>
    /// 地牢战斗结算管理器：击杀计数 + 胜负判定 + 结算面板 + 重新开始。
    ///
    /// ==== 设计说明 ====
    /// 把"下午地牢"从"能打怪"变成"有始有终的一局"。负责：
    ///   - 击杀计数：监听敌人死亡广播的 E_Monster_Dead（无参）累加。
    ///   - 胜利判定：击杀数达到 killTarget → 弹胜利面板。
    ///   - 失败判定：玩家死亡 → 弹失败面板。
    ///
    /// 为什么直连玩家 Health.OnDeath 而不是听 E_Player_Dead：
    ///   全项目没有任何地方 EventTrigger(E_Player_Dead)，那个事件只有订阅方、没有发布方。
    ///   玩家死亡的唯一可靠信号是玩家身上 Health 组件的 OnDeath 回调，所以这里直连它。
    ///
    /// 为什么是场景挂载 MonoBehaviour 而非全局单例：
    ///   它要在 Inspector 里引用场景内的面板 / 文本 / 刷怪器。做成单例会跨场景存活、
    ///   持有已销毁场景对象的悬空引用。随场景挂载 + 重载天然重建，最省心。
    ///
    /// 结算面板用场景内普通 GameObject（SetActive 切换），不走 UIMgr——
    ///   UIMgr 是 AssetBundle 驱动的重管线，为两个原型面板搭它得不偿失。
    /// </summary>
    public class DungeonBattleManager : MonoBehaviour
    {
        [Header("胜利条件")]
        [Tooltip("达成多少击杀数判定胜利")]
        [SerializeField] private int killTarget = 20;

        [Header("结算面板（默认 inactive）")]
        [Tooltip("胜利面板")]
        [SerializeField] private GameObject victoryPanel;
        [Tooltip("失败面板")]
        [SerializeField] private GameObject defeatPanel;

        [Header("HUD")]
        [Tooltip("击杀计数文本，显示 击杀 X/N")]
        [SerializeField] private Text killCountText;

        [Header("引用")]
        [Tooltip("刷怪器，结算时停止刷怪。留空自动查找")]
        [SerializeField] private EnemySpawner spawner;

        [Header("行为")]
        [Tooltip("结算时是否冻结游戏（timeScale=0）")]
        [SerializeField] private bool freezeOnEnd = true;

        private int _kills;
        private bool _resolved;          // 已结算，防止重复触发
        private Health _playerHealth;    // 直连的玩家血量组件

        private void Start()
        {
            _kills = 0;
            _resolved = false;

            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (defeatPanel != null) defeatPanel.SetActive(false);
            RefreshKillText();

            // 击杀来源：敌人死亡广播 E_Monster_Dead
            EventCenter.Instance.AddEventListener(E_EventType.E_Monster_Dead, OnMonsterDead);

            // 失败来源：直连玩家 Health.OnDeath
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                _playerHealth = p.GetComponent<Health>();
                if (_playerHealth != null) _playerHealth.OnDeath += OnPlayerDeath;
            }

            if (spawner == null) spawner = FindObjectOfType<EnemySpawner>();

            // T5：R 键重开（结算后）。输入走 DungeonInput 意图层。
            DungeonInput.EnsureEnabled();
            DungeonInput.OnRestart += OnRestartPressed;
        }

        private void OnDestroy()
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Monster_Dead, OnMonsterDead);
            if (_playerHealth != null) _playerHealth.OnDeath -= OnPlayerDeath;
            DungeonInput.OnRestart -= OnRestartPressed;
        }

        /// <summary>
        /// 敌人死亡：累加击杀，达标则胜利结算。
        /// </summary>
        private void OnMonsterDead()
        {
            if (_resolved) return;
            _kills++;
            RefreshKillText();
            if (_kills >= killTarget) Resolve(true);
        }

        /// <summary>
        /// 玩家死亡：直接失败结算。
        /// </summary>
        private void OnPlayerDeath()
        {
            if (_resolved) return;
            Resolve(false);
        }

        /// <summary>
        /// R 键重开：只在已结算（死亡/胜利）后生效，避免中途误触重开。
        /// </summary>
        private void OnRestartPressed()
        {
            if (!_resolved) return;
            RestartBattle();
        }

        /// <summary>
        /// 结算：停刷怪、广播 BattleResult、弹对应面板、按需冻结。
        /// </summary>
        private void Resolve(bool isVictory)
        {
            _resolved = true;

            if (spawner != null) spawner.enabled = false;

            EventCenter.Instance.EventTrigger(
                E_EventType.E_Dungeon_BattleResolved,
                new BattleResult(isVictory, _kills, killTarget));

            GameObject panel = isVictory ? victoryPanel : defeatPanel;
            if (panel != null) panel.SetActive(true);

            // 冻结放最后：置 0 后本帧后续基于缩放时间的逻辑都会停，
            // 面板显隐用的是 SetActive（不受 timeScale 影响），所以安全。
            if (freezeOnEnd) Time.timeScale = 0f;
        }

        private void RefreshKillText()
        {
            if (killCountText != null)
                killCountText.text = $"击杀 {_kills}/{killTarget}";
        }

        /// <summary>
        /// 重新开始：重载当前场景。绑给胜利 / 失败面板的重开按钮 OnClick。
        ///
        /// 关键：必须先把 timeScale 复位为 1，否则重载后的新场景一出生就是冻结的。
        /// SceneMgr.LoadScene 按"场景名"加载，故 DungeonDemo 必须已注册进 Build Settings。
        /// 池的清理由 DungeonBootstrap.Awake 的 ClearPool 负责。
        /// </summary>
        public void RestartBattle()
        {
            Time.timeScale = 1f;
            SceneMgr.Instance.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
