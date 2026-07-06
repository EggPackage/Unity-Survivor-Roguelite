using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson44 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 缓存池模块(数据结构类、逻辑类)优化 主要目的是什么？
        //我们目前实现的缓存池
        //主要是针对场景上的GameObject对象的
        //我们只能对场景上的对象进行缓存池功能处理

        //但是在游戏开发中，还会存在大量的不需要挂载到场景上的实例化对象
        //比如一些数据结构类、逻辑类，它们并不依附于场景上的对象，而仅仅是被引用
        //举例：
        //一个自定义数据结构类
        //TestData t = new TestData();
        //当我们不使用它时，往往会将其置空
        //t = null;
        //下次又要使用时，再new
        //t = new TestData();
        //那么对于一些频繁使用的数据结构类或逻辑类
        //这样做也会产生大量的垃圾
        //因此我们完全可以修改缓存池模块，让其也支持对不挂载的类对象也进行复用

        //说人话
        //缓存池模块(数据结构类、逻辑类)优化 主要目的是
        //让缓存池支持回收复用不继承MonoBehaviour（不挂载GameObject）的类对象
        #endregion

        #region 知识点二 缓存池模块(数据结构类、逻辑类)优化 制作思路
        //1.修改缓存池管理器PoolMgr
        //2.添加一个新池子容器，专门用来记录不继承MonoBehaviour（不挂载GameObject）的类对象
        //3.提供专门的方法供外部使用
        //  3-1.从池子中获取对象的方法
        //  3-2.压入池子中的方法
        //  3-3.清空池子容器的方法

        //注意：由于这些自定义数据或逻辑类中可能有对其他内容的引用
        //     因此需要让其有一个重置数据的方法，在压入池子时重置数据
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
