using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson21 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 异步加载AB包可能带来的问题
        //如果我们想要将ABMgr中的异步加载方法改为真正意义上的异步
        //所谓真正意义上的异步是指：不仅从AB包中加载资源是异步的，还需要在加载AB包时也采用异步
        
        //那么我们就需要考虑一个问题
        //如果当我们正在异步加载AB包时，又进行了一次同步加载AB包
        //是否会报错（因为AB包不允许重复加载）
        #endregion

        #region 知识点二 举例说明问题

        #endregion

        #region 知识点三 修改逻辑时的注意事项
        //通过验证我们得出结论
        //异步加载AB包时，再同步加载AB包，会因重复加载而报错
        //因此我们在修改逻辑时应该考虑到这个问题
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
