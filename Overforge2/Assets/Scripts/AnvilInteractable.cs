using UnityEngine;

public class AnvilInteractable : Interactable
{
    [SerializeField] private ItemSO resultItem;
    public ItemTypeEnum otherRequirement;
    [SerializeField] ItemSO otherResult;
    protected override void Update()
    {
        base.Update();
    }

    //public override void HighlightInteractable()
    //{
    //    base.HighlightInteractable();
    //}

    public override void InitInteraction()
    {
        base.InitInteraction();
        Player.Instance.ClearMaterial();
        //if (otherRequirement != 0)
        //{
        //    if (Player.Instance.MaterialBeingHold().GetComponent<Item>().GetItemSO().itemType == otherRequirement)
        //    {
        //        Player.Instance.GetMaterial(otherResult);
        //        return;
        //    }
        //}
        if (resultItem != null) Player.Instance.GetMaterial(resultItem);
    }


}
