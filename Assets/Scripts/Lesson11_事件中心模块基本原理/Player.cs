using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener<Monster>(E_EventType.E_Monster_Dead, PlayerWaitMonsterDeadDo);

        EventCenter.Instance.AddEventListener(E_EventType.E_Test, Test);
    }

    public void Test()
    {
        print("无参事件监听者");
    }

    public void PlayerWaitMonsterDeadDo(Monster info)
    {
        Debug.Log("玩家得奖励" + info.monsterName);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<Monster>(E_EventType.E_Monster_Dead, PlayerWaitMonsterDeadDo);

        EventCenter.Instance.RemoveEventListener(E_EventType.E_Test, Test);
    }
}
