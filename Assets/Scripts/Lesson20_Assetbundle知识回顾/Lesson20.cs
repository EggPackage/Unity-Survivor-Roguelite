using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson20 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 关于知识回顾选修内容
        //AB包相关知识 是我们在热更新解决方案之Lua中进行讲解的
        //如果你没有学习或者学习了已经忘记了
        //那么可以学习本套课程中的选修内容

        //如果你印象深刻
        //那么可以从资料区获取
        //1.ABMgr脚本
        //2.AB包打包工具 —— AssetBundles-Browser-master
        //将他们导入到工程中来
        //回忆一下我们当时实现的AB包资源加载管理器
        #endregion

        #region 知识点二 Assetbundle资源加载模块我们主要做什么
        //目前已有的ABMgr中的异步加载部分
        //只是从AB包中异步加载资源
        //并没有真正的去异步加载AB包本身
        //我们要做的事情，就是对ABMgr进行优化
        //让其中的异步加载真正意义上的实现异步
        //就是将AB包本身的加载也变为异步的
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
