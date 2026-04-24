using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe Step")]
public class RecipeStepSO : ScriptableObject
{
    public StationTypeEnum station;
    public ItemSO input1;
    [Tooltip("Leave empty for single-input steps")]
    public ItemSO input2;
    public ItemSO output;
}
