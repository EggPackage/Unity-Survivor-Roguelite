using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson8 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 窗口布局优化指的是什么
        //现在直接失活对象，当之后项目做大了，抽屉多了，对象多了
        //游戏中成百上千个对象，在开发测试时不方便从Hierarchy窗口中查看对象获取信息
        //因此我们希望能优化一下Hierarchy窗口中的布局
        //将对象和抽屉的关系可视化
        #endregion

        #region 知识点二 制作思路和具体实现
        //制作思路：
        //1.柜子管理自己的柜子根物体
        //2.抽屉管理自己的抽屉根物体
        //3.失活时建立父子关系，激活活时断开父子关系

        //具体实现:
        //1.先实现将所有对象放入柜子根物体中
        //2.再实现将对象放入对应的抽屉根物体中
        //  用面向对象的思想将抽屉相关数据行为封装起来
        #endregion

        #region 知识点三 将其变为可控制开启的功能
        //这样可以避免在真机运行时，由于父子关系的频繁变换
        //带来一些额外的性能开销
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
    }
}
