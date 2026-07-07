using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson59 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //1.Unity中向量、四元数 相关知识点（Unity四部曲之Unity基础中）
        //2.射线检测 相关知识点（Unity四部曲之Unity基础中）
        //3.范围检测 相关知识点 （Unity四部曲之Unity基础中）
        #endregion

        #region 知识点一 数学计算工具模块 主要作用
        //在游戏开发中
        //经常会进行一些通用的数学计算
        //为了减少代码冗余
        //我们往往会把常用的数学计算逻辑
        //封装到一个工具类中提供给外部使用

        //因此数学计算工具模块
        //主要就是将通用的数学计算逻辑封装为方法
        //供外部使用
        #endregion

        #region 知识点二 数学计算工具模块 基本原理
        //我们将新建一个数学计算工具类
        //主要提供以下方法：
        //1.角度和弧度的转换
        //2.得到两个对象在xz平面上的距离 
        //3.判断对象是否在屏幕范围外
        //4.判断对象位置是否在xz平面扇形范围内
        //5.射线检测
        //6.范围检测
        //等等

        //我们只实现以上方法
        //更多内容，在实际开发时根据需求加入到工具类中即可
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
