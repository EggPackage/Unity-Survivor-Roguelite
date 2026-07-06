using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson14 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 事件中心中 参数类型自定义 指的是什么？
        //上节课我们通过万物之父 object 利用里式替换原则（父类装子类）
        //实现了在事件中心中传递参数
        //但是使用object时，当传递值类型参数时，存在装箱拆箱的性能浪费

        //而这节课要讲解的 参数类型自定义 指的就是
        //在传递数据时，参数类型可以根据需求自己定义，想传什么就传什么，而不是固定的object类型
        #endregion

        #region 知识点二 制作思路和具体实现
        //制作思路：
        //想让传递的参数类型可变，并且想传什么就传什么
        //我们可以很自然的联想到使用泛型来制作该功能

        //但是我们需要解决以下关键问题：
        //字典容器中的 泛型委托 如何提前定好类型
        //解决方案：
        //里式替换原则（父类装子类）
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
