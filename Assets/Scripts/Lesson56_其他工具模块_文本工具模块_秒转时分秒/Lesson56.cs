using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson56 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现文本工具类中的 秒转时分秒 的方法
        //为什么要封装 秒转时分秒 的方法

        //我们在游戏开发中界面显示时
        //经常可能会有这样的需求
        //要在界面中显示
        //n时n分n秒 的倒计时
        //而一般都是根据倒计时剩余秒数进行转换的

        //因此我们可以将其封装为一个常用方法

        //需求：
        //1.函数有两个参数：秒数 和 是否忽略0（若为 0时0分53秒 前面的0应该不拼接 直接显示53秒）
        //2.可以传入用于拼接的 时分秒 字符
        //3.当忽略0时，如果存在时，分和秒也必须显示；如果存在分，秒也必须显示（避免出现 1时53秒、1时5分 这种显示）

        print(TextUtil.SecondToHMS(0, true));
        print(TextUtil.SecondToHMS(3600, true));
        print(TextUtil.SecondToHMS(60, true));
        print(TextUtil.SecondToHMS(151323215));
        print(TextUtil.SecondToHMS(95328));
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
