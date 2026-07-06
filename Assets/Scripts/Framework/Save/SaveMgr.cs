// 新建文件：Assets/Scripts/Framework/Save/SaveMgr.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 存档管理系统 - 使用现有BaseManager单例框架
/// </summary>
public class SaveMgr : BaseManager<SaveMgr>
{
    /// <summary>
    /// 存档文件夹名称
    /// </summary>
    private const string SAVE_FOLDER = "GameSaves";
    /// <summary>
    /// 配置文件名称
    /// </summary>
    private const string CONFIG_FILE = "GameConfig.json";
    /// <summary>
    /// 最大存档槽位数
    /// </summary>
    private const int MAX_SAVE_SLOTS = 3;
    
    /// <summary>
    /// 存档路径
    /// </summary>
    private string savePath;
    /// <summary>
    /// 配置文件路径
    /// </summary>
    private string configPath;
    
    /// <summary>
    /// 当前加载的存档槽位
    /// </summary>
    private int currentSlot = 0;
    
    /// <summary>
    /// 游戏配置数据
    /// </summary>
    private GameConfig gameConfig;
    /// <summary>
    /// 永久养成数据
    /// </summary>
    private PermanentData permanentData;
    /// <summary>
    /// 当日临时数据
    /// </summary>
    private DailyData dailyData;
    
    /// <summary>
    /// 私有构造函数（单例模式）
    /// </summary>
    private SaveMgr()
    {
        // 初始化路径
        savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        configPath = Path.Combine(Application.streamingAssetsPath, CONFIG_FILE);
        
        // 确保存档目录存在
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        // 确保配置目录存在
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        
        // 加载全局配置
        LoadConfig();
    }
    
    /// <summary>
    /// 加载全局配置表
    /// </summary>
    private void LoadConfig()
    {
        if (File.Exists(configPath))
        {
            try
            {
                string json = File.ReadAllText(configPath);
                gameConfig = JsonUtility.FromJson<GameConfig>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置文件失败，使用默认配置: {e.Message}");
                gameConfig = CreateDefaultConfig();
                SaveConfig();
            }
        }
        else
        {
            // 使用默认配置
            gameConfig = CreateDefaultConfig();
            SaveConfig();
        }
    }
    
    /// <summary>
    /// 保存配置（开发调试用）
    /// </summary>
    public void SaveConfig()
    {
        string json = JsonUtility.ToJson(gameConfig, true);
        File.WriteAllText(configPath, json);
    }
    
    /// <summary>
    /// 创建默认配置
    /// </summary>
    private GameConfig CreateDefaultConfig()
    {
        GameConfig config = new GameConfig();
        
        // 店铺配置
        config.shopConfig = new ShopConfig
        {
            maxLevel = 5,
            levelUpCost = new[] { 0, 500, 1500, 3000, 5000 },
            warehouseCapacityByLevel = new[] { 100, 150, 200, 300, 500 },
            dungeonCarryLimitByLevel = new[] { 20, 30, 40, 50, 60 },
            fieldSlotsByLevel = new[] { 4, 6, 8, 10, 12 },
            barrelSlotsByLevel = new[] { 4, 6, 8, 10, 12 }
        };
        
        // 地牢配置
        config.dungeonConfig = new DungeonConfig
        {
            maxFloor = 4,
            baseAwarenessCostPerStep = 1,
            baseAwarenessCostPerAttack = 3,
            baseAwarenessCostPerHit = 5,
            rescueNpcBonusMultiplier = 1.1f
        };
        
        // 饮品配置（初始为空，通过JSON配置）
        config.drinkConfig = new DrinkConfig
        {
            coffeeRecipes = new List<DrinkData>(),
            wineRecipes = new List<DrinkData>()
        };
        
        // 魔物配置（初始为空，通过JSON配置）
        config.monsterConfig = new MonsterConfig
        {
            monsters = new List<MonsterData>()
        };
        
        // NPC配置（初始为空，通过JSON配置）
        config.npcConfig = new NPCConfig
        {
            npcs = new List<NPCData>()
        };
        
        return config;
    }
    
    /// <summary>
    /// 获取全局配置
    /// </summary>
    public GameConfig GetConfig()
    {
        return gameConfig;
    }
    
    /// <summary>
    /// 获取永久数据
    /// </summary>
    public PermanentData GetPermanentData()
    {
        return permanentData;
    }
    
    /// <summary>
    /// 获取当日数据
    /// </summary>
    public DailyData GetDailyData()
    {
        return dailyData;
    }
    
    /// <summary>
    /// 新建游戏存档
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    public void NewGame(int slot)
    {
        if (slot < 0 || slot >= MAX_SAVE_SLOTS)
            return;
        
        currentSlot = slot;
        permanentData = new PermanentData();
        dailyData = new DailyData();
        
        // 初始化每日数据
        dailyData.maxAwareness = 100 + permanentData.talents.coffeeAwarenessMax * 10;
        dailyData.awareness = dailyData.maxAwareness;
        dailyData.inventory.capacity = gameConfig.shopConfig.dungeonCarryLimitByLevel[0];
        dailyData.warehouse.capacity = gameConfig.shopConfig.warehouseCapacityByLevel[0];
        dailyData.field.maxSlots = gameConfig.shopConfig.fieldSlotsByLevel[0];
        dailyData.barrel.maxSlots = gameConfig.shopConfig.barrelSlotsByLevel[0];
        
        // 随机生成当日事件
        dailyData.currentEvent = GenerateDailyEvent();
        
        // 自动存档
        AutoSave();
        
        // 触发存档加载事件
        EventCenter.Instance.EventTrigger(E_EventType.E_SaveLoaded);
    }
    
    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    /// <returns>是否加载成功</returns>
    public bool LoadGame(int slot)
    {
        if (slot < 0 || slot >= MAX_SAVE_SLOTS)
            return false;
        
        string path = GetSlotPath(slot);
        
        if (!File.Exists(path))
            return false;
        
        try
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
            // 坏档校验
            if (saveData == null || saveData.permanentData == null || saveData.dailyData == null)
                return false;
            
            currentSlot = slot;
            permanentData = saveData.permanentData;
            dailyData = saveData.dailyData;
            
            // 检查史莱姆是否已收容（解锁自动分拣）
            dailyData.warehouse.autoSortEnabled = permanentData.hasSlime;
            
            // 触发存档加载事件
            EventCenter.Instance.EventTrigger(E_EventType.E_SaveLoaded);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载存档失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 自动存档（地牢通关、深夜结算时调用）
    /// </summary>
    public void AutoSave()
    {
        Save(currentSlot);
    }
    
    /// <summary>
    /// 手动保存到指定槽位
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    /// <returns>是否保存成功</returns>
    public bool Save(int slot)
    {
        if (slot < 0 || slot >= MAX_SAVE_SLOTS)
            return false;
        
        try
        {
            SaveData saveData = new SaveData
            {
                permanentData = permanentData,
                dailyData = dailyData,
                saveTime = DateTime.Now.ToString(),
                version = "1.0.0"
            };
            
            string json = JsonUtility.ToJson(saveData, true);
            string path = GetSlotPath(slot);
            
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存存档失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 删除存档
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    /// <returns>是否删除成功</returns>
    public bool DeleteSave(int slot)
    {
        if (slot < 0 || slot >= MAX_SAVE_SLOTS)
            return false;
        
        string path = GetSlotPath(slot);
        
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 检查存档是否存在
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    /// <returns>是否存在</returns>
    public bool SaveExists(int slot)
    {
        return File.Exists(GetSlotPath(slot));
    }
    
    /// <summary>
    /// 获取存档信息
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    /// <returns>存档信息</returns>
    public SaveInfo GetSaveInfo(int slot)
    {
        string path = GetSlotPath(slot);
        
        if (!File.Exists(path))
            return null;
        
        try
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
            return new SaveInfo
            {
                slot = slot,
                dayCount = saveData.dailyData.dayCount,
                shopLevel = saveData.permanentData.shopLevel,
                maxClearedFloor = saveData.permanentData.maxClearedFloor,
                saveTime = saveData.saveTime
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"获取存档信息失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 获取存档槽位路径
    /// </summary>
    /// <param name="slot">存档槽位（0-2）</param>
    /// <returns>文件路径</returns>
    private string GetSlotPath(int slot)
    {
        return Path.Combine(savePath, $"save_{slot}.json");
    }
    
    /// <summary>
    /// 生成当日随机事件
    /// </summary>
    private DailyEventData GenerateDailyEvent()
    {
        System.Random rand = new System.Random();
        int randNum = rand.Next(100);
        
        DailyEventData ev = new DailyEventData();
        
        if (randNum < 25)
        {
            ev.id = "material_double";
            ev.description = "今天采集效率翻倍！";
            ev.eventType = E_EventType.MaterialDouble;
            ev.multiplier = 2f;
        }
        else if (randNum < 45)
        {
            ev.id = "monster_surge";
            ev.description = "魔物潮来袭，小心应对！";
            ev.eventType = E_EventType.MonsterSurge;
            ev.multiplier = 1.5f;
        }
        else if (randNum < 60)
        {
            ev.id = "many_trapped";
            ev.description = "今天有更多村民被困在地牢中！";
            ev.eventType = E_EventType.ManyTrappedNPCs;
            ev.multiplier = 1.5f;
        }
        else
        {
            ev.id = "normal_day";
            ev.description = "普通的一天。";
            ev.eventType = E_EventType.NormalDay;
            ev.multiplier = 1f;
        }
        
        return ev;
    }
    
    /// <summary>
    /// 推进到下一时间段
    /// </summary>
    public void AdvanceTime()
    {
        switch (dailyData.currentTime)
        {
            case E_TimeOfDay.Dawn:
                dailyData.currentTime = E_TimeOfDay.Day;
                break;
            case E_TimeOfDay.Day:
                dailyData.currentTime = E_TimeOfDay.Night;
                break;
            case E_TimeOfDay.Night:
                dailyData.currentTime = E_TimeOfDay.LateNight;
                // 深夜结算时自动存档
                AutoSave();
                break;
            case E_TimeOfDay.LateNight:
                // 进入下一天
                NewDay();
                break;
        }
        
        // 广播时间切换事件
        EventCenter.Instance.EventTrigger(E_EventType.E_TimeOfDayChange, dailyData.currentTime);
    }
    
    /// <summary>
    /// 进入新的一天
    /// </summary>
    private void NewDay()
    {
        dailyData.dayCount++;
        dailyData.currentTime = E_TimeOfDay.Dawn;
        dailyData.currentEvent = GenerateDailyEvent();
        dailyData.cafeCustomersServed = 0;
        dailyData.tavernCustomersServed = 0;
        dailyData.goldEarnedToday = 0;
        dailyData.essenceEarnedToday = 0;
        dailyData.completedQuests.Clear();
        dailyData.hasClearedDungeonToday = false;
        
        // 重置清醒值
        dailyData.maxAwareness = 100 + permanentData.talents.coffeeAwarenessMax * 10;
        dailyData.awareness = dailyData.maxAwareness;
        
        // 处理种植和酿造
        ProcessFieldGrowth();
        ProcessBrewing();
        
        // 广播新的一天事件
        EventCenter.Instance.EventTrigger(E_EventType.E_NewDay, dailyData.dayCount);
    }
    
    /// <summary>
    /// 处理种植生长
    /// </summary>
    private void ProcessFieldGrowth()
    {
        foreach (var slot in dailyData.field.slots)
        {
            if (!string.IsNullOrEmpty(slot.seedId) && !slot.readyToHarvest)
            {
                slot.growthDays++;
                if (slot.growthDays >= slot.totalGrowthDays)
                {
                    slot.readyToHarvest = true;
                }
            }
        }
    }
    
    /// <summary>
    /// 处理酿造进度
    /// </summary>
    private void ProcessBrewing()
    {
        foreach (var slot in dailyData.barrel.slots)
        {
            if (!string.IsNullOrEmpty(slot.recipeId) && slot.daysRemaining > 0)
            {
                slot.daysRemaining--;
            }
        }
    }
    
    /// <summary>
    /// 增加永久精华
    /// </summary>
    /// <param name="amount">增加数量</param>
    public void AddEssence(int amount)
    {
        permanentData.dawnEssence += amount;
        dailyData.essenceEarnedToday += amount;
        EventCenter.Instance.EventTrigger(E_EventType.E_EssenceChanged, permanentData.dawnEssence);
    }
    
    /// <summary>
    /// 消耗永久精华
    /// </summary>
    /// <param name="amount">消耗数量</param>
    /// <returns>是否消耗成功</returns>
    public bool SpendEssence(int amount)
    {
        if (permanentData.dawnEssence >= amount)
        {
            permanentData.dawnEssence -= amount;
            EventCenter.Instance.EventTrigger(E_EventType.E_EssenceChanged, permanentData.dawnEssence);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 救援NPC
    /// </summary>
    /// <param name="npcId">NPC ID</param>
    public void RescueNPC(string npcId)
    {
        if (!permanentData.rescuedNPCs.Contains(npcId))
        {
            permanentData.rescuedNPCs.Add(npcId);
            EventCenter.Instance.EventTrigger(E_EventType.E_NPCRescued, npcId);
        }
    }
    
    /// <summary>
    /// 收容史莱姆
    /// </summary>
    public void CaptureSlime()
    {
        permanentData.hasSlime = true;
        dailyData.warehouse.autoSortEnabled = true;
        EventCenter.Instance.EventTrigger(E_EventType.E_SlimeCaptured);
    }
}
