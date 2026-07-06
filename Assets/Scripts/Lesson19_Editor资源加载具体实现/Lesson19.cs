using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson19 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 主要使用的API
        //1.AssetDatabase.LoadAssetAtPath                       用于加载单个资源
        //2.AssetDatabase.LoadAllAssetRepresentationsAtPath     用于加载图集资源中的内容(该API用于加载所有子资源)

        //注意：
        //这两个API加载资源的路径都是从 Assets/... 开始
        #endregion

        #region 知识点二 创建用于放置资源的文件夹
        //我们可以创建Editor文件夹
        //以后我们最终会打包为AB包的资源都放置在此处
        #endregion

        #region 知识点三 实现Editor资源管理器
        //封装API
        //主要提供加载单个资源以及加载图集资源的API
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
