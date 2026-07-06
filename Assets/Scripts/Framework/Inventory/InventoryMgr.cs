// 新建文件：Assets/Scripts/Framework/Inventory/InventoryMgr.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品管理系统 - 使用现有BaseManager单例框架
/// </summary>
public class InventoryMgr : BaseManager<InventoryMgr>
{
    /// <summary>
    /// 私有构造函数（单例模式）
    /// </summary>
    private InventoryMgr() { }
    
    /// <summary>
    /// 添加物品到背包
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="quantity">数量</param>
    /// <returns>是否添加成功</returns>
    public bool AddToInventory(string itemId, int quantity = 1)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        InventoryData inventory = dailyData.inventory;
        
        // 检查容量
        int currentTotal = GetTotalCount(inventory.items);
        if (currentTotal + quantity > inventory.capacity)
            return false;
        
        // 查找现有堆叠
        ItemStack existingStack = inventory.items.Find(s => s.itemId == itemId);
        if (existingStack != null)
        {
            existingStack.quantity += quantity;
        }
        else
        {
            inventory.items.Add(new ItemStack { itemId = itemId, quantity = quantity });
        }
        
        // 如果开启自动分拣，同步到仓库
        if (dailyData.warehouse.autoSortEnabled)
        {
            AutoSortToWarehouse();
        }
        
        // 触发背包变化事件
        EventCenter.Instance.EventTrigger(E_EventType.E_InventoryChanged);
        return true;
    }
    
    /// <summary>
    /// 从背包移除物品
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="quantity">数量</param>
    /// <returns>是否移除成功</returns>
    public bool RemoveFromInventory(string itemId, int quantity = 1)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        InventoryData inventory = dailyData.inventory;
        
        ItemStack stack = inventory.items.Find(s => s.itemId == itemId);
        if (stack == null || stack.quantity < quantity)
            return false;
        
        stack.quantity -= quantity;
        if (stack.quantity <= 0)
            inventory.items.Remove(stack);
        
        // 触发背包变化事件
        EventCenter.Instance.EventTrigger(E_EventType.E_InventoryChanged);
        return true;
    }
    
    /// <summary>
    /// 检查背包中物品数量
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <returns>物品数量</returns>
    public int GetInventoryCount(string itemId)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        ItemStack stack = dailyData.inventory.items.Find(s => s.itemId == itemId);
        return stack?.quantity ?? 0;
    }
    
    /// <summary>
    /// 添加物品到仓库
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="quantity">数量</param>
    /// <returns>是否添加成功</returns>
    public bool AddToWarehouse(string itemId, int quantity = 1)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        WarehouseData warehouse = dailyData.warehouse;
        
        // 检查容量
        int currentTotal = GetTotalCount(warehouse.items);
        if (currentTotal + quantity > warehouse.capacity)
            return false;
        
        // 查找现有堆叠
        ItemStack existingStack = warehouse.items.Find(s => s.itemId == itemId);
        if (existingStack != null)
        {
            existingStack.quantity += quantity;
        }
        else
        {
            warehouse.items.Add(new ItemStack { itemId = itemId, quantity = quantity });
        }
        
        // 触发仓库变化事件
        EventCenter.Instance.EventTrigger(E_EventType.E_WarehouseChanged);
        return true;
    }
    
    /// <summary>
    /// 从仓库移除物品
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="quantity">数量</param>
    /// <returns>是否移除成功</returns>
    public bool RemoveFromWarehouse(string itemId, int quantity = 1)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        WarehouseData warehouse = dailyData.warehouse;
        
        ItemStack stack = warehouse.items.Find(s => s.itemId == itemId);
        if (stack == null || stack.quantity < quantity)
            return false;
        
        stack.quantity -= quantity;
        if (stack.quantity <= 0)
            warehouse.items.Remove(stack);
        
        // 触发仓库变化事件
        EventCenter.Instance.EventTrigger(E_EventType.E_WarehouseChanged);
        return true;
    }
    
    /// <summary>
    /// 检查仓库中物品数量
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <returns>物品数量</returns>
    public int GetWarehouseCount(string itemId)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        ItemStack stack = dailyData.warehouse.items.Find(s => s.itemId == itemId);
        return stack?.quantity ?? 0;
    }
    
    /// <summary>
    /// 从仓库转移到背包
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="quantity">数量</param>
    /// <returns>是否转移成功</returns>
    public bool TransferToInventory(string itemId, int quantity = 1)
    {
        if (!RemoveFromWarehouse(itemId, quantity))
            return false;
        
        return AddToInventory(itemId, quantity);
    }
    
    /// <summary>
    /// 从背包转移到仓库
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="quantity">数量</param>
    /// <returns>是否转移成功</returns>
    public bool TransferToWarehouse(string itemId, int quantity = 1)
    {
        if (!RemoveFromInventory(itemId, quantity))
            return false;
        
        return AddToWarehouse(itemId, quantity);
    }
    
    /// <summary>
    /// 自动分拣：背包满时自动转移到仓库
    /// </summary>
    private void AutoSortToWarehouse()
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        InventoryData inventory = dailyData.inventory;
        
        int currentTotal = GetTotalCount(inventory.items);
        if (currentTotal <= inventory.capacity)
            return;
        
        // 超出部分转移到仓库
        int overflow = currentTotal - inventory.capacity;
        
        // 从最后添加的物品开始转移（假设列表末尾是最新添加的）
        for (int i = inventory.items.Count - 1; i >= 0 && overflow > 0; i--)
        {
            ItemStack stack = inventory.items[i];
            int transferAmount = Mathf.Min(stack.quantity, overflow);
            
            // 添加到仓库
            AddToWarehouse(stack.itemId, transferAmount);
            
            // 从背包移除
            stack.quantity -= transferAmount;
            if (stack.quantity <= 0)
            {
                inventory.items.RemoveAt(i);
            }
            
            overflow -= transferAmount;
        }
    }
    
    /// <summary>
    /// 获取物品列表总数量
    /// </summary>
    /// <param name="items">物品列表</param>
    /// <returns>总数量</returns>
    private int GetTotalCount(List<ItemStack> items)
    {
        int total = 0;
        foreach (var stack in items)
        {
            total += stack.quantity;
        }
        return total;
    }
    
    /// <summary>
    /// 检查是否有足够的材料制作饮品
    /// </summary>
    /// <param name="recipeId">配方ID</param>
    /// <returns>是否有足够材料</returns>
    public bool HasEnoughMaterials(string recipeId)
    {
        GameConfig config = SaveMgr.Instance.GetConfig();
        DrinkData recipe = FindRecipe(config, recipeId);
        
        if (recipe == null)
            return false;
        
        // 检查仓库中是否有足够材料
        foreach (string matId in recipe.requiredMaterials)
        {
            if (GetWarehouseCount(matId) < 1)
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 消耗材料制作饮品
    /// </summary>
    /// <param name="recipeId">配方ID</param>
    /// <returns>是否消耗成功</returns>
    public bool ConsumeMaterials(string recipeId)
    {
        if (!HasEnoughMaterials(recipeId))
            return false;
        
        GameConfig config = SaveMgr.Instance.GetConfig();
        DrinkData recipe = FindRecipe(config, recipeId);
        
        foreach (string matId in recipe.requiredMaterials)
        {
            RemoveFromWarehouse(matId, 1);
        }
        
        return true;
    }
    
    /// <summary>
    /// 查找配方
    /// </summary>
    /// <param name="config">游戏配置</param>
    /// <param name="recipeId">配方ID</param>
    /// <returns>配方数据</returns>
    private DrinkData FindRecipe(GameConfig config, string recipeId)
    {
        // 先在咖啡配方中查找
        foreach (var recipe in config.drinkConfig.coffeeRecipes)
        {
            if (recipe.id == recipeId)
                return recipe;
        }
        
        // 再在酒水配方中查找
        foreach (var recipe in config.drinkConfig.wineRecipes)
        {
            if (recipe.id == recipeId)
                return recipe;
        }
        
        return null;
    }
}
