using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson5 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 本节课使用的知识点
        //细节知识点
        //C#中多线程相关知识点（C#四部曲C#进阶）
        #endregion

        #region 知识点一 是否加锁问题指什么？
        //如果程序当中存在多线程
        //我们需要考虑当多个线程同时访问同一个内存空间时出现的问题
        //如果不加以控制，可能会导致数据出错
        //我们一般称这种问题为多线程并发问题，指多线程对共享数据的并发访问和操作。

        //而一般解决该问题的方式，就是通过C#中的lock关键字进行加锁
        //我们需要考虑我们的单例模式对象们是否需要加锁(lock)

        //lock 的原理保证了在任何时刻只有一个线程能够执行被锁保护的代码块
        //从而防止多个线程同时访问或修改共享资源，确保线程安全
        #endregion

        #region 知识点二 解决多线程并发来带的问题
        //1.不继承MonoBehaviour的单例模式
        //  建议加锁，避免以后使用多线程时出现并发问题
        //  比如在处理网络通讯模块、复杂算法模块时，经常会进行多线程并发处理

        //2.继承MonoBehaviour的单例模式
        //  可加可不加，但是建议不加。
        //  因为Unity中的机制是，Unity主线程中处理的一些对象（如GameObject、Transform等等）
        //  是不允许被其他多线程修改访问的，会直接报错
        //  因此我们一般不会通过多线程去访问继承MonoBehaviour的相关对象
        //  既然如何，就不会发生多线程并发问题
        #endregion

        #region 总结
        //对于不继承MonoBehaviour的单例模式对象 
        //建议加锁
        //对于继承MonoBehaviour的单例模式对象 
        //可以不加

        //但是具体是否加锁，都根据需求来定
        //如果你的项目中压根就不会使用多线程
        //那么完全可以不用考虑加锁问题
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
