using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Renderer[] highlightRenderers;
    [SerializeField] private GameObject screen;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private string testMessage;

    private static readonly int VisualEffect = Shader.PropertyToID("_Visual_effect");

    private bool selectedForInteraction;

    /// <summary>
    /// Determines if the player can interact with this interactable given the item they hold (can be null).
    /// Each subclass implements its own rules.
    /// </summary>
    public virtual bool CanInteract(ItemSO heldItem)
    {
        return false;
    }

    public virtual void InitInteraction()
    {
        if (message != null) message.text = testMessage;
        if (screen != null) screen.SetActive(true);
        Invoke(nameof(DeactivateMessage), 3f);
    }

    public void SetHighlight(bool active)
    {
        selectedForInteraction = active;
    }

    protected virtual void DeactivateMessage()
    {
        if (screen != null) screen.SetActive(false);
    }

    protected virtual void Update()
    {
        float value = selectedForInteraction ? 1f : 0f;

        foreach (Renderer r in highlightRenderers)
        {
            if (r == null) continue;
            foreach (Material mat in r.materials)
                mat.SetFloat(VisualEffect, value);
        }

        selectedForInteraction = false;
    }
}