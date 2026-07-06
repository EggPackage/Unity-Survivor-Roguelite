using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 本节课使用的知识点
        //细节知识点
        //C#中抽象类的知识点（C#四部曲之C#核心）
        //C#中反射相关知识点（C#四部曲之C#进阶）
        #endregion

        #region 知识点一 构造函数带来的唯一性问题指什么？
        //1.对于不继承MonoBehaviour的单例模式基类
        //  我们要避免在外部 new 单例模式类对象

        //2.对于继承MonoBehaviour的单例模式基类
        //  由于继承MonoBehaviour的脚本不能通过new创建，因此不用过多考虑
        #endregion

        #region 知识点二 解决构造函数带来的安全问题
        //1.父类变为抽象类

        //2.规定继承单例模式基类的类必须显示实现私有无参构造函数

        //3.在基类中通过反射来调用私有构造函数实例化对象
        //  主要知识点：
        //  利用Type中的 GetConstructor(约束条件, 绑定对象, 参数类型, 参数修饰符)方法
        //  来获取私有无参构造函数
        //  ConstructorInfo constructor = typeof(T).GetConstructor(
        //  BindingFlags.Instance | BindingFlags.NonPublic, //表示成员私有方法
        //    null,                                         //表示没有绑定对象
        //    Type.EmptyTypes,                              //表示没有参数
        //    null);                                        //表示没有参数修饰符
        //          
        #endregion

        #region 总结
        //虽然公共构造函数可能给我们带来破坏唯一性的安全隐患
        //但是在实际开发中
        //单例模式是非常基础的知识点
        //可以说是程序员的必备基础知识
        //在使用时几乎不会有人马虎到自己去new单例模式对象

        //因此即使我们不做这些安全性的处理
        //其实也不会有太大问题
        //根据实际情况和需求进行设计即可
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
