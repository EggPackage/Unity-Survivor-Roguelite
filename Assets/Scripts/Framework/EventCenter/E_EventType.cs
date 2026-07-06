using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型 枚举
/// </summary>
public enum E_EventType 
{
    /// <summary>
    /// 怪物死亡事件 ?? 参数：Monster
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家获取奖励 ?? 参数：int
    /// </summary>
    E_Player_GetReward,
    /// <summary>
    /// 测试用事件 ?? 参数：无
    /// </summary>
    E_Test,
    /// <summary>
    /// 场景切换时进度变化获取
    /// </summary>
    E_SceneLoadChange,

    /// <summary>
    /// 输入系统触发技能1 行为
    /// </summary>
    E_Input_Skill1,
    /// <summary>
    /// 输入系统触发技能2 行为
    /// </summary>
    E_Input_Skill2,
    /// <summary>
    /// 输入系统触发技能3 行为
    /// </summary>
    E_Input_Skill3,

    /// <summary>
    /// 水平热键 -1~1的事件监听
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// 竖直热键 -1~1的事件监听
    /// </summary>
    E_Input_Vertical,

    // ===== 新增游戏事件 =====
    /// <summary>
    /// 存档加载完成事件
    /// </summary>
    E_SaveLoaded,
    /// <summary>
    /// 时间切换事件 ?? 参数：E_TimeOfDay
    /// </summary>
    E_TimeOfDayChange,
    /// <summary>
    /// 进入新的一天事件 ?? 参数：int dayCount
    /// </summary>
    E_NewDay,
    /// <summary>
    /// NPC被救援事件 ?? 参数：string npcId
    /// </summary>
    E_NPCRescued,
    /// <summary>
    /// 史莱姆被收容事件
    /// </summary>
    E_SlimeCaptured,
    /// <summary>
    /// 店铺等级提升事件 ?? 参数：int newLevel
    /// </summary>
    E_ShopLevelUp,
    /// <summary>
    /// 天赋升级事件 ?? 参数：string talentType
    /// </summary>
    E_TalentLevelUp,
    /// <summary>
    /// 配方解锁事件 ?? 参数：string recipeId
    /// </summary>
    E_RecipeUnlocked,
    /// <summary>
    /// 地牢通关事件 ?? 参数：int floor
    /// </summary>
    E_DungeonCleared,
    /// <summary>
    /// 清醒值变化事件 ?? 参数：int currentAwareness
    /// </summary>
    E_AwarenessChanged,
    /// <summary>
    /// 金币变化事件 ?? 参数：int gold
    /// </summary>
    E_GoldChanged,
    /// <summary>
    /// 精华变化事件 ?? 参数：int essence
    /// </summary>
    E_EssenceChanged,
    /// <summary>
    /// 背包变化事件
    /// </summary>
    E_InventoryChanged,
    /// <summary>
    /// 仓库变化事件
    /// </summary>
    E_WarehouseChanged,
    /// <summary>
    /// 任务完成事件 ?? 参数：string questId
    /// </summary>
    E_QuestCompleted,
    /// <summary>
    /// 结局触发事件 ?? 参数：bool isTrueEnding
    /// </summary>
    E_EndingTriggered,

    // ===== 每日随机事件 =====
    /// <summary>
    /// 采集翻倍事件
    /// </summary>
    MaterialDouble,
    /// <summary>
    /// 魔物潮事件
    /// </summary>
    MonsterSurge,
    /// <summary>
    /// 大量被困村民事件
    /// </summary>
    ManyTrappedNPCs,
    /// <summary>
    /// 普通日事件
    /// </summary>
    NormalDay,
}