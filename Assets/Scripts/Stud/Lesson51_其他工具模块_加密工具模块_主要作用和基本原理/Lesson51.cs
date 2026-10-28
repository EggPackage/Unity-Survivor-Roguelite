using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson51 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //数据加密、异或加密 相关知识点（数据持久化之2进制中）
        #endregion

        #region 知识点一 加密工具模块 主要作用
        //在游戏开发中
        //为了防止玩家利用修改器等工具修改游戏数据
        //我们往往会加入数据加密功能
        //主要作用就是提高游戏安全性，防止被修改
        //而加密工具模块就是为了方便数据加密而存在的

        //加密工具模块的主要作用
        //就是将存储在内存中的数据进行加密
        //避免玩家利用修改器查找内存中数据进行修改
        #endregion

        #region 知识点二 加密工具模块 基本原理
        //加密：
        //将原本存储在内存中的值进行加密后变换为另一个值存储在内存中
        //解密：
        //将内存中存储的值进行解密后得到原始值
        //加密算法：
        //异或加密

        //我们将新建一个加密工具类
        //主要提供以下方法：
        //1.获取随机密钥 方法
        //2.加密数据 方法
        //3.解密数据 方法

        //只要在需要加密的地方进行相关方法调用即可
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
