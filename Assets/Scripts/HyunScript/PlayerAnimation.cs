using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private int currentCombo = 0;
    private float lastAttackTime;
    [SerializeField] private float comboResetTime = 0.4f; // 콤보 유지 시간
    [SerializeField] private int maxCombo = 5;

    private void Update()
    {
        // 공격 후 콤보 시간 초과 시 리셋
        if (currentCombo > 0 && Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void SetMove(float horizontal, bool isGround, int verticalVelocity)
    {
        animator.SetFloat("MoveX", horizontal);
        animator.SetBool("IsGround", isGround);
        animator.SetInteger("VerticalVelocity", verticalVelocity);
    }

    public void TriggerJump()
    {
        if (!IsInState("Jump"))
            animator.SetTrigger("Jump");
    }

    public void TriggerAttack()
    {
        // 콤보 증가
        currentCombo++;
        if (currentCombo > maxCombo) currentCombo = 1;

        // 애니메이터 파라미터 설정
        animator.SetInteger("ComboIndex", currentCombo);
        animator.SetTrigger("Attack");

        // 마지막 공격 시간 갱신
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

    public void TriggerDie(bool isDead)
    {
        animator.SetBool("Die", isDead);
    }

    // 🔹 애니메이션 이벤트에서 호출할 수 있도록 public 처리
    public void ResetCombo()
    {
        currentCombo = 0;
        animator.SetInteger("ComboIndex", 0);
    }

    // 🔹 공격 애니메이션이 끝날 때 자동으로 Idle/Move로 돌아가도록 트리거
    //    → Attack 애니메이션 마지막 프레임에 Animation Event 걸어서 호출
    public void OnAttackAnimationEnd()
    {
        ResetCombo();
        animator.ResetTrigger("Attack");
        // 필요하다면 Idle로 강제 전환 트리거 추가 가능
        animator.SetTrigger("ToIdle");
    }
}
