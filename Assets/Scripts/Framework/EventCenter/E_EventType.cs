using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型 枚举
/// </summary>
public enum E_EventType
{
    /// <summary>
    /// 怪物死亡事件 —— 参数：Monster
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家获取奖励 —— 参数：int
    /// </summary>
    E_Player_GetReward,
    /// <summary>
    /// 测试用事件 —— 参数：无
    /// </summary>
    E_Test,
    /// <summary>
    /// 场景切换时进度变化读取
    /// </summary>
    E_SceneLoadChange,

    /// <summary>
    /// 输入系统：技能1 行为
    /// </summary>
    E_Input_Skill1,
    /// <summary>
    /// 输入系统：技能2 行为
    /// </summary>
    E_Input_Skill2,
    /// <summary>
    /// 输入系统：技能3 行为
    /// </summary>
    E_Input_Skill3,

    /// <summary>
    /// 水平轴 -1~1 事件传递
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// 垂直轴 -1~1 事件传递
    /// </summary>
    E_Input_Vertical,

    // ===== 吧台核心玩法事件（早咖晚酒 · 摆放系统） =====

    /// <summary>
    /// 吧台上摆放了一个物品 —— 参数：PlacedItem（新落位的物品）
    /// </summary>
    E_Bar_ItemPlaced,

    /// <summary>
    /// 吧台上移除了一个物品 —— 参数：PlacedItem（被移除的物品）
    /// </summary>
    E_Bar_ItemRemoved,

    /// <summary>
    /// 一次相邻 combo 结算完成 —— 参数：ComboResult（本次结算产出的所有效果）
    /// 白天经营用它算出杯效率，下午地牢用它算战斗数值，UI 用它播特效
    /// </summary>
    E_Bar_ComboResolved,

    // ===== 地牢玩法事件（早咖晚酒 · 下午情绪调和） =====

    /// <summary>
    /// 一次地牢战斗（情绪调和）结算完成 —— 参数：BattleResult
    /// 携带安抚值 / 执念阈值 / 是否化解 / 咖啡·酒分项，UI 用它显示胜负与救回居民
    /// </summary>
    E_Dungeon_BattleResolved,
}
