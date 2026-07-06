using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson33 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识回顾 制作UI管理器的目的
        //方便管理所有UI面板
        //控制UI面板的显示隐藏
        //  提供公共API供外部使用
        //  比如：
        //  1:显示面板
        //  2:隐藏面板
        //  3:获取面板
        //  4:添加自定义事件
        //  等等
        #endregion

        #region 知识点 制作UI管理器层级规划功能
        //主要思路：
        //1.UI面板在任何场景都会显示，因此Canvas和EventSystem对象应该过场景不移除，并且保证唯一性和动态创建
        //  1-1.UI管理器为不继承MonoBehaviour的单例模式
        //  1-2.在构造函数中动态创建设置好的Canvas和EventSystem预设体(如果使用了UI摄像机，也需要单独处理摄像机预设体)

        //2.UI面板的显示可以存在层级（前后）关系，我们可以预先创建好层级对象，提供获取层级对象的方法
        //  2-1.在Canvas下创建好管理层级的子对象，之后面板作为对应层级对象的子对象达到分层作用
        //  2-2.提供获取层级对象的方法
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
