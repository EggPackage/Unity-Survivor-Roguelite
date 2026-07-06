using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson16 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //1.C#中Dictionary相关知识点（C#四部曲C#进阶）
        //2.C#中委托相关知识点（C#四部曲C#进阶）
        //3.Unity中Resources相关知识点（Unity四部曲Unity基础）
        //4.Unity中协同程序相关知识点（Unity四部曲Unity基础）
        #endregion

        #region 知识点 Resources资源加载模块的具体实现
        //1.创建ResourcesMgr 继承 不继承MonoBehaviour的单例模式基类
        //2.为它封装Resources异步加载资源的相关方法（主要目的 避免异步加载的代码冗余）
        //3.为它封装Resources同步加载资源的相关方法（顺便封装）
        //4.为它封装资源卸载相关方法（顺便封装）
        #endregion
    }

    //private void LoadOverDoSomthing(GameObject obj)
    //{
    //    Instantiate(obj);
    //}


    // Update is called once per frame
    void Update()
    {
        
    }
}
