using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson32 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 主要使用的知识点 
        //1.字典
        //2.里式替换原则（父类装子类）
        //3.GetComponentsInChildren 方法
        //4.委托、闭包
        //等等
        #endregion

        #region 知识点 实现UI面板基类
        //主要实现思路：
        //在基类中完成声明组件、查找组件、监听组件相关功能
        //让子类可以直接处理事件逻辑，获取指定控件

        //主要实现内容：
        //1.通用的查找组件功能
        //2.通用的添加事件功能
        //3.显示面板、隐藏面板时的逻辑执行虚函数
        //4.获取指定组件的功能
        //等等

        //关键点：
        //制定控件命名规则
        //1.要使用的组件需要改名
        //2.不使用只用于显示的组件可以使用默认名
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
