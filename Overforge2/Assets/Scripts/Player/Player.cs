using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform materialPivot;
    [SerializeField] private DroppedItemInteractable droppedItemPrefab;
    [SerializeField] private float dropDistance = 1.2f;

    private bool isMoving;
    private bool isHolding;
    private Vector3 lastInteractDir;
    private Interactable selectedInteractable;
    private GameObject materialHeld;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += DoInteraction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsMoving() => isMoving;
    public bool IsHolding() => isHolding;
    public GameObject MaterialBeingHold() => materialHeld;

    public ItemSO GetHeldItemSO()
    {
        if (materialHeld == null) return null;
        return materialHeld.GetComponent<Item>().GetItemSO();
    }

    public void GetMaterial(ItemSO so)
    {
        if (materialHeld != null) Destroy(materialHeld);
        materialHeld = Instantiate(so.prefab.gameObject, materialPivot);
        materialHeld.GetComponent<Item>().SetItemSO(so);
        isHolding = true;
    }

    public void ClearMaterial()
    {
        if (materialHeld != null) Destroy(materialHeld);
        materialHeld = null;
        isHolding = false;
    }

    private void DoInteraction(object sender, System.EventArgs e)
    {
        if (selectedInteractable != null)
        {
            selectedInteractable.InitInteraction();
        }
        else if (materialHeld != null)
        {
            Vector3 dropPos = transform.position + lastInteractDir * dropDistance;
            DropItem(GetHeldItemSO(), dropPos);
            ClearMaterial();
        }
    }

    public void DropItem(ItemSO itemSO, Vector3 position)
    {
        DroppedItemInteractable dropped = Instantiate(droppedItemPrefab, position, Quaternion.identity);
        dropped.Setup(itemSO);
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero) lastInteractDir = moveDir;

        float interactDistance = 1.6f;

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit hit, interactDistance, countersLayerMask))
        {
            if (hit.transform.TryGetComponent(out Interactable interactable))
            {
                ItemSO heldItem = GetHeldItemSO();

                if (interactable.CanInteract(heldItem))
                {
                    interactable.SetHighlight(true);
                    selectedInteractable = interactable;
                }
                else
                {
                    selectedInteractable = null;
                }
            }
            else
            {
                selectedInteractable = null;
            }
        }
        else
        {
            selectedInteractable = null;
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.8f, moveDir, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.8f, moveDirX, moveDistance);

            if (canMove) moveDir = moveDirX;
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.8f, moveDirZ, moveDistance);
                if (canMove) moveDir = moveDirZ;
            }
        }

        if (canMove) transform.position += moveDir * moveDistance;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 15f);
        isMoving = moveDir != Vector3.zero;
    }
}