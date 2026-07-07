using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不允许在同一个对象上挂载多个相同脚本
[DisallowMultipleComponent]
public class Test2Mgr : SingletonMono<Test2Mgr>
{
    protected override void Awake()
    {
        //重写Awake时 这一定不能省略或删除
        base.Awake();
    }

}
