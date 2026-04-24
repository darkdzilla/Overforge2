using UnityEngine;

public class DroppedItemInteractable : Interactable
{
    private ItemSO storedItemSO;
    [SerializeField] private Transform itemPivot;

    public void Setup(ItemSO itemSO)
    {
        storedItemSO = itemSO;
        GameObject visual = Instantiate(itemSO.prefab.gameObject, itemPivot);
        visual.transform.localPosition = Vector3.zero;
    }

    public override bool CanInteract(ItemSO heldItem)
    {
        // Player can always pick up or swap
        return true;
    }

    public override void InitInteraction()
    {
        ItemSO heldItem = Player.Instance.GetHeldItemSO();

        if (heldItem != null)
        {
            // Swap: drop current item here, pick up this one
            ItemSO previousHeld = heldItem;
            Player.Instance.ClearMaterial();
            Player.Instance.GetMaterial(storedItemSO);
            Player.Instance.DropItem(previousHeld, transform.position);
        }
        else
        {
            // Just pick up
            Player.Instance.GetMaterial(storedItemSO);
        }

        Destroy(gameObject);
    }
}
