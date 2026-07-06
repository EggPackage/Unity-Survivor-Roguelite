using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson24 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 关于 知识回顾 选修内容
        //UnityWebRequest相关知识 是我们在Unity网络开发基础中进行讲解的
        //如果你没有学习或者学习了已经忘记了
        //那么可以学习本套课程中的选修内容

        //选修内容包括 WWW 和 UnityWebRequest 获取数据的常用操作
        //WWW在新版本中已经是过时的方案，但是还能够使用，可以作为了解
        //UnityWebRequest是需要着重回顾的

        //重要内容回顾：
        //WWW 和 UnityWebRequest 都是Unity提供给我们的简单的访问网页的类
        //我们可以利用它们来进行数据的上传和下载
        //除此之外
        //它们都支持 file:// 本地文件传输协议
        //我们可以利用该协议来异步加载本地文件

        //特别是在Android平台时
        //我们无法直接通过C#中的File公共类加载StreamingAssets文件夹中的内容
        //我们需要使用WWW或UnityWebRequest类来加载
        #endregion

        #region 知识点二 UnityWebRequest资源加载模块我们主要做什么
        //我们主要封装UnityWebRequest当中的
        //1.获取文本或二进制数据 方法
        //2.获取纹理数据 方法
        //3.获取AB包数据 方法
        //制作UWQResMgr，让外部使用UnityWebRequest加载资源时更加方便
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
