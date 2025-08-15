using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private int currentCombo = 0;
    private float lastAttackTime;
    public float comboResetTime = 1f;
    private int maxCombo = 5;

    private void Update()
    {
        if (currentCombo > 0 && Time.time - lastAttackTime > comboResetTime)
        {
            currentCombo = 0;
            animator.SetInteger("ComboIndex", 0); // 애니메이터 값도 같이 초기화
        }
    }

    public void SetMove(int horizontal, bool isGround, float verticalVelocity)
    {
        animator.SetInteger("MoveX", horizontal);
        animator.SetBool("IsGround", isGround);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }

    public void TriggerJump()
    {
        if (!IsInState("Jump"))
            animator.SetTrigger("Jump");
    }

    public void TriggerAttack()
    {
        currentCombo++;
        if (currentCombo > maxCombo) currentCombo = 1;
        animator.SetInteger("ComboIndex", currentCombo);
        animator.SetTrigger("Attack");

        lastAttackTime = Time.time;
    }

    public void TriggerKick()
    {
        if (!IsInState("Kick"))
            animator.SetTrigger("Kick");
    }

    public bool IsInState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    // 애니메이션 이벤트에서 호출 가능
    public void ResetCombo()
    {
        currentCombo = 0;
        animator.SetInteger("ComboIndex", 0);
    }

    public void TriggerHit()
    {
        if (!IsInState("Hit"))
            animator.SetTrigger("Hit");
    }

    public void TriggerDodge()
    {
        if (!IsInState("Dodge"))
            animator.SetTrigger("Dodge");
    }

    public void TriggerDie()
    {
        if (!IsInState("Die"))
            animator.SetTrigger("Die");
    }
}
