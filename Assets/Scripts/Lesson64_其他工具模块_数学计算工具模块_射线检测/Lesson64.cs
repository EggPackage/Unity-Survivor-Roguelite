using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson64 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //射线检测 相关知识点（Unity四部曲之Unity基础中）
        #endregion

        #region 知识点 实现数学计算工具类中的 射线检测 相关方法
        //为什么要封装 射线检测 相关方法

        //射线检测是在游戏开发中经常会使用的功能
        //我们完全可以基于Unity的射线检测进行二次封装
        //让其使用起来更加的方便
        //可以直接通过委托回调的形式对获取到的内容进行操作

        //需求：
        //通过委托回调的形式获取到射线检测得到的对象
        //可以有几种重载，分别获取到RaycastHit、GameObject、指定脚本
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //MathUtil.RayCast(Camera.main.ScreenPointToRay(Input.mousePosition), 
            //                GetRayCastObj, 
            //                1000, 
            //                1 << LayerMask.NameToLayer("Monster"));

            MathUtil.RayCastAll(Camera.main.ScreenPointToRay(Input.mousePosition),
                            GetRayCastObj,
                            1000,
                            1 << LayerMask.NameToLayer("Monster"));
        }
    }

    private void GetRayCastObj(GameObject obj)
    {
        print(obj.name);
    }
}
