using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson65 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 必备知识点
        //范围检测 相关知识点（Unity四部曲之Unity基础中）
        #endregion

        #region 知识点 实现数学计算工具类中的 范围检测 相关方法
        //为什么要封装 范围检测 相关方法

        //范围检测是在游戏开发中经常会使用的功能
        //我们完全可以基于Unity的范围检测进行二次封装
        //让其使用起来更加的方便
        //可以直接通过委托回调的形式对获取到的内容进行操作

        //需求：
        //通过委托回调的形式获取到范围检测得到的对象
        //主要封装 盒装检测和球体检测 即可（胶囊检测使用很少）
        //可以指定获取到Collider、GameObject、指定脚本

        
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            MathUtil.OverlapBox<GameObject>(Vector3.zero,
                                        Quaternion.identity,
                                        0.5f * Vector3.one,
                                        1 << LayerMask.NameToLayer("Monster"),
                                        (obj) => {
                                            print(obj.name);
                                        });
        }
    }
}
