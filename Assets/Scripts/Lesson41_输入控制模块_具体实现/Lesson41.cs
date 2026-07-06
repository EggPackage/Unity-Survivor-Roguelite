using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson41 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现输入控制模块
        //主要要实现的就是监听
        //键盘、鼠标、热键的输入
        //并分发事件

        //1.制作InputMgr单例模式管理器
        //2.在输入管理器中进行按键检测
        //3.利用事件中心分发事件
        //4.在希望处理输入逻辑的位置监听事件
        //5.提供输入系统检测开关
        #endregion

        #region 提出问题
        //自己思考，我不会讲
        //  1.多开关，分别控制键盘、鼠标、热键检测的开关 —— 思路：加bool值
        //  2.按下、抬起、长按3种类型的事件合3为1 —— 思路：通过参数传递当前事件类型
        //进阶优化中会讲解的
        //  3.如何制作改键功能
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
