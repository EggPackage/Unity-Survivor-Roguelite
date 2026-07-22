using UnityEngine;

/// <summary>
/// 入口场景启动器：场景加载时，通过 UIMgr 显示 LoginPanel 登录面板。
///
/// 用法：
///   1. 在入口场景（如 StartScenes.unity）里放一个空 GameObject
///   2. 挂上本脚本
///   3. 确保场景里没有直接把 LoginPanel 拖到 Canvas 下（否则会出现两个面板）
///   4. 确保 LoginPanel.prefab 放在 Assets/Resources/UI/LoginPanel.prefab
///      （或打进名为 "ui" 的 AssetBundle 包），并且预设体上挂了 LoginPanel 脚本
///
/// 面板上的按钮：
///   - 名为 "StartBtn" 的 Button：点击后跳转到 DungeonDemo 场景
///   - 按钮名字必须和 LoginPanel.ClickBtn() 里的 switch case 严格一致
/// </summary>
public class StartSceneBootstrap : MonoBehaviour
{
    private void Start()
    {
        // 让 InputMgr 先开启，按钮才能响应
        InputMgr.Instance.StartOrCloseInputMgr(true);

        // 通过 UIMgr 动态加载 LoginPanel（从 ui AB 包，找不到就回退 Resources/UI/）
        UIMgr.Instance.ShowPanel<LoginPanel>(E_UILayer.Middle, (panel) =>
        {
            Debug.Log("[StartSceneBootstrap] LoginPanel 已加载并显示");
        }, true);
    }
}