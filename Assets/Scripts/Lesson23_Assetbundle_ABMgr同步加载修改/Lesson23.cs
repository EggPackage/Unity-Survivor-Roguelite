using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson23 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 修改同步加载主要面对的问题
        //在进行同步加载时如果遇到有AB包正在被异步加载应该如何解决？

        //通过我们之前学习的 潜在问题考虑 得知
        //在进行异步加载时再重复加载相同AB包是会报错的
        //即使是同步加载，我们也必须等待异步加载结束，再进行下一步
        #endregion

        #region 知识点二 修改ABMgr中的同步加载相关逻辑
        //1.注释之前的同步加载代码
        //2.在异步加载的基础上进行修改
        #endregion

        #region 总结
        //我们已经完成了对ABMgr的修改
        //主要实现的就是彻底的异步化
        //对于大家来说还有可以修改的内容
        //比如：
        //1.完善卸载资源逻辑
        //2.添加AB包或资源预加载功能
        //3.考虑AB包加载失败的情况
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
