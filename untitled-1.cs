// 修改文件：F:/UnityTest/MyGameFrame/MyGameFrame/Assets/Scripts/Framework/EventCenter/E_EventType.cs
public enum E_EventType 
{
    // ===== 原有事件 =====
    E_Monster_Dead,
    E_Player_GetReward,
    E_Test,
    E_SceneLoadChange,
    E_Input_Skill1,
    E_Input_Skill2,
    E_Input_Skill3,
    E_Input_Horizontal,
    E_Input_Vertical,
    
    // ===== 新增游戏事件 =====
    E_SaveLoaded,
    E_TimeOfDayChange,
    E_NewDay,
    E_NPCRescued,
    E_SlimeCaptured,
    E_ShopLevelUp,
    E_TalentLevelUp,
    E_RecipeUnlocked,
    E_DungeonCleared,
    E_AwarenessChanged,
    E_GoldChanged,
    E_EssenceChanged,
    E_InventoryChanged,
    E_WarehouseChanged,
    E_QuestCompleted,
    E_EndingTriggered,
    
    // ===== 每日随机事件 =====
    MaterialDouble,
    MonsterSurge,
    ManyTrappedNPCs,
    NormalDay
}
