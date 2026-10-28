using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson63 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //向量点乘、叉乘 相关知识点（Unity四部曲之Unity基础中）
        #endregion

        #region 知识点 实现数学计算工具类中的 扇形范围判断 的方法
        //为什么要封装 扇形范围判断 的方法

        //在进行游戏开发时
        //我们经常需要判断目标是否在自己前方的扇形范围内
        //用于触发下一步的行为，比如 触发攻击、触发伤害等等
        //因此我们可以把 扇形范围判断 封装为一个方法
        //方便外部使用

        //需求：
        //我们只需要实现判断XZ平面的扇形范围内即可

        Vector3 pos = Vector3.zero;
        Vector3 forward = Vector3.forward;

        Vector3 targetPos = new Vector3(1, 0, 1);

        if (MathUtil.IsInSectorRangeXZ(pos, forward, targetPos, 2, 60))
            print("目标在扇形范围内");
        else
            print("目标不在扇形范围内");

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
