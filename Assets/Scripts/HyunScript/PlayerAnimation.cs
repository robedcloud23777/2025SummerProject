using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    public void SetMove(float horizontal, bool isGround)
    {
        animator.SetFloat("MoveX", horizontal);
        animator.SetBool("IsMoving", horizontal != 0);
        animator.SetBool("IsGround", isGround);
    }

    public void TriggerJump()
    {
        animator.SetTrigger("Jump");
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TriggerKick()
    {
        animator.SetTrigger("Kick");
    }
}