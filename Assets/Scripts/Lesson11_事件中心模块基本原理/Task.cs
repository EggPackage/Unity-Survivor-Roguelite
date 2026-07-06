using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener<Monster>(E_EventType.E_Monster_Dead, TaskWaitMonsterDeadDo);
    }

    public void TaskWaitMonsterDeadDo(Monster info)
    {
        Debug.Log("ÈÎÎñ¼ÇÂ¼" + info.monsterName);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<Monster>(E_EventType.E_Monster_Dead, TaskWaitMonsterDeadDo);
    }
}
