using UnityEngine;
using UnityEngine.UI;

public class WorkbenchInteractable : Interactable
{
    [SerializeField] private StationTypeEnum stationType = StationTypeEnum.Workbench;
    [SerializeField] private RecipeDatabaseSO recipeDatabase;
    [SerializeField] private Transform itemPivot;
    [SerializeField] private Image itemIcon;

    private Item itemInBench;

    public override bool CanInteract(ItemSO heldItem)
    {
        // Bench empty + player holds something → can deposit
        if (itemInBench == null && heldItem != null)
            return true;

        // Bench has item + player empty → can retrieve
        if (itemInBench != null && heldItem == null)
            return true;

        // Bench has item + player holds something → only if valid combo
        if (itemInBench != null && heldItem != null)
            return recipeDatabase.CanCombine(stationType, itemInBench.GetItemSO(), heldItem);

        return false;
    }

    public override void InitInteraction()
    {
        base.InitInteraction();

        ItemSO heldItem = Player.Instance.GetHeldItemSO();

        // Bench empty + player holds item → deposit
        if (itemInBench == null && heldItem != null)
        {
            DepositItem();
            return;
        }

        // Bench has item + player empty → retrieve
        if (itemInBench != null && heldItem == null)
        {
            RetrieveItem();
            return;
        }

        // Bench has item + player holds item → try combine
        if (itemInBench != null && heldItem != null)
        {
            TryCombine(heldItem);
        }
    }

    private void DepositItem()
    {
        GameObject heldObj = Player.Instance.MaterialBeingHold();
        GameObject deposited = Instantiate(heldObj, itemPivot);
        itemInBench = deposited.GetComponent<Item>();
        itemInBench.SetItemSO(heldObj.GetComponent<Item>().GetItemSO());
        Player.Instance.ClearMaterial();
        UpdateIcon(itemInBench.GetItemSO().sprite);
    }

    private void RetrieveItem()
    {
        Player.Instance.GetMaterial(itemInBench.GetItemSO());
        Destroy(itemInBench.gameObject);
        itemInBench = null;
        UpdateIcon(null);
    }

    private void TryCombine(ItemSO heldItem)
    {
        ItemSO result = recipeDatabase.TryGetResult(stationType, itemInBench.GetItemSO(), heldItem);
        if (result == null) return;

        Player.Instance.ClearMaterial();
        Player.Instance.GetMaterial(result);
        Destroy(itemInBench.gameObject);
        itemInBench = null;
        UpdateIcon(null);
    }

    private void UpdateIcon(Sprite sprite)
    {
        if (itemIcon == null) return;
        itemIcon.sprite = sprite;
        itemIcon.enabled = sprite != null;
    }
}