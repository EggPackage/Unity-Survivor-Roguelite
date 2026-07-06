using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson12 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 重要知识回顾
        //必备知识点
        //1.C#中Dictionary相关知识点（C#四部曲之C#进阶中）
        //2.C#中委托事件相关知识点（C#四部曲之C#进阶中）
        //3.C#中泛型相关知识点（C#四部曲之C#进阶中）
        //4.C#中里式替换原则（C#四部曲之C#核心中）
        //5.观察者设计模式

        //事件中心的主要作用
        //解耦程序模块，降低程序耦合度
        //它可以降低游戏中不同模块的耦合度
        //不需要直接引用或依赖于彼此的具体实现

        //事件中心的基本原理
        //利用字典和委托相关的知识点
        //再结合观察者设计模式的基本原理
        //实现一个中心化的机制
        //使得多个系统、模块、对象之间可以进行松耦合的通信
        #endregion

        #region 知识点一 实现事件中心模块
        //1.创建EventCenter继承 不继承MonoBehaviour的单例模式基类
        //2.声明管理事件用容器
        //3.实现关键方法
        //  触发(分发)事件 方法
        //  添加事件监听者 方法
        //  移除事件监听者 方法
        //  清除所有事件监听者 方法
        #endregion

        #region 知识点二 测试

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
