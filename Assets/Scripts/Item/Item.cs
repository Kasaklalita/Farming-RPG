using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private int _itemCode;
    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCodeParam)
    {

    }
}
