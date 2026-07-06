using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayRemove : MonoBehaviour
{

    // Start is called before the first frame update
    private void OnEnable()
    {
        Invoke("RemoveMe", 1f);
    }

    private void RemoveMe()
    {
        PoolMgr.Instance.PushObj(this.gameObject);
    }
}
