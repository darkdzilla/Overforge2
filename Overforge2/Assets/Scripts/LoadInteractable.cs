using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadInteractable : Interactable
{
    [Header("Order Settings")]
    [SerializeField] private ItemSO requiredProduct;
    [SerializeField] private int requiredQuantity = 3;
    [SerializeField] private float timeLimit = 60f;

    [Header("World UI")]
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image productIcon;
    [Tooltip("Image with Image Type set to Filled")]
    [SerializeField] private Image timerFillImage;

    private int remainingQuantity;
    private float timeRemaining;
    private bool isActive;
    private bool hasBeenSetup;

    private Action onCompleted;
    private Action onExpired;

    private void Start()
    {
        // Standalone mode: if no LevelManager called Setup() before Start,
        // fall back to the serialized defaults so the prefab still works
        // when dropped directly in a scene for testing.
        if (!hasBeenSetup)
            Setup(requiredProduct, requiredQuantity, timeLimit);
    }

    /// <summary>
    /// Called by LevelManager (or directly for testing) to initialize the order.
    /// </summary>
    public void Setup(ItemSO product, int quantity, float duration,
                      Action onCompleted = null, Action onExpired = null)
    {
        requiredProduct = product;
        remainingQuantity = quantity;
        timeLimit = duration;
        timeRemaining = duration;
        isActive = true;
        hasBeenSetup = true;

        this.onCompleted = onCompleted;
        this.onExpired = onExpired;

        RefreshUI();
    }

    public override bool CanInteract(ItemSO heldItem)
    {
        if (!isActive || heldItem == null || requiredProduct == null) return false;
        return heldItem.itemType == requiredProduct.itemType;
    }

    public override void InitInteraction()
    {
        // Deliberately skip base.InitInteraction() — we handle our own UI.
        if (!isActive) return;

        remainingQuantity--;
        Player.Instance.ClearMaterial();
        RefreshUI();

        if (remainingQuantity <= 0)
            CompleteOrder();
    }

    protected override void Update()
    {
        base.Update(); // keeps highlight logic running

        if (!isActive) return;

        timeRemaining -= Time.deltaTime;
        if (timerFillImage != null)
            timerFillImage.fillAmount = Mathf.Clamp01(timeRemaining / timeLimit);

        if (timeRemaining <= 0f)
            ExpireOrder();
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private void RefreshUI()
    {
        if (quantityText != null)
            quantityText.text = remainingQuantity.ToString();

        if (productIcon != null && requiredProduct != null)
        {
            productIcon.sprite = requiredProduct.sprite;
            productIcon.enabled = requiredProduct.sprite != null;
        }

        if (timerFillImage != null)
            timerFillImage.fillAmount = Mathf.Clamp01(timeRemaining / timeLimit);
    }

    private void CompleteOrder()
    {
        isActive = false;
        onCompleted?.Invoke();
        Destroy(gameObject);
    }

    private void ExpireOrder()
    {
        isActive = false;
        onExpired?.Invoke();
        Destroy(gameObject);
    }
}