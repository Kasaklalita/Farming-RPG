using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDectionary;

    public List<InventoryItem>[] inventoryLists;

    //The index of the array is the inventory list (from the EnventoryLocation enum), and the value is the capacity of that inventory list
    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();

        CreateItemDetailsDictionary();
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

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem inventoryItem in inventoryList)
    //    {
    //        Debug.Log("Item Description: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "\tItem Quantity: " + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("********************");
    //}
}
