using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson31 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //1.UGUI 相关知识点（UI四部曲之UGUI中）
        //2.Dictionary 相关知识点（C#四部曲之C#进阶中）
        //3.委托 相关知识点（C#四部曲之C#进阶中）
        //4.MonoBehaviour 相关知识点（Unity四部曲之Unity入门中）
        //等等
        #endregion

        #region 知识点一 回顾制作UI面板的传统流程
        //1.拼面板（必须做）
        //2.声明组件（重复工作）
        //3.查找组件（重复工作）
        //4.监听事件（重复工作）
        //5.处理逻辑（必须做）
        #endregion

        #region 知识点二 UI管理模块的基本原理
        //1.制作UI面板基类，帮助我们自动化的查找组件，监听事件，无需每次写大量冗余代码
        //  将2、3、4步做成自动化的，无需重复去做
        //  解决方案一般有两种：
        //  1-1.自动化工具生成代码（Unity编辑器开发之编辑器拓展中）
        //  1-2.基类中规范冗余代码（我们将讲解的）

        //2.制作UI管理器，管理所有UI面板，UI面板的显示隐藏都通过UI管理器来进行管理
        //  提供公共API供外部使用
        //  比如：
        //  2-1:显示面板
        //  2-2:隐藏面板
        //  2-3:获取面板
        //  2-4:添加自定义事件
        //  等等

        //按照这个思路制作UI管理模块后
        //我们之后在制作UI功能时，只需要把重点放在拼面板，和面板逻辑处理上了
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
