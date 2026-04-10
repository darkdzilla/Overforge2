using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interactable : MonoBehaviour
{
    [SerializeField] GameObject highlight;
    [SerializeField] private GameObject screen;
    //public string requirement;
    public ItemTypeEnum typeRequirement;

    private bool selectedForInteraction;

    public string testMessage;
    public TextMeshProUGUI message;
    
    public virtual void HighlightInteractable ()
    {
        selectedForInteraction = true;
    }

    public virtual void InitInteraction()
    {
        if (message != null) message.text = testMessage;
        if (screen != null) screen.SetActive(true);
        Invoke("DeactivateMessage", 3f);
    }

    protected virtual void DeactivateMessage ()
    {
        if (screen != null) screen.SetActive(false);
    }

    protected virtual void Update()
    {
        highlight.SetActive(selectedForInteraction);
        selectedForInteraction = false;
    }
}
