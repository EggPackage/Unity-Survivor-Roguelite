using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson58 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现文本工具类中的 大数据数值转字符串 的方法
        //为什么要封装 大数据数值转字符串 的方法

        //我们在游戏开发时
        //为了避免在界面中显示过长的数值
        //往往需要把过长数值进行转换
        //比如：
        //1198678978 转换为 11亿9千万
        //5328987 转换为 532万8千
        //因为如果直接在界面上显示过长的数值可能会影响UI布局
        //而过大的数也可以直接粗略显示
        //可以直接省略后面的内容
        //显示一个大概数值

        //因此我们可以将其封装为一个常用方法

        //需求：
        //以 亿 和 万 作为转换临界点

        print(TextUtil.GetBigDataToString(1245000000));
        print(TextUtil.GetBigDataToString(5689523));
        print(TextUtil.GetBigDataToString(5869));
        print(TextUtil.GetBigDataToString(1));
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
