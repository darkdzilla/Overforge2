using UnityEngine;

public class MaterialPileInteractable : Interactable
{
    [SerializeField] private ItemSO materialObject;
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
        //base.InitInteraction();
        Player.Instance.GetMaterial(materialObject);
    }
}
