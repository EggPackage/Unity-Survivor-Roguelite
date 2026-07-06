// 新建文件：F:/UnityTest/MyGameFrame/MyGameFrame/Assets/Scripts/Framework/Time/TimeMgr.cs
using UnityEngine;

public class TimeMgr : BaseManager<TimeMgr>
{
    private E_TimeOfDay currentTime;
    private int currentDay;
    
    private TimeMgr()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_SaveLoaded, OnSaveLoaded);
    }
    
    public E_TimeOfDay CurrentTime => currentTime;
    public int CurrentDay => currentDay;
    public bool IsDungeonTime => currentTime == E_TimeOfDay.Dawn;
    public bool IsCafeTime => currentTime == E_TimeOfDay.Day;
    public bool IsTavernTime => currentTime == E_TimeOfDay.Night;
    public bool IsSettlementTime => currentTime == E_TimeOfDay.LateNight;
    
    public void InitTime(E_TimeOfDay time, int day)
    {
        currentTime = time;
        currentDay = day;
        BroadcastTimeChange();
    }
    
    public void AdvanceTime()
    {
        SaveMgr.Instance.AdvanceTime();
    }
    
    private void BroadcastTimeChange()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_TimeOfDayChange, currentTime);
    }
    
    private void OnSaveLoaded()
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        currentTime = dailyData.currentTime;
        currentDay = dailyData.dayCount;
        BroadcastTimeChange();
    }
}
