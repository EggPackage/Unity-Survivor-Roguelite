using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson37 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点 
        //UGUI中 EventTrigger 相关知识点（UI四部曲之UGUI 中）
        #endregion

        #region 知识点一 为什么要进行 自定义事件添加函数 优化
        //我们在制作UI功能时
        //经常会有这样的需求:
        //1.为一些不带默认事件的控件添加自定义事件，比如Image、Text这些基础组件，想为他们添加点击、单击、拖拽等事件监听
        //2.为一些带默认事件的控件添加自定义事件，比如为 Button 按钮添加鼠标进入、鼠标移除等事件监听
        //等等

        //因此我们完全可以把添加自定义事件封装为一个公共函数
        //方便为各种控件添加自定义事件
        #endregion

        #region 知识点二 自定义事件添加函数 实现
        //主要实现思路:
        //1.为想要添加自定义事件的控件添加EventTrigger组件
        //2.通过EventTrigger组件添加对应自定义事件的监听
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
