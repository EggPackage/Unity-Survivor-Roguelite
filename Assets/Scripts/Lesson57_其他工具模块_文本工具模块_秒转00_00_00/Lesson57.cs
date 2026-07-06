using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson57 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现文本工具类中的 秒转00:00:00 的方法
        //我们上节课封装了 秒转时分秒的方法
        //这节课 我们可以基于上节课代码进行修改
        //提供一个 秒转00:00:00 的方法

        //需求：
        //在上节课的需求基础上加入
        //1.用:作为间隔符进行封装
        //2.每个位置的格式为最少2位(即存在这样的格式 01:05:35 秒)

        print(TextUtil.SecondToHMS2(0));
        print(TextUtil.SecondToHMS2(60, true));
        print(TextUtil.SecondToHMS2(3600));
        print(TextUtil.SecondToHMS2(5416518));
        print(TextUtil.SecondToHMS2(85251));

        print(TextUtil.SecondToHMS2(5));
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
