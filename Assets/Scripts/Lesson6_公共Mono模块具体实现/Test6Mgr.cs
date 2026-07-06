using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test6Mgr : BaseManager<Test6Mgr>
{
    private Coroutine testFun;
    private Test6Mgr()
    {

    }

    public void ICanUpdateAndCoroutine()
    {
        MonoMgr.Instance.AddUpdateListener(MyUpdate);

        testFun = MonoMgr.Instance.StartCoroutine(Test());
    }

    public void ICanStopUpdateAndCoroutine()
    {
        MonoMgr.Instance.RemoveUpdateListener(MyUpdate);

        MonoMgr.Instance.StopCoroutine(testFun);
    }


    private IEnumerator Test()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("TestTestTest");
    }


    private void MyUpdate()
    {
        Debug.Log("Test6Mgr");
    }
}
