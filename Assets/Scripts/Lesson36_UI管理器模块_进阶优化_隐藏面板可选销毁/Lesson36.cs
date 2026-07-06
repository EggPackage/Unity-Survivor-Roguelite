using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson36 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 为什么要进行 隐藏面板可选销毁 优化
        //我们目前隐藏面板时，会直接将面板销毁
        //下次创建时再重新创建
        //优点：
        //当存在内存压力时
        //直接销毁面板后，当内存不足时会触发GC
        //不会因为存在没有使用的面板引用而造成内存崩溃

        //缺点：
        //会产生内存垃圾加快GC的触发
        //频繁的销毁创建会增加性能消耗

        //也就是说我们不能直接将面板隐藏改成不销毁
        //而应该改为可以让我们自己控制最好
        //我们可以根据项目的实际情况 选择性的使用失活或销毁
        #endregion

        #region 知识点二 隐藏面板可选销毁实现
        //主要制作思路：
        //  无需使用缓存池，因为缓存池主要是提供给非唯一对象使用的
        //  UI面板大部分情况下是唯一的，因此我们直接在UI管理器中修改逻辑即可
        //  主要实现内容：
        //  1.隐藏面板时，可以选择销毁还是失活
        //  2.显示面板时，如果存在直接激活，如果不存在再重新创建
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
