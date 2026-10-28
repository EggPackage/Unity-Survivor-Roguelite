using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson6 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 重要内容回顾
        //制作公共Mono模块必备知识点
        //1.委托事件 相关知识（C#四部曲C#进阶中）
        //2.生命周期函数 相关知识（Unity四部曲之Unity入门中）
        //3.协同程序 相关知识（Unity四部曲之Unity基础中

        //公共Mono模块的主要作用
        //让不继承MonoBehaviour的脚本也能
        //1.利用帧更新或定时更新处理逻辑
        //2.利用协同程序处理逻辑
        //3.可以统一执行管理帧更新或定时更新相关逻辑(不管你是否继承MonoBehaviour)
        
        //公共Mono模块的基本原理
        //1.通过事件或委托 管理 相关更新函数
        //2.提供协同程序开启或关闭的方法
        #endregion

        #region 知识点一 实现公共Mono模块
        //1.创建MonoMgr继承 自动挂载式的继承MonoBehaviour的单例模式基类
        //2.实现Update、FixedUpdate、LateUpdate生命周期函数
        //3.声明对应事件或委托用于存储外部函数，并提供添加移除方法，从而达到让不继承MonoBehaviour的脚本可以执行帧更新或定时更新的目的
        //4.声明协同程序开启关闭函数，从而达到让不继承MonoBehaviour的脚本可以执行协同程序的目的
        #endregion

        #region 知识点二 测试
        //MonoMgr.Instance.AddUpdateListener(MyUpdate);
        #endregion
    }

    private void MyUpdate()
    {
        print("Lesson6");
    }
}
