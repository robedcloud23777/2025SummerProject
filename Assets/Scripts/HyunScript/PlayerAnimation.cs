using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    public void SetMove(int horizontal, bool isGround, int verticalVelocity)
    {
        animator.SetInteger("MoveX", horizontal);
        animator.SetBool("IsGround", isGround);
        animator.SetInteger("VerticalVelocity", verticalVelocity);
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