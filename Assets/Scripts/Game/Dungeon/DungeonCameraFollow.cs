using UnityEngine;

namespace Game.Dungeon
{
    /// <summary>
    /// 地牢相机跟随：平滑跟随玩家，保持正交俯视。
    ///
    /// ==== 设计说明 ====
    /// 原来的 Main Camera 是静态的，玩家一走就出画面。这里让相机在 LateUpdate
    /// （所有移动结束后）指数平滑地追玩家的 xy，z 保持不变（2D 相机需在 -10）。
    /// 可选边界钳制：地图是固定 30×20 矩形，把相机中心限制在地图内一圈，
    /// 避免边缘露出地图外的空白。留空（useBounds=false）则纯跟随。
    ///
    /// 只动相机 localPosition 的 xy。注意与 GameFeelManager 的震屏有交互：
    /// 震屏改的是相机 localPosition，本脚本每帧也写 localPosition —— 两者会打架。
    /// 折中：本脚本跟随的是"目标基准位"，震屏是短促的叠加抖动且很快归位，
    /// 原型阶段视觉上可接受；若明显冲突，后续把跟随与震屏拆到父子两层节点。
    /// </summary>
    public class DungeonCameraFollow : MonoBehaviour
    {
        [Header("跟随目标")]
        [Tooltip("跟随对象。留空则自动按 Tag=Player 查找")]
        [SerializeField] private Transform target;
        [Tooltip("跟随平滑系数，越大越快贴合")]
        [SerializeField] private float smooth = 8f;

        [Header("边界钳制（可选）")]
        [Tooltip("是否把相机中心限制在地图范围内")]
        [SerializeField] private bool useBounds = false;
        [Tooltip("相机中心可到达的最小 xy")]
        [SerializeField] private Vector2 min = new Vector2(4f, 4f);
        [Tooltip("相机中心可到达的最大 xy")]
        [SerializeField] private Vector2 max = new Vector2(26f, 16f);

        private float _z;

        private void Awake()
        {
            _z = transform.position.z; // 记住初始 z（2D 通常 -10）
        }

        private void Start()
        {
            if (target == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) target = p.transform;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 cur = transform.position;
            Vector3 goal = new Vector3(target.position.x, target.position.y, _z);

            if (useBounds)
            {
                goal.x = Mathf.Clamp(goal.x, min.x, max.x);
                goal.y = Mathf.Clamp(goal.y, min.y, max.y);
            }

            // 用 unscaledDeltaTime，结算冻结（timeScale=0）时相机不会僵在半途
            float t = 1f - Mathf.Exp(-smooth * Time.unscaledDeltaTime);
            transform.position = Vector3.Lerp(cur, goal, t);
        }
    }
}
