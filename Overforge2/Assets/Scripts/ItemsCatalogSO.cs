using UnityEngine;

[CreateAssetMenu()]
public class ItemsCatalogSO : ScriptableObject
{
    public ItemSO[] finalProductCatalog;

    public ItemSO[] subproductCatalog;

    public ItemSO[] rawMaterialCatalog;

}
