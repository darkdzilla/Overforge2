using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Player player;
    private void Awake()
    {
        //animator = GetComponent<Animator>();       
    }

    private void Update()
    {
        animator.SetBool("Moving", player.IsMoving());
        animator.SetBool("HoldingThing", player.IsHolding());

        Debug.Log("moving " + player.IsMoving() + " - holding " + player.IsHolding());
    }
}
