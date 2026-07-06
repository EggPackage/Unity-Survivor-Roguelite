using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 本节课使用的知识点
        //大体知识体系
        //C#相关知识点（C#四部曲） Unity相关知识点（Unity四部曲）

        //细节知识点
        //1.上节课的相关细节知识点
        //2.MonoBehaviour相关知识点（Unity四部曲之Unity入门中）
        #endregion

        #region 知识点一 实现继承MonoBehaviour的单例模式基类的注意事项
        //重要的事情说三边
        //继承MonoBehaviour的脚本不能new！
        //继承MonoBehaviour的脚本不能new！
        //继承MonoBehaviour的脚本不能new！

        //继承MonoBehaviour的脚本一定得依附在GameObject上
        //继承MonoBehaviour的脚本一定得依附在GameObject上
        //继承MonoBehaviour的脚本一定得依附在GameObject上
        #endregion

        #region 知识点二 实现挂载式的单例模式基类
        //这种方式不建议大家使用
        //因为很容易被破坏单例模式的唯一性
        //1.挂载多个脚本
        //2.切换场景回来时，由于场景放置了挂载脚本的对象，回到该场景时 又会有一个该单例模式对象
        //3.还可以通过代码动态的添加多个该脚本 也会破坏唯一性
        #endregion

        #region 知识点三 实现自动挂载式的单例模式基类

        #endregion

        #region 知识点四 潜在的安全问题
        //1.构造函数问题：
        //  继承MonoBehaviour的函数，不能new，所以不用担心公共构造函数
        //2.多线程问题：
        //  Unity主线程中相关内容，不允许其他线程直接调用，很少有这样的需求，所以也不用太担心
        //3.重复挂载问题：
        //  1.手动重复挂载
        //  2.代码重复添加
        //  需要人为干涉，定规则，或者通过代码逻辑强制处理
        #endregion

        #region 总结
        //继承MonoBehaviour的单例模式对象
        //建议大家使用自动挂载式的方案

        //继承MonoBehaviour的单例模式对象
        //潜在的安全问题相对较小，制定好规则后可以忽略不计
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
