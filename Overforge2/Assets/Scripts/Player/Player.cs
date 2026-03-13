using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    private bool isMoving;
    private Vector3 lastInteractDir;

    void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    private void HandleInteractions ()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero) lastInteractDir = moveDir;

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out Interactable interactable)) interactable.HighlightInteractable();
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
}
