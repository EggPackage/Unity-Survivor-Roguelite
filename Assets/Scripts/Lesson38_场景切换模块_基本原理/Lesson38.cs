using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson38 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //1.场景切换 相关知识点 （Unity四部曲之Unity入门）
        //2.协同程序 相关知识点 （Unity四部曲之Unity基础）
        //3.委托 相关知识点 （C#四部曲之C#进阶）
        #endregion

        #region 知识点一 为什么要制作场景切换模块
        //在游戏开发中很多时候可能需要进行场景切换
        //只要存在场景切换，我们往往需要在 切换场景时 和 切换场景结束后 进行一些操作
        //比如：
        //1.切换场景中
        //  更新Loading界面进度条
        //2.切换场景后
        //  隐藏Loading界面
        //  动态创建场景中关卡信息:如角色、怪物、场景物件等等

        //因此为了避免有太多关于切换场景的冗余代码
        //并且为了使用更方便
        //我们可以将切换场景的逻辑统一整理在一个场景切换管理器中
        #endregion

        #region 知识点二 实现场景切换模块的主要思路
        //1.制作SceneMgr单例模式管理器
        //2.实现同步加载场景 的公共方法
        //3.实现异步加载场景 的公共方法
        //4.实现外部获取异步加载场景进度
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
