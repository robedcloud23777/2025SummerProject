using System;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks, IHitable
{
    public float health = 100f;
    public PlayerKickAttack kickAttack;
    public PlayerAttack punchAttack;
    private PlayerMove playerMove;
    private PlayerAnimation playerAnim;
    private Slider HpSlider;

    // 입력 쿨다운 설정 (원하면 값 조정)
    public float punchInputCooldown = 0.3f;
    public float kickInputCooldown = 0.5f;

    public bool canPunch = true;
    public bool canKick = true;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            // 체력 닳았을때 이펙트 추가.
        }
    }

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAnim = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (canPunch)
            {
                canPunch = false;
                punchAttack.Attack();
                playerAnim.TriggerAttack();
                StartCoroutine(PunchCooldownRoutine());
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (canKick)
            {
                canKick = false;
                kickAttack.Attack();
                playerAnim.TriggerKick();
                StartCoroutine(KickCooldownRoutine());
            }
        }

        if(health < 0f)
        {
            playerAnim.TriggerDie();
        }
    }

    private IEnumerator PunchCooldownRoutine()
    {
        yield return new WaitForSeconds(punchInputCooldown);
        canPunch = true;
    }

    private IEnumerator KickCooldownRoutine()
    {
        yield return new WaitForSeconds(kickInputCooldown);
        canKick = true;
    }

    public void Hit(float damage)
    {
        if (playerMove.guarding == true) return;
        health -= damage;
        playerAnim.TriggerHit();
        playerMove.Push(new Vector2(-transform.right.x*10,3) );//이거 뭔가 이상하게 작동함.

    }
}
