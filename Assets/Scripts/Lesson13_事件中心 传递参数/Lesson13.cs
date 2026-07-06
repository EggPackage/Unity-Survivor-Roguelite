using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson13 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 事件中心中传递参数指的是什么？
        //希望在触发事件时传递数据
        //比如
        //怪物死亡时，将怪物信息传递出去
        //获取奖励时，将奖励信息传递出去
        //等等

        //传递数据的主要目的是
        //我们可以在各系统、模块、对象中获取到我们希望获取的有用信息
        #endregion

        #region 知识点二 制作思路和具体实现
        //我们目前触发事件，是通过执行委托中存储的函数来执行各系统对应的逻辑
        //那么需要传递参数，我们很自然的联想到应该从委托入手

        //但是参数的类型可能有多种多样，我们可以采用万物之父 object 利用里式替换原则
        //父类容器装载子类对象的方式来传递参数
        #endregion

        #region 知识点三 存在的问题
        //使用object进行参数传递，当传递值类型数据时，会存在装箱拆箱，增加性能开销
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
