using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson53 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 文本工具模块 主要作用
        //在游戏开发中
        //经常会对字符串进行一些处理
        //而这些处理往往会在多处使用
        //为了减少代码冗余
        //我们往往会把常用的文本操作逻辑
        //封装到一个工具类中提供给外部使用

        //因此文本工具模块
        //主要就是提取通用的文本处理逻辑封装为方法
        //供外部使用
        #endregion

        #region 知识点二 文本工具模块 基本原理
        //我们将新建一个文本工具类
        //主要提供以下方法：
        //1.字符串拆封公共方法
        //  字符串拆分为字符串数组
        //  字符串拆分为int数组
        //2.数字前补0转字符串
        //3.秒转时分秒
        //4.秒转00:00:00
        //5.大数据数值转换
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
