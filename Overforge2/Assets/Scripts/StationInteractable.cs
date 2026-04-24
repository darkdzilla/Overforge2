using UnityEngine;

public class StationInteractable : Interactable
{
    [SerializeField] private StationTypeEnum stationType;
    [SerializeField] private RecipeDatabaseSO recipeDatabase;

    public override bool CanInteract(ItemSO heldItem)
    {
        if (heldItem == null) return false;
        return recipeDatabase.CanStationUseItem(stationType, heldItem);
    }

    public override void InitInteraction()
    {
        base.InitInteraction();

        ItemSO heldItem = Player.Instance.GetHeldItemSO();
        if (heldItem == null) return;

        ItemSO result = recipeDatabase.TryGetResult(stationType, heldItem);
        if (result == null) return;

        Player.Instance.ClearMaterial();
        Player.Instance.GetMaterial(result);
    }
}
