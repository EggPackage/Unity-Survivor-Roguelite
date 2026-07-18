using UnityEngine;

/// <summary>
/// WASD 8 方向移动 + 朝向切换 + 待机/移动状态。
/// 通过 Animator 参数 MoveX / MoveY / Speed 驱动 8 方向 Blend Tree。
///
/// 素材限制:只有 south 方向有逐帧跑步动画,其余方向是静态朝向图。
/// 为了让所有方向移动时都有跑动感,这里对显示用的子物体(Visual)
/// 施加程序化的上下弹跳(hop),与 south 的逐帧动画叠加,整体统一。
/// </summary>
public class PlayerController2D : MonoBehaviour
{
    [Header("移动")]
    [Tooltip("移动速度(单位/秒)")]
    public float moveSpeed = 3f;

    [Header("弹跳(移动时的上下起伏)")]
    [Tooltip("弹跳高度(局部单位)")]
    public float hopHeight = 0.08f;
    [Tooltip("每秒弹跳次数")]
    public float hopFrequency = 4f;
    [Tooltip("停止移动后弹跳归位速度倍率")]
    public float hopReturnSpeed = 12f;

    [Header("组件引用")]
    [Tooltip("显示用子物体(挂 SpriteRenderer / Animator)。留空则自动查找名为 Visual 的子物体")]
    public Transform visual;
    [Tooltip("留空则从 visual 上自动获取")]
    public Animator animator;

    // 记录最后的朝向,静止时用于保持面朝方向
    private Vector2 _lastDir = Vector2.down; // 默认朝南

    private float _hopPhase;    // 弹跳相位
    private float _visualBaseY; // 子物体初始局部 Y

    // Animator 参数哈希
    private static readonly int HashMoveX = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY = Animator.StringToHash("MoveY");
    private static readonly int HashSpeed = Animator.StringToHash("Speed");

    private void Reset()
    {
        visual = transform.Find("Visual");
        if (visual != null)
            animator = visual.GetComponent<Animator>();
    }

    private void Awake()
    {
        if (visual == null)
            visual = transform.Find("Visual");
        if (animator == null && visual != null)
            animator = visual.GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (visual != null)
            _visualBaseY = visual.localPosition.y;
    }

    private void Update()
    {
        // 读取输入
        float x = 0f;
        float y = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;

        Vector2 input = new Vector2(x, y);
        bool moving = input.sqrMagnitude > 0.001f;

        if (moving)
        {
            input.Normalize();          // 斜向不加速
            _lastDir = input;           // 记录朝向
        }

        // 移动根物体
        Vector3 delta = new Vector3(_lastDir.x, _lastDir.y, 0f) * (moving ? moveSpeed : 0f) * Time.deltaTime;
        transform.position += delta;

        // 驱动动画:朝向始终有效,Speed 控制 Idle/Move 切换
        if (animator != null)
        {
            animator.SetFloat(HashMoveX, _lastDir.x);
            animator.SetFloat(HashMoveY, _lastDir.y);
            animator.SetFloat(HashSpeed, moving ? 1f : 0f);
        }

        // 程序化弹跳:对所有方向都生效,让移动有跑动感
        UpdateHop(moving);
    }

    private void UpdateHop(bool moving)
    {
        if (visual == null) return;

        Vector3 lp = visual.localPosition;

        if (moving)
        {
            _hopPhase += Time.deltaTime * hopFrequency * Mathf.PI * 2f;
            // abs(sin) 让角色每半个周期落地一次,像脚步
            float offset = Mathf.Abs(Mathf.Sin(_hopPhase)) * hopHeight;
            lp.y = _visualBaseY + offset;
        }
        else
        {
            _hopPhase = 0f;
            lp.y = Mathf.MoveTowards(lp.y, _visualBaseY, hopReturnSpeed * hopHeight * Time.deltaTime);
        }

        visual.localPosition = lp;
    }
}
