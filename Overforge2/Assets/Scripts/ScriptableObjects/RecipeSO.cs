using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class RecipeSO : ScriptableObject
{
    public string recipeName;
    public RecipeStepSO[] steps;
}
