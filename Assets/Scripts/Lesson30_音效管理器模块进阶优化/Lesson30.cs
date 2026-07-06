using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson30 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 优化频繁创建删除音效组件
        //目前我们音效管理器中的音效组件会频繁的创建和删除
        //这样会产生大量的内存垃圾，并且频繁创建对象也会带来性能消耗
        //因此我们可以利用缓存池对其进行优化
        #endregion

        #region 知识点二 优化3D音效问题
        //由于我们使用了缓存池，因此AudioSource依附的将会是一个个独立的游戏对象
        //而3D音效主要考虑的问题是：
        //1.音效依附在对象上跟随对象移动
        //2.音效中3D相关参数的设置

        //如果你想要使用3D音效，那么只需要获取到音效组件和它依附的对象就可以迎刃而解了
        //1.获取音效组件依附的对象，改变它的父对象，设置它的位置，音效便可以跟随对象移动
        //2.获取音效组件改变其中的3D音效相关参数即可完成相关设置
        #endregion

        #region 知识点三 提供清除所有音效的方法
        //目前过场景时，音效相关对象会被自动的删除
        //但是音效管理器中我们的容器还占着引用，我们应该提供方法清空容器
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
