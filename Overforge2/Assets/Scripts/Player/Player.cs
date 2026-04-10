using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform materialpivot;

    private bool isMoving;
    private bool isHolding;
    private Vector3 lastInteractDir;
    private Interactable selectedInteractable;
    private GameObject materialHolded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += DoInteraction;
    }

    void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsHolding()
    {
        return isHolding;
    }

    private void DoInteraction (object sender, System.EventArgs e)
    {
        if (selectedInteractable != null) selectedInteractable.InitInteraction();
    }

    private void HandleInteractions ()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero) lastInteractDir = moveDir;

        float interactDistance = 1.6f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out Interactable interactable)) 
            {
                if (materialHolded && interactable.typeRequirement != 0)
                {
                    if (interactable.typeRequirement == materialHolded.GetComponent<Item>().GetItemSO().itemType)
                    {
                        interactable.HighlightInteractable();
                        selectedInteractable = interactable;
                    }
                    else
                    {
                        AnvilInteractable ai = (AnvilInteractable)interactable;
                        if (ai != null && ai.otherRequirement == materialHolded.GetComponent<Item>().GetItemSO().itemType)
                        {
                            interactable.HighlightInteractable();
                            selectedInteractable = interactable;
                        }
                    }
                }
                else if (interactable.typeRequirement == 0)
                {
                    interactable.HighlightInteractable();
                    selectedInteractable = interactable;
                }
            }
        }
        else
        {
            selectedInteractable = null;
        }
    }

    private void HandleMovement ()
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

    public void GetMaterial(ItemSO so)
    {
        if (materialHolded != null) Destroy(materialHolded);
        materialHolded = Instantiate(so.prefab.gameObject, materialpivot);
        materialHolded.GetComponent<Item>().SetItemSO(so);
        isHolding = true;
    }

    public void ClearMaterial()
    {
        if (materialHolded != null) Destroy(materialHolded);
        materialHolded = null;
        isHolding = false;
    }

    public GameObject MaterialBeingHold()
    {
        return materialHolded;
    }
}
