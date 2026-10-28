using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson55 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 必备知识点
        //1.内插字符串 相关知识点（C#四部曲之C#进阶中）
        //2.ToString() 相关知识点（C#四部曲之C#核心中）
        #endregion

        #region 知识点一 实现文本工具类中的 转字符串数字前补0 的方法
        //为什么要封装 转字符串数字前补0 的方法

        //我们在游戏开发中界面显示时
        //经常可能会有这样的需求
        //要在界面中显示
        //0001
        //01
        //010
        //也就是说想要让某个数字在转换为字符串时
        //可以指定转换的字符串长度
        //并且能够在数字前自动的添加0

        print(TextUtil.GetNumStr(1, 4));//0001
        print(TextUtil.GetNumStr(10, 3));//010
        print(TextUtil.GetNumStr(1000, 3));//1000
        #endregion

        #region 知识点二 实现文本工具类中的 转字符串保留n位小数 的方法
        //为什么要封装 转字符串保留n位小数 的方法

        //我们在游戏开发中界面显示时
        //经常可能会有这样的需求
        //要在界面中显示
        //3.1415926 保留两位小数 只显示 3.14
        //也就是说想要让某个数字在转换为字符串时
        //可以指定保留小数点后的位数

        print(TextUtil.GetDecimalStr(3.1415926f, 2));//3.14
        print(TextUtil.GetDecimalStr(3.1415926f, 4));//3.1415
        print(TextUtil.GetDecimalStr(0.000006f, 2));//0.00
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
