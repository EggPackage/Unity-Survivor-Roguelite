using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson7 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 重要内容回顾
        //必备知识点
        //1.C#中垃圾回收机制（GC）相关知识（C#四部曲之C#核心中）
        //2.C#中Dictionary、List、Stack、Queue相关知识点（C#四部曲之C#进阶中）
        //3.Unity中GameObject相关知识（Unity四部曲之Unity入门中）
        //4.Unity中Resources相关知识点（Unity四部曲之Unity入门中

        //缓存池（对象池）的主要作用
        //通过重复利用已经创建的对象，避免频繁的创建和销毁
        //从而减少系统的内存分配和垃圾回收带来的开销

        //缓存池（对象池）的基本原理
        //用一个“柜子”中的“各种抽屉”来装“东西”
        //用时去拿（没有就创造，存在就获取）
        //不用就还（将“东西”分门别类的放入“抽屉”中）
        #endregion

        #region 知识点一 实现缓存池(对象池)模块
        //1.创建PoolMgr继承 不继承MonoBehaviour的单例模式基类
        //2.声明柜子(Dictionary)和抽屉(List、Stack、Queue等)容器
        //3.拿东西方法
        //  3-1：有抽屉并且抽屉里有东西 直接获取
        //  3-2: 没有抽屉或者抽屉里没东西 创造
        //4.放东西方法
        //  4-1：有抽屉，直接放
        //  4-2：没抽屉，创建抽屉，再放
        //5.清空柜子方法
        //  我们在切场景时，对象都会被移除，这时应该清空柜子
        //  否则会出现内存泄漏，并且下次取东西会出问题
        #endregion

        #region 知识点二 测试
        //1.创建测试用几何体
        //2.通过监听鼠标事件，动态创建物体
        //3.创建一个延迟销毁脚本
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
