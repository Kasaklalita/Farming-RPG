using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDectionary;

    private int[] selectedInventoryItem; //The index of the array is the inventory list, and the value is the item code;

    public List<InventoryItem>[] inventoryLists;

    //The index of the array is the inventory list (from the EnventoryLocation enum), and the value is the capacity of that inventory list
    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();

        CreateItemDetailsDictionary();

        //Initialise selected inventory item array
        selectedInventoryItem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        //Initialise inventory list capacity array
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        //Initialise player inventory list capacity
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    /// <summary>
    /// Populates the itemDetailsDictionary from the scriptable object items list
    /// </summary>
    private void CreateItemDetailsDectionary()
    {
        itemDetailsDectionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDectionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    /// <summary>
    /// Add an item to the inventory list for the inventoryLocation and then destroy the gameObjectToDelete
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    /// <param name="jameObjecttoDelete"></param>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);
        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// Add an item to the inventory list for the inventoryLocation
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        //Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //Check if invenntory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        //Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// Add item to the end of the inventory
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add item to position in the inventory
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    /// <param name="position"></param>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;

        Debug.ClearDeveloperConsole();
        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Swap item at fromItem index with item at toItem index in inventoryLocation inventory list
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="fromItem"></param>
    /// <param name="toItem"></param>
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        //If fromItem index and toItem index are within the bounds of the list, not the same, and greater than or equal to zero
        if (fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            //Send event that inventory has been updated
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }

    /// <summary>
    /// Clear the selected inventory item for inventoryLocation
    /// </summary>
    /// <param name="inventoryLocation"></param>
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    /// <summary>
    /// Find if an itemCode is already in the inventory. Returns the item position in the inventory list,
    /// or -1 if the item is not in the inventory
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Populates the itemDetailsDictionary from the scriptable object items list
    /// </summary>

    private void CreateItemDetailsDictionary()
    {
        itemDetailsDectionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDectionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    /// <summary>
    /// Returns the itemDetails (from the SO_itemList) for the itemcode, or null is the item code doesnt`t exist
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDectionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the itemDetails (from the SO_ItemList) for the currently selected item in the inventoryLocation, or null if an item isn`t selected
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <returns></returns>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    /// <summary>
    /// Get the selected item for inventoryLocation - returns itemCode or -1 if nothing is selected
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <returns></returns>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    /// <summary>
    /// Get the item type description for an item type - returns the item type description as a string for a given ItemType
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    /// <summary>
    /// Remove an item from the inventory, and create a game object at the position it was dropped
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        //Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    /// <summary>
    /// Set the selected inventory item for inventoryLocation to itemCode
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem inventoryItem in inventoryList)
    //    {
    //        Debug.Log("Item Description: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "\tItem Quantity: " + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("********************");
    //}
}
