using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 本节课使用的知识点
        //大体知识体系
        //C#相关知识点（C#四部曲）

        //细节知识点
        //1.面向对象（C#四部曲C#核心中）
        //2.静态、单例模式相关知识（C#四部曲之C#核心中）
        //3.泛型、泛型约束相关知识点（C#四部曲之C#进阶中）
        #endregion

        #region 知识点一 为什么要写单例模式基类
        //用面向对象的思想避免代码冗余（多余、重复）
        #endregion

        #region 知识点二 实现不继承MonoBehaviour的单例模式基类

        #endregion

        #region 知识点三 潜在的安全问题
        //1.构造函数问题：构造函数可在外部调用 可能会破坏唯一性
        //2.多线程问题：当多个线程同时访问管理器时，可能会出现共享资源的安全访问问题
        #endregion

        #region 总结
        //利用泛型相关知识
        //实现单例模式基类
        //可能存在的安全问题
        //我们在之后的课程中专门讲解
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
