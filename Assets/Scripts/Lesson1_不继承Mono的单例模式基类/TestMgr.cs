using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMgr:BaseManager<TestMgr>
{
    private List<object> list = new List<object>();

    private TestMgr()
    {
        if (!InstanceisNull)
            Debug.LogError("已经存在单例模式对象，请不要再实例化了");
    }

    public void TestLog()
    {
        Debug.Log("TestMgr");
    }

    public void AddData(object data)
    {
        lock(lockObj)
        {
            list.Add(data);
        }
            
    }
}
