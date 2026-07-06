using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Lesson47 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现 计时器模块
        //想要达到的效果：
        //1.能够延时响应逻辑（比如：10s后执行某一逻辑）
        //2.在延时过程中，可以固定时间间隔响应逻辑（比如：10s后执行某一逻辑，但是10s内每隔1s又能响应其它逻辑）

        //基本原理：
        //1.定义一个计时器对象，计时器对象记录延时执行时间、间隔执行时间、执行结束回调委托、间隔执行回调委托等等
        //2.计时器模块中统一管理所有计时器对象，每个计时器对象有一个唯一ID，用来区分获取计时器对象
        //3.在计时器模块中利用一个协同程序固定间隔时间进行计时器对象的时间检测
        //4.计时器模块中提供创建计时器、移除计时器、停止计时器、开始计时器、重置计时器的相关方法

        //主要要实现的类：
        //计时器模块管理器
        //TimerMgr
        //  需要声明的关键成员
        //  1.计时器字典容器
        //  2.待移除计时器列表容器
        //  等等
        //  需要实现的关键方法
        //  1.开启计时器管理器
        //  2.关闭计时器管理器
        //  3.创建单个计时器
        //  4.移除单个计时器
        //  5.重置单个计时器
        //  6.开启单个计时器
        //  7.停止单个计时器
        //  等等

        //计时器对象类
        //TimerItem
        //  需要声明的关键成员
        //  1.唯一ID
        //  2.延时执行回调委托
        //  3.间隔执行回到委托
        //  4.总时间
        //  5.间隔时间
        //  6.是否开启
        //  等等
        //  需要实现的关键方法
        //  1.初始化数据
        //  2.重置时间
        //  等等
        #endregion

        #region 提出问题
        //目前的计时器管理器会受到 Time.timeScale 的影响
        //我们应该如何让计时器管理器可以自己决定是否受到它的影响呢？
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
