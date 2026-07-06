using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson40 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //1.Unity中Input公共类 相关知识点（Unity四部曲之Unity入门中）
        //2.事件中心模块

        //注意：
        //1.本节课主要针对的是PC端 鼠标键盘输入相关的内容
        //  如果是手游，会有些不同，但是整体思路是一致的
        //2.本节课中主要针对老输入系统（Input公共类）进行封装
        //  新输入系统（InputSystem）相关知识请观看我的另一套课程（Unity进阶之InputSystem）
        #endregion

        #region 知识点一 为什么要制作 输入控制模块
        //制作输入控制模块主要是降低 输入相关代码的耦合性
        //以前我们制作老输入系统相关功能，都是在Update当中进行按键检测 处理对应逻辑
        //但是当游戏开发中存在角色切换功能时，可能就会存在冗余代码
        //所以我们将通过输入控制模块来降低代码耦合性，减少冗余代码
        #endregion

        #region 知识点二 输入控制模块的基本原理
        //1.制作InputMgr单例模式管理器
        //2.在输入管理器中进行按键检测
        //3.利用事件中心分发事件
        //4.在希望处理输入逻辑的位置监听事件
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
