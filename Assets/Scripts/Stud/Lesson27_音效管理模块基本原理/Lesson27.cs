using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson27 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //1.Unity音频 相关知识点 (Unity四部曲之Unity入门中)
        //2.List 相关知识点（C#四部曲之C#进阶中）
        //等等
        #endregion

        #region 知识点一 主要作用
        //游戏开发中有很多会用到音乐的地方
        //比如：
        //背景音乐
        //打击音效
        //UI音效
        //特效音效
        //等等
        //而不同的音乐音效会在不同的模块中去进行播放
        //比如：
        //技能释放、怪物受伤、角色受伤、副本通关、奖励发放

        //如果我们不写一个音效管理模块
        //那么在播放音乐音效时会有很多冗余的代码
        //不方便管理，整体结构会显得较为杂乱
        //因此我们单独实现一个独立的音效管理模块
        //专门用于管理控制游戏中的所有音乐相关的功能
        //提供给外部使用
        #endregion

        #region 知识点二 基本原理
        //实现一个音乐管理器
        //提供一些公共方法给外部使用
        //比如：
        //1.播放背景音乐
        //2.停止背景音乐
        //3.播放音效
        //4.停止播放音效
        //5.设置背景音乐大小
        //6.设置音效大小
        //等等

        //所有的系统都可以直接调用
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
