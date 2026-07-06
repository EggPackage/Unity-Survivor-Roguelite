using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 本节课使用的知识点
        //细节知识点
        //Unity中Destroy函数（Unity四部曲Unity入门）
        //Unity中特性相关知识（Unity四部曲Unity入门）
        #endregion

        #region 知识点一 重复挂载带来的唯一性问题指什么？
        //对于继承MonoBehaviour的挂载式的单例模式基类
        //1.手动挂载多个相同单例模式脚本
        //2.代码动态添加多个相同单例模式脚本
        #endregion

        #region 知识点二 解决重复挂载带来的安全问题
        //对于挂载式的单例模式脚本
        //1.同个对象的重复挂载
        //  为脚本添加特性[DisallowMultipleComponent]

        //2.修改代码逻辑
        //  判断如果存在对象，移除脚本

        //对于自动挂载式的单例模式脚本
        //  制定使用规则，不允许手动挂载或代码添加
        #endregion

        #region 总结
        //为了避免重复挂载我们一般采用以下几种方案：

        //1.对于挂载式的单例模式基类，相同对象上重复挂载问题，通过添加特性解决
        //2.对于挂载式的单例模式基类，不同对象上的重复挂载，通过逻辑判断，代码移除多余的脚本

        //3.最好的避免重复挂载的方式，就是使用自动挂载式的单例模式基类，并且制定使用规则（不允许手动挂载和代码添加）
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
