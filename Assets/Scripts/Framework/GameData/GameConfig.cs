// 新建文件：Assets/Scripts/Game/Data/GameData.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏全局配置数据（外部JSON配置）
/// </summary>
[System.Serializable]
public class GameConfig
{
    /// <summary>
    /// 店铺配置
    /// </summary>
    public ShopConfig shopConfig;
    /// <summary>
    /// 地牢配置
    /// </summary>
    public DungeonConfig dungeonConfig;
    /// <summary>
    /// 饮品配置
    /// </summary>
    public DrinkConfig drinkConfig;
    /// <summary>
    /// 魔物配置
    /// </summary>
    public MonsterConfig monsterConfig;
    /// <summary>
    /// NPC配置
    /// </summary>
    public NPCConfig npcConfig;
}

/// <summary>
/// 店铺配置数据
/// </summary>
[System.Serializable]
public class ShopConfig
{
    /// <summary>
    /// 最高等级
    /// </summary>
    public int maxLevel = 5;
    /// <summary>
    /// 升级所需金币
    /// </summary>
    public int[] levelUpCost;
    /// <summary>
    /// 各等级仓库容量
    /// </summary>
    public int[] warehouseCapacityByLevel;
    /// <summary>
    /// 各等级地牢携带上限
    /// </summary>
    public int[] dungeonCarryLimitByLevel;
    /// <summary>
    /// 各等级种植槽数量
    /// </summary>
    public int[] fieldSlotsByLevel;
    /// <summary>
    /// 各等级酿造桶数量
    /// </summary>
    public int[] barrelSlotsByLevel;
}

/// <summary>
/// 地牢配置数据
/// </summary>
[System.Serializable]
public class DungeonConfig
{
    /// <summary>
    /// 最大层数
    /// </summary>
    public int maxFloor = 4;
    /// <summary>
    /// 每步基础清醒值消耗
    /// </summary>
    public int baseAwarenessCostPerStep;
    /// <summary>
    /// 每次攻击基础清醒值消耗
    /// </summary>
    public int baseAwarenessCostPerAttack;
    /// <summary>
    /// 每次被攻击基础清醒值消耗
    /// </summary>
    public int baseAwarenessCostPerHit;
    /// <summary>
    /// 救援NPC客流加成倍率
    /// </summary>
    public float rescueNpcBonusMultiplier;
}

/// <summary>
/// 饮品配置数据
/// </summary>
[System.Serializable]
public class DrinkConfig
{
    /// <summary>
    /// 咖啡配方列表
    /// </summary>
    public List<DrinkData> coffeeRecipes;
    /// <summary>
    /// 酒水配方列表
    /// </summary>
    public List<DrinkData> wineRecipes;
}

/// <summary>
/// 饮品数据
/// </summary>
[System.Serializable]
public class DrinkData
{
    /// <summary>
    /// 配方ID
    /// </summary>
    public string id;
    /// <summary>
    /// 饮品名称
    /// </summary>
    public string name;
    /// <summary>
    /// 售价
    /// </summary>
    public int sellPrice;
    /// <summary>
    /// 清醒值恢复量
    /// </summary>
    public float awarenessRestore;
    /// <summary>
    /// 伤害加成
    /// </summary>
    public float damageBonus;
    /// <summary>
    /// 控制效果
    /// </summary>
    public float controlEffect;
    /// <summary>
    /// 救援效果
    /// </summary>
    public float rescueEffect;
    /// <summary>
    /// 所需材料ID列表
    /// </summary>
    public List<string> requiredMaterials;
    /// <summary>
    /// 描述文本
    /// </summary>
    public string description;
    /// <summary>
    /// 是否可售卖
    /// </summary>
    public bool canSell;
    /// <summary>
    /// 是否可在地牢使用
    /// </summary>
    public bool canUseInDungeon;
    /// <summary>
    /// 是否可用于救援
    /// </summary>
    public bool canRescue;
    /// <summary>
    /// 饮品类型
    /// </summary>
    public E_DrinkType drinkType;
}

/// <summary>
/// 饮品类型枚举
/// </summary>
public enum E_DrinkType
{
    /// <summary>
    /// 咖啡 - 克制醉行尸
    /// </summary>
    Coffee,
    /// <summary>
    /// 酒水 - 克制渊灵
    /// </summary>
    Wine,
    /// <summary>
    /// 中性 - 无特殊克制
    /// </summary>
    Neutral
}

/// <summary>
/// 魔物配置数据
/// </summary>
[System.Serializable]
public class MonsterConfig
{
    /// <summary>
    /// 魔物列表
    /// </summary>
    public List<MonsterData> monsters;
}

/// <summary>
/// 魔物数据
/// </summary>
[System.Serializable]
public class MonsterData
{
    /// <summary>
    /// 魔物ID
    /// </summary>
    public string id;
    /// <summary>
    /// 魔物名称
    /// </summary>
    public string name;
    /// <summary>
    /// 生命值
    /// </summary>
    public int health;
    /// <summary>
    /// 伤害值
    /// </summary>
    public int damage;
    /// <summary>
    /// 清醒值吸取量
    /// </summary>
    public float awarenessDrain;
    /// <summary>
    /// 出现层数
    /// </summary>
    public int floor;
    /// <summary>
    /// 魔物类型
    /// </summary>
    public E_MonsterType monsterType;
    /// <summary>
    /// 咖啡抗性
    /// </summary>
    public float coffeeResistance;
    /// <summary>
    /// 烈酒抗性
    /// </summary>
    public float wineResistance;
    /// <summary>
    /// 经验奖励
    /// </summary>
    public int expReward;
    /// <summary>
    /// 精华奖励
    /// </summary>
    public int essenceReward;
}

/// <summary>
/// 魔物类型枚举
/// </summary>
public enum E_MonsterType
{
    /// <summary>
    /// 醉行尸 - 咖啡克制
    /// </summary>
    DrunkZombie,
    /// <summary>
    /// 渊灵 - 烈酒克制
    /// </summary>
    AbyssSpirit,
    /// <summary>
    /// BOSS - 双属性
    /// </summary>
    Boss,
    /// <summary>
    /// 友好史莱姆
    /// </summary>
    Friendly
}

/// <summary>
/// NPC配置数据
/// </summary>
[System.Serializable]
public class NPCConfig
{
    /// <summary>
    /// NPC列表
    /// </summary>
    public List<NPCData> npcs;
}

/// <summary>
/// NPC数据
/// </summary>
[System.Serializable]
public class NPCData
{
    /// <summary>
    /// NPC ID
    /// </summary>
    public string id;
    /// <summary>
    /// NPC名称
    /// </summary>
    public string name;
    /// <summary>
    /// 职业
    /// </summary>
    public string profession;
    /// <summary>
    /// 基础客流加成
    /// </summary>
    public int baseCustomerBonus;
    /// <summary>
    /// 每日奖励类型
    /// </summary>
    public string dailyRewardType;
    /// <summary>
    /// 每日奖励数量
    /// </summary>
    public int dailyRewardAmount;
    /// <summary>
    /// 救援所需饮品ID
    /// </summary>
    public string rescueRequiredDrink;
}

/// <summary>
/// 永久养成数据（跨天保留）
/// </summary>
[System.Serializable]
public class PermanentData
{
    /// <summary>
    /// 店铺等级
    /// </summary>
    public int shopLevel = 1;
    /// <summary>
    /// 晨昏精华
    /// </summary>
    public int dawnEssence = 0;
    /// <summary>
    /// 天赋数据
    /// </summary>
    public TalentData talents = new TalentData();
    /// <summary>
    /// 已解锁配方列表
    /// </summary>
    public List<string> unlockedRecipes = new List<string>();
    /// <summary>
    /// 已救援NPC列表
    /// </summary>
    public List<string> rescuedNPCs = new List<string>();
    /// <summary>
    /// 是否拥有史莱姆
    /// </summary>
    public bool hasSlime = false;
    /// <summary>
    /// 当前地牢层数
    /// </summary>
    public int currentFloor = 1;
    /// <summary>
    /// 最高通关层数
    /// </summary>
    public int maxClearedFloor = 0;
    /// <summary>
    /// 是否解锁真结局
    /// </summary>
    public bool trueEndingUnlocked = false;
    /// <summary>
    /// 是否解锁坏结局
    /// </summary>
    public bool badEndingUnlocked = false;
}

/// <summary>
/// 天赋数据
/// </summary>
[System.Serializable]
public class TalentData
{
    // ===== 咖啡系天赋 (0-5级) =====
    /// <summary>
    /// 清醒值上限提升
    /// </summary>
    public int coffeeAwarenessMax = 0;
    /// <summary>
    /// 咖啡buff时长提升
    /// </summary>
    public int coffeeBuffDuration = 0;
    /// <summary>
    /// 咖啡售价提升
    /// </summary>
    public int coffeePriceBonus = 0;
    
    // ===== 酿酒系天赋 (0-5级) =====
    /// <summary>
    /// 酒水控制效果提升
    /// </summary>
    public int wineControlEffect = 0;
    /// <summary>
    /// 酿造产量提升
    /// </summary>
    public int wineProductionBonus = 0;
    /// <summary>
    /// 酒馆客单价提升
    /// </summary>
    public int winePriceBonus = 0;
    
    // ===== 采集系天赋 (0-5级) =====
    /// <summary>
    /// 素材掉落率提升
    /// </summary>
    public int materialDropRate = 0;
    /// <summary>
    /// 移动速度提升
    /// </summary>
    public int movementSpeed = 0;
    /// <summary>
    /// 救援饮品消耗降低
    /// </summary>
    public int rescueDrinkCostReduction = 0;
}

/// <summary>
/// 当日临时数据（每天重置）
/// </summary>
[System.Serializable]
public class DailyData
{
    /// <summary>
    /// 当前天数
    /// </summary>
    public int dayCount = 1;
    /// <summary>
    /// 当前时间段
    /// </summary>
    public E_TimeOfDay currentTime = E_TimeOfDay.Dawn;
    /// <summary>
    /// 当日随机事件
    /// </summary>
    public DailyEventData currentEvent;
    /// <summary>
    /// 当前清醒值
    /// </summary>
    public int awareness = 100;
    /// <summary>
    /// 最大清醒值
    /// </summary>
    public int maxAwareness = 100;
    /// <summary>
    /// 随身背包
    /// </summary>
    public InventoryData inventory = new InventoryData();
    /// <summary>
    /// 店铺仓库
    /// </summary>
    public WarehouseData warehouse = new WarehouseData();
    /// <summary>
    /// 种植田数据
    /// </summary>
    public FieldData field = new FieldData();
    /// <summary>
    /// 酿造桶数据
    /// </summary>
    public BarrelData barrel = new BarrelData();
    /// <summary>
    /// 今日咖啡馆服务顾客数
    /// </summary>
    public int cafeCustomersServed = 0;
    /// <summary>
    /// 今日酒馆服务顾客数
    /// </summary>
    public int tavernCustomersServed = 0;
    /// <summary>
    /// 今日赚取金币
    /// </summary>
    public int goldEarnedToday = 0;
    /// <summary>
    /// 今日赚取精华
    /// </summary>
    public int essenceEarnedToday = 0;
    /// <summary>
    /// 已完成任务列表
    /// </summary>
    public List<string> completedQuests = new List<string>();
    /// <summary>
    /// 是否已通关地牢
    /// </summary>
    public bool hasClearedDungeonToday = false;
}

/// <summary>
/// 时间段枚举
/// </summary>
public enum E_TimeOfDay
{
    /// <summary>
    /// 清晨 - 地牢探索
    /// </summary>
    Dawn,
    /// <summary>
    /// 白天 - 咖啡馆经营
    /// </summary>
    Day,
    /// <summary>
    /// 夜晚 - 酒馆经营
    /// </summary>
    Night,
    /// <summary>
    /// 深夜 - 结算养成
    /// </summary>
    LateNight
}

/// <summary>
/// 每日事件数据
/// </summary>
[System.Serializable]
public class DailyEventData
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public string id;
    /// <summary>
    /// 事件描述
    /// </summary>
    public string description;
    /// <summary>
    /// 事件类型
    /// </summary>
    public E_EventType eventType;
    /// <summary>
    /// 事件倍率
    /// </summary>
    public float multiplier;
}

/// <summary>
/// 背包数据
/// </summary>
[System.Serializable]
public class InventoryData
{
    /// <summary>
    /// 物品列表
    /// </summary>
    public List<ItemStack> items = new List<ItemStack>();
    /// <summary>
    /// 背包容量
    /// </summary>
    public int capacity = 20;
}

/// <summary>
/// 仓库数据
/// </summary>
[System.Serializable]
public class WarehouseData
{
    /// <summary>
    /// 物品列表
    /// </summary>
    public List<ItemStack> items = new List<ItemStack>();
    /// <summary>
    /// 仓库容量
    /// </summary>
    public int capacity = 100;
    /// <summary>
    /// 是否开启自动分拣
    /// </summary>
    public bool autoSortEnabled = false;
}

/// <summary>
/// 物品堆叠数据
/// </summary>
[System.Serializable]
public class ItemStack
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public string itemId;
    /// <summary>
    /// 数量
    /// </summary>
    public int quantity;
}

/// <summary>
/// 种植田数据
/// </summary>
[System.Serializable]
public class FieldData
{
    /// <summary>
    /// 种植槽列表
    /// </summary>
    public List<PlantSlot> slots = new List<PlantSlot>();
    /// <summary>
    /// 最大槽位数量
    /// </summary>
    public int maxSlots = 4;
}

/// <summary>
/// 种植槽数据
/// </summary>
[System.Serializable]
public class PlantSlot
{
    /// <summary>
    /// 种子ID
    /// </summary>
    public string seedId;
    /// <summary>
    /// 已生长天数
    /// </summary>
    public int growthDays;
    /// <summary>
    /// 总生长天数
    /// </summary>
    public int totalGrowthDays;
    /// <summary>
    /// 是否可收获
    /// </summary>
    public bool readyToHarvest;
}

/// <summary>
/// 酿造桶数据
/// </summary>
[System.Serializable]
public class BarrelData
{
    /// <summary>
    /// 酿造槽列表
    /// </summary>
    public List<BrewSlot> slots = new List<BrewSlot>();
    /// <summary>
    /// 最大槽位数量
    /// </summary>
    public int maxSlots = 4;
}

/// <summary>
/// 酿造槽数据
/// </summary>
[System.Serializable]
public class BrewSlot
{
    /// <summary>
    /// 配方ID
    /// </summary>
    public string recipeId;
    /// <summary>
    /// 剩余天数
    /// </summary>
    public int daysRemaining;
    /// <summary>
    /// 总天数
    /// </summary>
    public int totalDays;
}

/// <summary>
/// 存档数据容器
/// </summary>
[System.Serializable]
public class SaveData
{
    /// <summary>
    /// 永久数据
    /// </summary>
    public PermanentData permanentData;
    /// <summary>
    /// 当日数据
    /// </summary>
    public DailyData dailyData;
    /// <summary>
    /// 存档时间
    /// </summary>
    public string saveTime;
    /// <summary>
    /// 版本号
    /// </summary>
    public string version;
}

/// <summary>
/// 存档信息（用于存档选择界面）
/// </summary>
public class SaveInfo
{
    /// <summary>
    /// 存档槽位
    /// </summary>
    public int slot;
    /// <summary>
    /// 当前天数
    /// </summary>
    public int dayCount;
    /// <summary>
    /// 店铺等级
    /// </summary>
    public int shopLevel;
    /// <summary>
    /// 最高通关层数
    /// </summary>
    public int maxClearedFloor;
    /// <summary>
    /// 存档时间
    /// </summary>
    public string saveTime;
}
