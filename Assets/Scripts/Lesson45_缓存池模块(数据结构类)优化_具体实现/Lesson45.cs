using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson45 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 缓存池模块(数据结构类、逻辑类)优化 具体实现
        //主要思路：
        //1.修改缓存池管理器PoolMgr
        //2.添加一个新池子容器，专门用来记录不继承MonoBehaviour（不挂载GameObject）的类对象
        //3.提供专门的方法供外部使用
        //  3-1.从池子中获取对象的方法
        //  3-2.压入池子中的方法
        //  3-3.清空池子容器的方法

        //注意：由于这些自定义数据或逻辑类中可能有对其他内容的引用
        //     因此需要让其有一个重置数据的方法，在压入池子时重置数据

        //关键点：
        //1.池子容器父类 里式替换 父类装子类
        //2.池子容器子类 泛型类 确定对象类型
        //3.对象父接口，用于实现重置数据方法
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
