using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe Database")]
public class RecipeDatabaseSO : ScriptableObject
{
    public RecipeSO[] recipes;

    /// <summary>
    /// Tries to find the output for a given station and input(s).
    /// For single-input stations pass input2 as null.
    /// Input order does not matter for two-input steps.
    /// </summary>
    public ItemSO TryGetResult(StationTypeEnum station, ItemSO input1, ItemSO input2 = null)
    {
        foreach (var recipe in recipes)
        {
            foreach (var step in recipe.steps)
            {
                if (step.station != station) continue;

                if (MatchesInputs(step, input1, input2))
                    return step.output;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns true if the given item can be used at the given station
    /// in any recipe step (as input1 or input2).
    /// </summary>
    public bool CanStationUseItem(StationTypeEnum station, ItemSO item)
    {
        foreach (var recipe in recipes)
        {
            foreach (var step in recipe.steps)
            {
                if (step.station != station) continue;

                if (step.input1 != null && step.input1.itemType == item.itemType)
                    return true;
                if (step.input2 != null && step.input2.itemType == item.itemType)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if the two items can be combined at the given station.
    /// Used by Workbench to validate the second item when one is already placed.
    /// </summary>
    public bool CanCombine(StationTypeEnum station, ItemSO itemA, ItemSO itemB)
    {
        return TryGetResult(station, itemA, itemB) != null;
    }

    private bool MatchesInputs(RecipeStepSO step, ItemSO input1, ItemSO input2)
    {
        // Single-input step
        if (step.input2 == null && input2 == null)
            return step.input1.itemType == input1.itemType;

        // Two-input step: accept either order
        if (step.input2 != null && input1 != null && input2 != null)
        {
            bool directMatch = step.input1.itemType == input1.itemType
                            && step.input2.itemType == input2.itemType;
            bool swappedMatch = step.input1.itemType == input2.itemType
                             && step.input2.itemType == input1.itemType;
            return directMatch || swappedMatch;
        }

        return false;
    }
}
