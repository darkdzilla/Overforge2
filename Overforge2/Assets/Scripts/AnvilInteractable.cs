using UnityEngine;

public class AnvilInteractable : Interactable
{
    [SerializeField] private ItemSO resultItem;
    protected override void Update()
    {
        base.Update();
    }

    public override void HighlightInteractable()
    {
        base.HighlightInteractable();
    }

    public override void InitInteraction()
    {
        base.InitInteraction();
        Player.Instance.ClearMaterial();
        if (resultItem != null) Player.Instance.GetMaterial(resultItem);
    }


}
