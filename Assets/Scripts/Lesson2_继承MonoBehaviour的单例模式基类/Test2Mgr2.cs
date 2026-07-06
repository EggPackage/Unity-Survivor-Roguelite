using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2Mgr2 : SingletonAutoMono<Test2Mgr2>
{
    public void TestLog()
    {
        print("Test2Mgr2 ");
    }
}
