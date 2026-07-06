using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson48 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 问题回顾
        //目前的计时器管理器会受到 Time.timeScale 的影响
        //我们应该如何让计时器管理器可以自己决定是否受到它的影响呢？
        #endregion

        #region 必备知识点
        //Unity中
        //在协同程序中使用
        //yield return new WaitForSeconds(秒);
        //会受到Time.timeScale影响
        //但是Unity也提供了不受其影响的
        //yield return new WaitForSecondsRealtime(秒);
        //我们将利用它制作不受timeScale影响的计时器
        #endregion

        #region 知识点 计时器模块 进阶优化
        //主要制作思路：
        //在计时器模块中
        //保留之前的受Time.timeScale影响的计时器
        //并添加一种不受其影响的计时器
        //让开发者可以根据需求选择使用

        //主要修改处：
        //1.添加一个字典容器专门记录不受其影响的计时器
        //2.多开一个协同程序专门用于处理不受其影响的计时器
        //3.修改相关方法
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
