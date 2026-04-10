using UnityEngine;

public class WorkbenchInteractable : Interactable
{
    [SerializeField] private ItemSO resultItem;

    [SerializeField] private ItemTypeEnum item1;
    [SerializeField] private ItemTypeEnum item2;

    [SerializeField] Transform itemPivot;

    private Item itemInBench;

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

        if (itemInBench == null)
        {
            GameObject i = Instantiate(Player.Instance.MaterialBeingHold(), itemPivot);
            itemInBench = i.GetComponent<Item>();
            itemInBench.SetItemSO(Player.Instance.MaterialBeingHold().GetComponent<Item>().GetItemSO());
            Player.Instance.ClearMaterial();
        }
        else
        {
            Debug.LogError("ya hay un objeto");
            if (itemInBench.GetItemSO().itemType == item1 && Player.Instance.MaterialBeingHold().GetComponent<Item>().GetItemSO().itemType == item2)
            {
                Debug.LogError("dos objetos correctos");
                Player.Instance.ClearMaterial();
                if (resultItem != null)
                {
                    Player.Instance.GetMaterial(resultItem);
                }
                Destroy(itemInBench.gameObject);
                itemInBench = null;
            }
            else if (itemInBench.GetItemSO().itemType == item2 && Player.Instance.MaterialBeingHold().GetComponent<Item>().GetItemSO().itemType == item1)
            {
                Debug.LogError("dos objetos correctos");
                Player.Instance.ClearMaterial();
                if (resultItem != null)
                {
                    Player.Instance.GetMaterial(resultItem);
                }
                Destroy(itemInBench.gameObject);
                itemInBench = null;
            }
        }
    }
}
