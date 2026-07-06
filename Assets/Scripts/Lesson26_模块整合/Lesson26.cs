using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson26 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 小框架代码整合
        //将之前写的小框架相关代码分门别类的整合在小框架文件夹中
        #endregion

        #region 知识点二 EditorResMgr和ABMgr整合
        //EditorResMgr的主要目的是方便资源管理
        //避免频繁的打AB包进行测试
        //它主要是为AB包预备资源服务的
        //因此我们可以将它和ABMgr整合在一起
        #endregion

        #region 知识点三 为什么不整合ResMgr和UWQResMgr
        //这两种资源加载管理器相对比较独立
        //一个专门用于加载Resources下的资源
        //一个专门用于加载指定位置的资源
        //当我们使用它们时那一定是确定了位置和加载方式后才会使用的
        //因此无需更多的整合处理
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
