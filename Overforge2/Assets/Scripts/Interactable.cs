using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] GameObject highlight;
    private bool selectedForInteraction;
    
    public void HighlightInteractable ()
    {
        selectedForInteraction = true;
    }

    private void Update()
    {
        highlight.SetActive(selectedForInteraction);
        selectedForInteraction = false;
    }
}
