using System.Collections;
using UnityEngine;

namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 打击手感管理器：命中顿帧（HitStop）+ 屏幕震动（ScreenShake）。
    ///
    /// ==== 设计说明 ====
    /// 对应 GDD 下午地牢"硬核反馈：受击顿帧、屏幕震动"。做成全局单例，
    /// 任何攻击命中处都能一行调用，不用各自持有摄像机引用。
    /// 继承项目的 SingletonAutoMono：无需手动挂场景、无需切场景管理，
    /// 首次访问 Instance 时自动生成并 DontDestroyOnLoad。
    ///
    /// 关键坑：HitStop 用 Time.timeScale=0 实现，此时基于缩放时间的等待会被冻结，
    /// 所以顿帧和震屏的协程一律用 WaitForSecondsRealtime（真实时间），
    /// 否则顿帧期间震屏不会推进、顿帧自己也永远恢复不了。
    ///
    /// 同时监听 EventCenter 的 E_Hitstop / E_Camera_Shake 事件，
    /// 让远端逻辑（如敌人爆炸）不持有引用也能触发。
    /// </summary>
    public class GameFeelManager : SingletonAutoMono<GameFeelManager>
    {
        [Header("屏幕震动")]
        [Tooltip("震动衰减时长（秒），强度在这段时间内线性衰减到 0")]
        [SerializeField] private float shakeDuration = 0.15f;
        [Tooltip("强度到实际位移的换算系数")]
        [SerializeField] private float shakeMagnitudeScale = 0.15f;

        // 顿帧防重入：多次命中只保留一次，取较长时长
        private bool _isHitStopping;
        private float _pendingHitStopUntilRealtime;

        // 震屏目标摄像机与其初始局部坐标
        private Transform _camTransform;
        private Coroutine _shakeCo;

        private void Awake()
        {
            CacheCamera();
            // 事件入口：别处可通过 EventCenter 触发，无需持有本单例引用
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Hitstop, HitStop);
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Camera_Shake, ScreenShake);
        }

        private void OnDestroy()
        {
            // 单例随游戏生命周期存在，通常不会销毁；防御性移除监听
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Hitstop, HitStop);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Camera_Shake, ScreenShake);
        }

        /// <summary>
        /// 缓存主摄像机 Transform。摄像机可能在切场景后变化，找不到时重新获取。
        /// </summary>
        private void CacheCamera()
        {
            if (Camera.main != null)
                _camTransform = Camera.main.transform;
        }

        /// <summary>
        /// 命中顿帧：短暂把时间缩放置 0，制造打击卡顿感。
        /// 多次调用取"最晚恢复时间"，避免相互打断导致时间卡死。
        /// </summary>
        /// <param name="duration">顿帧时长（秒，真实时间）</param>
        public void HitStop(float duration)
        {
            if (duration <= 0f) return;

            // 用真实时间算目标恢复点，多次命中取更晚的那个
            float target = Time.realtimeSinceStartup + duration;
            if (target > _pendingHitStopUntilRealtime)
                _pendingHitStopUntilRealtime = target;

            if (!_isHitStopping)
                StartCoroutine(HitStopRoutine());
        }

        private IEnumerator HitStopRoutine()
        {
            _isHitStopping = true;
            float cachedScale = Time.timeScale <= 0f ? 1f : Time.timeScale;
            Time.timeScale = 0f;

            // 用真实时间等待，兼容 timeScale=0；期间若有新命中会推后恢复点
            while (Time.realtimeSinceStartup < _pendingHitStopUntilRealtime)
                yield return null;

            Time.timeScale = cachedScale;
            _isHitStopping = false;
        }

        /// <summary>
        /// 屏幕震动：对主摄像机施加随机抖动并在 shakeDuration 内衰减回原位。
        /// </summary>
        /// <param name="intensity">震动强度（0 表示不震）</param>
        public void ScreenShake(float intensity)
        {
            if (intensity <= 0f) return;
            if (_camTransform == null) CacheCamera();
            if (_camTransform == null) return;

            if (_shakeCo != null)
                StopCoroutine(_shakeCo);
            _shakeCo = StartCoroutine(ShakeRoutine(intensity));
        }

        private IEnumerator ShakeRoutine(float intensity)
        {
            // 每次开震都以"当前"局部坐标为基准，避免上一段未归位造成漂移
            Vector3 basePos = _camTransform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                // 顿帧期间 timeScale=0，用真实间隔推进，保证顿帧+震屏叠加时仍在动
                elapsed += Time.unscaledDeltaTime;
                float damper = 1f - Mathf.Clamp01(elapsed / shakeDuration);
                float mag = intensity * shakeMagnitudeScale * damper;

                Vector2 offset = Random.insideUnitCircle * mag;
                _camTransform.localPosition = basePos + new Vector3(offset.x, offset.y, 0f);
                yield return null;
            }

            _camTransform.localPosition = basePos;
            _shakeCo = null;
        }
    }
}
