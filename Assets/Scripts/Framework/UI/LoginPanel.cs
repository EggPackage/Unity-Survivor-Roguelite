using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public override void ShowMe()
    {
        Debug.Log("LoginPanel.ShowMe() —— 登录面板显示");
        gameObject.SetActive(true);
    }

    public override void HideMe()
    {
        Debug.Log("LoginPanel.HideMe() —— 登录面板隐藏");
        gameObject.SetActive(false);
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "StartBtn":
                Debug.Log("LoginPanel: StartBtn 被点击，开始跳转到 DungeonDemo 场景");
                UIMgr.Instance.HidePanel<LoginPanel>(true);
                SceneMgr.Instance.LoadSceneAsyn("DungeonDemo", () =>
                {
                    Debug.Log("LoginPanel: DungeonDemo 场景加载完成");
                });
                break;

            default:
                Debug.LogWarning($"LoginPanel: 未处理的按钮名 —— {btnName}");
                break;
        }
    }
}