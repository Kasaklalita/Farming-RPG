using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    //public TextMeshProUGUI textMeshProUGUI;
    public Text text;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
}
