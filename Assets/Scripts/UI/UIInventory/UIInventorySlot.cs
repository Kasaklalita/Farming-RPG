using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GridCursor gridCursor;
    private Cursor cursor;
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    //public TextMeshProUGUI textMeshProUGUI;
    public Text text;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
    }

    private void ClearCursors()
    {
        //Disable cursor
        gridCursor.DisableCursor();
        cursor.DisableCursor();

        //Set item type to none
        gridCursor.SelectedItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }

    /// <summary>
    /// Sets this inventory slot item to be selected
    /// </summary>
    private void SetSelectedItem()
    {
        //Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        //Highlight item on inventory bar
        isSelected = true;

        //Set highlighted inventory slots
        inventoryBar.SetHighlightedInventorySlots();

        //Set use radius for cursors
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        //If the item required a gridCursor then enable cursor
        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        if (itemDetails.itemUseRadius > 0)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        //Set item type
        gridCursor.SelectedItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType;


        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if (itemDetails.canBeCarried == true)
        {
            Player.Instance.ShowCarriedItrem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }

    private void ClearSelectedItem()
    {
        ClearCursors();

        //Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        //Set no item selected in inventory
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        Player.Instance.ClearCarriedItem();
    }

    /// <summary>
    /// Drops the item (if selected) at the current mouse position. Called by the DropItem event
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            // Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
            ////If can drop item here
            //Vector3Int gridPosition = GridPropertiesManager.Instance.grid.WorldToCell(worldPosition);
            //GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

            //if (gridPropertyDetails != null && gridPropertyDetails.canDropItem)
            ////if (gridCursor.CursorPositionIsValid)
            //{

            //    //Create item from prefab at mouse position
            //    GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize / 2f, worldPosition.z), Quaternion.identity, parentItem);
            //    Item item = itemGameObject.GetComponent<Item>();
            //    item.ItemCode = itemDetails.itemCode;

            //    //Remove item from player`s inventory
            //    InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

            //    //If no more of item then clear selected
            //    if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
            //    {
            //        ClearSelectedItem();
            //    }
            //}
            // If  a valid cursor position
            if (gridCursor.CursorPositionIsValid)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
                // Create item from prefab at mouse position
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize / 2f, worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                // Remove item from players inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                // If no more of item then clear selected
                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }
    }

    private void RemoveSelectedItemFromInventory()
    {
        if (itemDetails != null && isSelected)
        {
            int itemCode = itemDetails.itemCode;

            //Remove item from players inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            //If no more of item then clear selected
            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("[eq");
            }
            //Disable keyboard input
            Player.Instance.DisablePlayerInputAndResetMovement();

            //Instantiate gameObject as dragged item
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            //Get image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Move game object as dragged item
        if (draggedItem != null)
        {
            //draggedItem.transform.position = Input.mousePosition;
            draggedItem.transform.position = new Vector3(Input.mousePosition.x - 40f, Input.mousePosition.y - 40f, Input.mousePosition.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Destroy game object as dragged item
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            //If drag ends overinventory bar, get item drag is over and swap them
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                //Get the slot number where the drag ended
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                //Swap inventory items in inventory list
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                //Destroy inventory text box
                DestroyInventoryTextBox();

                //Clear selected item
                ClearSelectedItem();
            }
            //Else attempt to drom the item if it can be dropped
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }
            Player.Instance.EnablePlayerInput();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //If inventory slot currently selected, then deselect
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Populate text box with item details
        if (itemQuantity != 0)
        {
            //Instantiate inventory text box
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            //Set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            //Populate text box
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            //Set text box position according to inventory bar position
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }

    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}
