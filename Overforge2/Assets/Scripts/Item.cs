using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemSO itemSO;

    public void SetItemSO (ItemSO so)
    {
        itemSO = so;
    }

    public ItemSO GetItemSO ()
    {
        return itemSO;
    }
}
