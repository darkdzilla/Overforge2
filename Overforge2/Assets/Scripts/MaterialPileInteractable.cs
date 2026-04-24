using UnityEngine;

public class MaterialPileInteractable : Interactable
{
    [SerializeField] private ItemSO materialToGive;

    public override bool CanInteract(ItemSO heldItem)
    {
        return true;
    }

    public override void InitInteraction()
    {
        Player.Instance.ClearMaterial();
        Player.Instance.GetMaterial(materialToGive);
    }
}
