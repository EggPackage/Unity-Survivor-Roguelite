using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson43 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 实现获取输入信息逻辑
        //在InputMgr的更新函数中
        //获取当前输入内容
        //用委托返回给外部

        //主要思路：
        //当存在按下输入时
        //遍历监听KeyCode枚举中所有键位，检测是哪个键位输入了
        //监听鼠标左中右键键位，检测是哪个键位输入了
        #endregion

        #region 知识点二 延迟一帧获取
        //由于开启检测输入时可能伴随着键盘或者鼠标输入
        //因此我们利用协同程序延迟一帧再进行检测
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
