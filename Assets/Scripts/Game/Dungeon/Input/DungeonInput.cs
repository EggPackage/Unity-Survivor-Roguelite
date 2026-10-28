using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Dungeon.Input
{
    /// <summary>
    /// 地牢输入层（M1 核心）：只产出"意图"，业务层（DungeonPlayerController / BattleManager）
    /// 永远不认识 InputAction / KeyCode / 具体设备。
    ///
    /// 设备无关：
    ///   - Move   已绑 键盘 WASD（2DVector 复合）+ 手柄左摇杆
    ///   - Pause  已绑 ESC + 手柄 Start（M2 用，M1 先留位）
    ///   - Restart 已绑 R（死亡后重开）
    /// 后续 M2 加手柄按键、M3 改键、M5 触屏摇杆，只改本文件，业务代码零改动。
    ///
    /// 实现选择（与 M1 策划案 2.1 的偏差，已确认）：
    ///   采用"内联构建 InputActionMap"，不依赖 .inputactions 生成类。
    ///   理由——零外部资产依赖、首编译即可用、纯 C# 可断点、不踩生成类时序。
    ///   代价——改键不能可视化。若 M3 要做可视化改键，可改为加载 .inputactions
    ///   资产并转发到本层，对外事件契约（OnMove/OnPause/OnRestart）不变。
    ///
    /// 说明：自动攻击不配输入键（见策划案 2.2），攻击由 WeaponRunner 定时驱动。
    /// </summary>
    public static class DungeonInput
    {
        public static event Action<Vector2> OnMove;
        public static event Action OnPause;
        public static event Action OnRestart;

        private static InputActionMap _map;
        private static bool _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            _map = new InputActionMap("Dungeon");

            // Move：键盘 WASD（2DVector 复合） + 手柄左摇杆
            InputAction move = _map.AddAction("Move", InputActionType.Value);
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            move.AddBinding("<Gamepad>/leftStick");

            // 自动攻击不配输入键（见策划案 2.2），攻击由 WeaponRunner 定时驱动

            // Pause：ESC + 手柄 Start
            InputAction pause = _map.AddAction("Pause", InputActionType.Button);
            pause.AddBinding("<Keyboard>/escape");
            pause.AddBinding("<Gamepad>/start");

            // Restart：R（死亡后重开）
            InputAction restart = _map.AddAction("Restart", InputActionType.Button);
            restart.AddBinding("<Keyboard>/r");

            move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            move.canceled  += ctx => OnMove?.Invoke(Vector2.zero);
            pause.performed  += ctx => OnPause?.Invoke();
            restart.performed += ctx => OnRestart?.Invoke();

            _map.Enable();
        }

        /// <summary>
        /// 业务层 OnEnable 时调用：保证输入表已启用（首帧不会漏掉意图）。
        /// </summary>
        public static void EnsureEnabled()
        {
            if (!_initialized) Init();
            _map?.Enable();
        }
    }
}
