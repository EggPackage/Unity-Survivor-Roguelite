using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public override void ShowMe()
    {
        gameObject.SetActive(true);
    }

    public override void HideMe()
    {
        gameObject.SetActive(false);
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "StartBtn":
                UIMgr.Instance.HidePanel<LoginPanel>(true);
                SceneMgr.Instance.LoadSceneAsyn("DungeonDemo", () =>
                {
                 
                });
                break;

            default:
                Debug.LogWarning($"LoginPanel: 未处理的按钮名 —— {btnName}");
                break;
        }
    }
}