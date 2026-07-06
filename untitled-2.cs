// 新建文件：F:/UnityTest/MyGameFrame/MyGameFrame/Assets/Scripts/Framework/Inventory/InventoryMgr.cs
using System.Collections.Generic;

public class InventoryMgr : BaseManager<InventoryMgr>
{
    private InventoryMgr() { }
    
    public bool AddToInventory(string itemId, int quantity = 1)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        InventoryData inventory = dailyData.inventory;
        
        int currentTotal = GetTotalCount(inventory.items);
        if (currentTotal + quantity > inventory.capacity)
            return false;
        
        ItemStack existingStack = inventory.items.Find(s => s.itemId == itemId);
        if (existingStack != null)
        {
            existingStack.quantity += quantity;
        }
        else
        {
            inventory.items.Add(new ItemStack { itemId = itemId, quantity = quantity });
        }
        
        if (dailyData.warehouse.autoSortEnabled)
        {
            AutoSortToWarehouse();
        }
        
        EventCenter.Instance.EventTrigger(E_EventType.E_InventoryChanged);
        return true;
    }
    
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
        
        EventCenter.Instance.EventTrigger(E_EventType.E_InventoryChanged);
        return true;
    }
    
    public int GetInventoryCount(string itemId)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        ItemStack stack = dailyData.inventory.items.Find(s => s.itemId == itemId);
        return stack?.quantity ?? 0;
    }
    
    public bool AddToWarehouse(string itemId, int quantity = 1)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        WarehouseData warehouse = dailyData.warehouse;
        
        int currentTotal = GetTotalCount(warehouse.items);
        if (currentTotal + quantity > warehouse.capacity)
            return false;
        
        ItemStack existingStack = warehouse.items.Find(s => s.itemId == itemId);
        if (existingStack != null)
        {
            existingStack.quantity += quantity;
        }
        else
        {
            warehouse.items.Add(new ItemStack { itemId = itemId, quantity = quantity });
        }
        
        EventCenter.Instance.EventTrigger(E_EventType.E_WarehouseChanged);
        return true;
    }
    
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
        
        EventCenter.Instance.EventTrigger(E_EventType.E_WarehouseChanged);
        return true;
    }
    
    public int GetWarehouseCount(string itemId)
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        ItemStack stack = dailyData.warehouse.items.Find(s => s.itemId == itemId);
        return stack?.quantity ?? 0;
    }
    
    public bool TransferToInventory(string itemId, int quantity = 1)
    {
        if (!RemoveFromWarehouse(itemId, quantity))
            return false;
        
        return AddToInventory(itemId, quantity);
    }
    
    public bool TransferToWarehouse(string itemId, int quantity = 1)
    {
        if (!RemoveFromInventory(itemId, quantity))
            return false;
        
        return AddToWarehouse(itemId, quantity);
    }
    
    private void AutoSortToWarehouse()
    {
        DailyData dailyData = SaveMgr.Instance.GetDailyData();
        InventoryData inventory = dailyData.inventory;
        
        int currentTotal = GetTotalCount(inventory.items);
        if (currentTotal <= inventory.capacity)
            return;
        
        int overflow = currentTotal - inventory.capacity;
        
        for (int i = inventory.items.Count - 1; i >= 0 && overflow > 0; i--)
        {
            ItemStack stack = inventory.items[i];
            int transferAmount = Mathf.Min(stack.quantity, overflow);
            
            AddToWarehouse(stack.itemId, transferAmount);
            
            stack.quantity -= transferAmount;
            if (stack.quantity <= 0)
            {
                inventory.items.RemoveAt(i);
            }
            
            overflow -= transferAmount;
        }
    }
    
    private int GetTotalCount(List<ItemStack> items)
    {
        int total = 0;
        foreach (var stack in items)
        {
            total += stack.quantity;
        }
        return total;
    }
    
    public bool HasEnoughMaterials(string recipeId)
    {
        GameConfig config = SaveMgr.Instance.GetConfig();
        DrinkData recipe = FindRecipe(config, recipeId);
        
        if (recipe == null)
            return false;
        
        foreach (string matId in recipe.requiredMaterials)
        {
            if (GetWarehouseCount(matId) < 1)
                return false;
        }
        
        return true;
    }
    
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
    
    private DrinkData FindRecipe(GameConfig config, string recipeId)
    {
        foreach (var recipe in config.drinkConfig.coffeeRecipes)
        {
            if (recipe.id == recipeId)
                return recipe;
        }
        
        foreach (var recipe in config.drinkConfig.wineRecipes)
        {
            if (recipe.id == recipeId)
                return recipe;
        }
        
        return null;
    }
}
