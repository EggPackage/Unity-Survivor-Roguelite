using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器 主要用于开启、停止、重置等等操作的通用计时器
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    /// <summary>
    /// 用于记录当前需要分配的唯一ID号
    /// </summary>
    private int TIMER_KEY = 0;
    /// <summary>
    /// 用于存储管理所有计时器项的字典容器
    /// </summary>
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 用于存储管理所有计时器项的字典容器（不受Time.timeScale影响的计时器）
    /// </summary>
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 待移除列表
    /// </summary>
    private List<TimerItem> delList = new List<TimerItem>();

    //为了避免内存浪费 每次while中都新建
    //这里直接将它声明为成员变量
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);

    private Coroutine timer;
    private Coroutine realTimer;

    /// <summary>
    /// 计时器容器运行中的唯一间隔用的协同程序 的间隔时间
    /// </summary>
    private const float intervalTime = 0.1f;

    private TimerMgr()
    {
        Start();
    }

    //开启计时器容器功能的方法
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false, timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true, realTimerDic));
    }

    //关闭计时器容器功能的方法
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }

    IEnumerator StartTiming(bool isRealTime, Dictionary<int, TimerItem> timerDic)
    {
        while (true)
        {
            if (isRealTime)
                yield return waitForSecondsRealtime;
            else
                yield return waitForSeconds;
            foreach (TimerItem item in timerDic.Values)
            {
                if (!item.isRuning)
                    continue;
                if(item.callBack != null)
                {
                    item.intervalTime -= (int)(intervalTime*1000);
                    if(item.intervalTime <= 0)
                    {
                        item.callBack.Invoke();
                        item.intervalTime = item.maxIntervalTime;
                    }
                }
                item.allTime -= (int)(intervalTime * 1000);
                if(item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    delList.Add(item);
                }
            }

            for (int i = 0; i < delList.Count; i++)
            {
                timerDic.Remove(delList[i].keyID);
                PoolMgr.Instance.PushObj(delList[i]);
            }
            delList.Clear();
        }
    }

    /// <summary>
    /// 创建并添加计时器
    /// </summary>
    /// <param name="isRealTime">如果是true则不受Time.timeScale影响</param>
    /// <param name="allTime">总的时间 比如 1s=1000ms</param>
    /// <param name="overCallBack">计时结束后的回调</param>
    /// <param name="intervalTime">间隔计时时间 比如 1s=1000ms</param>
    /// <param name="callBack">间隔计时时间到后 回调</param>
    /// <returns>返回唯一ID 用于外部控制对应计时器</returns>
    public int CreateTimer(bool isRealTime, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        int keyID = ++TIMER_KEY;
        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        timerItem.InitInfo(keyID, allTime, overCallBack, intervalTime, callBack);
        if (isRealTime)
            realTimerDic.Add(keyID, timerItem);
        else
            timerDic.Add(keyID, timerItem);
        return keyID;
    }

    //移除单个计时器
    public void RemoveTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
        {
            PoolMgr.Instance.PushObj(timerDic[keyID]);
            timerDic.Remove(keyID);
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            PoolMgr.Instance.PushObj(realTimerDic[keyID]);
            realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void ResetTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }

    /// <summary>
    /// 开启单个计时器 主要用于暂停后重新开始
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StartTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = true;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = true;
        }
    }

    /// <summary>
    /// 停止单个计时器 主要用于暂停
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StopTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = false;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = false;
        }
    }
}
