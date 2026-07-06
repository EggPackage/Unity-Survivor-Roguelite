using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson22 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 准备工作
        //重新按照ABMgr中的规则进行AB包打包
        #endregion

        #region 知识点一 ABMgr异步加载修改主要目标
        //将ABMgr中的异步加载方法彻底异步话
        //让其中的
        //依赖包加载、资源包加载、资源加载都变为异步加载

        //注意：主包可以保留同步加载
        #endregion

        #region 知识点二 需要考虑的主要问题
        //1.某个AB包当正在异步加载时又进行重复加载
        //  遇到这种情况时，我们需要避免重复加载报错
        //  因此我们不应再次加载，而是等待之前的异步加载结束后直接使用

        //2.正在加载某个AB包时
        //  卸载AB包：如果正在加载中，不允许卸载
        //  清空AB包：停止所有协同程序，在清理AB包
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
