using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson18 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 问题回顾
        //1.在卸载资源时，我们并不知道是否还有地方使用着该资源
        //2.UnloadUnusedAssets是卸载没有使用的资源，我们无法判断是否使用
        #endregion

        #region 知识点一 通过引用计数判断资源是否使用的问题
        //什么是引用计数?
        //引用计数是一种内存管理技术，用于跟踪资源被引用的次数
        //我们通过一个整形变量来记录资源的使用次数
        //当有对象引用该资源时，计数器会增加；当对象不再引用该资源时，计数器会减少
        #endregion

        #region 知识点二 向ResMgr中加入引用计数功能
        //1.为ResInfo类加入引用计数成员变量和方法
        //2.使用资源时加
        //3.不使用资源时减
        //4.处理异步回调问题，某一个异步不使用资源了应该移除回调函数的记录
        //5.修改移除资源函数逻辑，引用计数为0时才真正移除资源
        //6.考虑资源频繁移除问题，加入马上移除bool标签
        //7.修改移除不使用资源函数逻辑，释放时清楚引用计数为0的记录
        #endregion

        #region 注意事项
        //1.加入引用计数的ResMgr
        //  我们在使用资源时就需要有用就有删
        //  当使用某个资源的对象移除时，一定要记得调用移除方法

        //2.如果觉得卸载资源的功能麻烦，也完全可以不使用卸载的相关方法
        //  加载相关逻辑不会有任何影响，和以前直接使用Resources的用法几乎一样
        //  只需要再添加一个主动清空字典的方法即可
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
