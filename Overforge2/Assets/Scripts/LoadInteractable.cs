using UnityEngine;

public class LoadInteractable : Interactable
{
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
    }
}
