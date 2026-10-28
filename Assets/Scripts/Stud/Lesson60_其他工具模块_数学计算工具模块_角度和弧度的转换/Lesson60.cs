using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson60 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现数学计算工具类中的 角度和弧度相互转换 的方法
        //为什么要封装 角度和弧度相互转换 的方法

        //虽然Unity中的Mathf类中已经有 Mathf.Deg2Rad 、 Mathf.Rad2Deg 相关的系数
        //但是为了避免使用错误
        //我们完全可以将其封装为方法
        //外部只需传入角度或弧度 便可以得到结果
        //无需在外部进行乘法运算

        //让这些常规操作使用起来更加的方便

        float rad = MathUtil.Deg2Rad(60);
        print(rad);
        print(MathUtil.Rad2Deg(rad));
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
