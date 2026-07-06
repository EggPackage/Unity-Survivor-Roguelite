using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson54 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //string中Split 相关知识点（C#四部曲中）
        #endregion

        #region 知识点 实现文本工具类中的 字符串拆分方法
        //为什么要封装字符串拆分方法

        //我们在配置表中
        //经常会配置如下格式的字符串数据
        //
        //2,11|3,12|4,55
        //遇到这种数据时，我们往往会使用string中的Split方法进行拆分

        //我们想要通过封装满足以下的需求
        //1.避免符号填写错误，英文字符填写成中文字符
        //2.能够直接拆分获取到int数组
        string str = "1,10;2,4;32,1";
        //string[] strs = TextUtil.SplitStr(str, 1);
        //for (int i = 0; i < strs.Length; i++)
        //{
        //    int[] ints = TextUtil.SplitStrToIntArr(strs[i], 2);
        //    print("道具ID" + ints[0]);
        //    print("道具数量" + ints[1]);
        //}

        TextUtil.SplitStrToIntArrTwice(str, 1, 2, (id, num) =>
        {
            print("道具ID" + id);
            print("道具数量" + num);
        });

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
