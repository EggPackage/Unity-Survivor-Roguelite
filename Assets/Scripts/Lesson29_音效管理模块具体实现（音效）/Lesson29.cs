using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson29 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 准备工作
        //随便找几个音效文件作为音效测试内容
        #endregion

        #region 知识点 实现音效管理模块 音效 相关内容
        //主要实现内容
        //1.播放音效
        //2.自动移除播放完成的音效
        //3.停止播放指定音效
        //4.设置音效声音大小
        //5.暂停或继续播放所有音效

        //注意：
        //  1.音效和背景音乐不同
        //    音效存在多个，并且音效需要管理是否结束
        //    因此需要用容器记录音效组件
        //  2.音效分为循环和非循环
        //    非循环的需要我们检测它播放结束
        //    循环的需要让外部进行管理
        #endregion

        #region 提出问题
        //1.频繁的删除创建音效组件，比较浪费性能，应该如何优化？
        //2.3D音效需要让音效对象位置产生变化，我们应该如何修改？
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
