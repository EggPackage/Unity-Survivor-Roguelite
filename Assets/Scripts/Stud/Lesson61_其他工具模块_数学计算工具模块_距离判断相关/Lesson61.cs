using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson61 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点 实现数学计算工具类中的 距离判断相关 的方法
        //为什么要封装 距离判断相关 的方法

        //在进行游戏开发时
        //我们经常要去判断两点之间的距离
        //但是对于游戏中的对象，我们经常只需要判断它在某一个2D平面上的距离
        //比如
        //部分 2D游戏中，我们往往只关心 xy平面上的距离，z的变化不应该影响距离的计算
        //部分 3D游戏中，我们往往只关心 xz平面上的距离，y的变化不应该影响距离的计算

        //为了方便我们得到对象之间在某个平面的距离
        //我们可以为其封装一些方法，专门用于计算 XZ、XY 平面上两点的距离

        Vector3 src = new Vector3(1, 5, 0);
        Vector3 target = new Vector3(0, 19, 1);
        print(MathUtil.GetObjDistanceXZ(src, target));

        if(MathUtil.CheckObjDistanceXZ(src, target, 5))
        {
            print("两点间距离小于5");
        }

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
