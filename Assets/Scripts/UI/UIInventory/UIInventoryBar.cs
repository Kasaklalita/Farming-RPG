using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlot = null;
    public GameObject inventoryBarDraggedItem;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;
    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    //private void OnDisable()
    //{
    //    EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    //}

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void ClearInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            //Loop through inventory slots and update with blank sprite
            for (int i = 0; i < inventorySlot.Length - 1; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                Debug.Log('1');
                //inventorySlot[i].textMeshProUGUI.text = "";
                inventorySlot[i].text.text = "";
                Debug.Log('2');
                inventorySlot[i].itemDetails = null;
                Debug.Log('3');
                inventorySlot[i].itemQuantity = 0;
                Debug.Log('4');
            }
        }
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            //transform.position = new Vector3(transform.position.x, 7.5f, 0f); //This was changed to control the rectTransform see below
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (inventorySlot.Length > 0 && inventoryList.Count > 0)
            {
                //Loop through inventory slots and update with corresponding inventory list item
                for (int i = 0; i < inventorySlot.Length - 1; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        //ItemDetails itemDetails = InventoryManager.Instance.itemList.itemDetails.Find(x => x.itemCode == itemCode);
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            //Add images and details to inventory item slot
                            inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            //inventorySlot[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlot[i].text.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlot[i].itemDetails = itemDetails;
                            inventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
