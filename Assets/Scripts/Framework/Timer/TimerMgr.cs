using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器 主要用于开启、停止、重置等等操作来管理计时器
/// 同时集成昼夜时间管理功能
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    /// <summary>
    /// 用于记录当前将要创建的唯一ID的
    /// </summary>
    private int TIMER_KEY = 0;
    /// <summary>
    /// 用于存储管理所有计时器的字典容器
    /// </summary>
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 用于存储管理所有计时器的字典容器（不受Time.timeScale影响的计时器）
    /// </summary>
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 待移除列表
    /// </summary>
    private List<TimerItem> delList = new List<TimerItem>();

    //为了避免内存的浪费 每次while都会生成 
    //我们直接将其声明为成员变量
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);

    private Coroutine timer;
    private Coroutine realTimer;

    /// <summary>
    /// 计时器管理器中的唯一计时用的协同程序 的间隔时间
    /// </summary>
    private const float intervalTime = 0.1f;

    // ===== 昼夜时间管理相关 =====
    /// <summary>
    /// 当前时间段
    /// </summary>
    private E_TimeOfDay currentTimeOfDay = E_TimeOfDay.Dawn;
    /// <summary>
    /// 当前天数
    /// </summary>
    private int currentDay = 1;
    /// <summary>
    /// 每个时间段持续时间（秒）
    /// </summary>
    private const int TIME_PHASE_DURATION = 30;
    /// <summary>
    /// 当前时间段剩余时间（秒）
    /// </summary>
    private float currentPhaseRemainingTime = TIME_PHASE_DURATION;
    /// <summary>
    /// 时间流逝速度倍率
    /// </summary>
    private float timeSpeed = 1f;
    /// <summary>
    /// 时间是否暂停
    /// </summary>
    private bool isTimePaused = false;
    /// <summary>
    /// 昼夜循环计时器ID
    /// </summary>
    private int dayNightTimerId = 0;

    private TimerMgr() 
    {
        Start();
    }

    //开启计时器管理器的方法
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false, timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true, realTimerDic));
        StartDayNightCycle();
    }

    //关闭计时器管理器的方法
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
        StopDayNightCycle();
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
    /// 创建单个计时器
    /// </summary>
    /// <param name="isRealTime">如果是true不受Time.timeScale影响</param>
    /// <param name="allTime">总的时间 毫秒 1s=1000ms</param>
    /// <param name="overCallBack">总时间结束回调</param>
    /// <param name="intervalTime">间隔计时时间 毫秒 1s=1000ms</param>
    /// <param name="callBack">间隔计时时间结束 回调</param>
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
    /// 开启当个计时器 主要用于暂停后重新开始
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

    // ===== 昼夜时间管理方法 =====

    /// <summary>
    /// 启动昼夜循环
    /// </summary>
    private void StartDayNightCycle()
    {
        if (dayNightTimerId != 0)
        {
            RemoveTimer(dayNightTimerId);
        }
        dayNightTimerId = CreateTimer(true, int.MaxValue, null, 1000, UpdateDayNightCycle);
    }

    /// <summary>
    /// 停止昼夜循环
    /// </summary>
    private void StopDayNightCycle()
    {
        if (dayNightTimerId != 0)
        {
            RemoveTimer(dayNightTimerId);
            dayNightTimerId = 0;
        }
    }

    /// <summary>
    /// 更新昼夜循环
    /// </summary>
    private void UpdateDayNightCycle()
    {
        if (isTimePaused)
            return;

        currentPhaseRemainingTime -= timeSpeed;

        if (currentPhaseRemainingTime <= 0)
        {
            AdvanceTime();
        }
    }

    /// <summary>
    /// 推进到下一时间段
    /// </summary>
    public void AdvanceTime()
    {
        switch (currentTimeOfDay)
        {
            case E_TimeOfDay.Dawn:
                currentTimeOfDay = E_TimeOfDay.Day;
                break;
            case E_TimeOfDay.Day:
                currentTimeOfDay = E_TimeOfDay.Night;
                break;
            case E_TimeOfDay.Night:
                currentTimeOfDay = E_TimeOfDay.LateNight;
                break;
            case E_TimeOfDay.LateNight:
                NewDay();
                break;
        }

        currentPhaseRemainingTime = TIME_PHASE_DURATION;

        EventCenter.Instance.EventTrigger(E_EventType.E_TimeOfDayChange, currentTimeOfDay);
    }

    /// <summary>
    /// 进入新的一天
    /// </summary>
    private void NewDay()
    {
        currentDay++;
        currentTimeOfDay = E_TimeOfDay.Dawn;

        EventCenter.Instance.EventTrigger(E_EventType.E_NewDay, currentDay);
    }

    /// <summary>
    /// 获取当前时间段
    /// </summary>
    public E_TimeOfDay GetCurrentTimeOfDay()
    {
        return currentTimeOfDay;
    }

    /// <summary>
    /// 获取当前天数
    /// </summary>
    public int GetCurrentDay()
    {
        return currentDay;
    }

    /// <summary>
    /// 获取当前时间段剩余时间（秒）
    /// </summary>
    public float GetPhaseRemainingTime()
    {
        return currentPhaseRemainingTime;
    }

    /// <summary>
    /// 获取时间段总时长（秒）
    /// </summary>
    public int GetPhaseDuration()
    {
        return TIME_PHASE_DURATION;
    }

    /// <summary>
    /// 设置时间流逝速度
    /// </summary>
    /// <param name="speed">速度倍率</param>
    public void SetTimeSpeed(float speed)
    {
        timeSpeed = Mathf.Max(0, speed);
    }

    /// <summary>
    /// 获取当前时间速度
    /// </summary>
    public float GetTimeSpeed()
    {
        return timeSpeed;
    }

    /// <summary>
    /// 暂停时间
    /// </summary>
    public void PauseTime()
    {
        isTimePaused = true;
    }

    /// <summary>
    /// 恢复时间
    /// </summary>
    public void ResumeTime()
    {
        isTimePaused = false;
    }

    /// <summary>
    /// 直接设置时间
    /// </summary>
    /// <param name="day">天数</param>
    /// <param name="timeOfDay">时间段</param>
    public void SetTime(int day, E_TimeOfDay timeOfDay)
    {
        currentDay = day;
        currentTimeOfDay = timeOfDay;
        currentPhaseRemainingTime = TIME_PHASE_DURATION;

        EventCenter.Instance.EventTrigger(E_EventType.E_TimeOfDayChange, currentTimeOfDay);
    }

    /// <summary>
    /// 直接设置时间段
    /// </summary>
    /// <param name="timeOfDay">时间段</param>
    public void SetTimeOfDay(E_TimeOfDay timeOfDay)
    {
        currentTimeOfDay = timeOfDay;
        currentPhaseRemainingTime = TIME_PHASE_DURATION;
        EventCenter.Instance.EventTrigger(E_EventType.E_TimeOfDayChange, currentTimeOfDay);
    }
}