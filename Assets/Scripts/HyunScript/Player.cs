using System;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks, IHitable
{
    public float health = 100f;
    public float otherHealth = 100f;
    public PlayerKickAttack kickAttack;
    public PlayerAttack punchAttack;
    private PlayerMove playerMove;
    private PlayerAnimation playerAnim;
    [SerializeField] private Slider HpSlider1;
    [SerializeField] private Slider HpSlider2;
    private Player otherPlayer;

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
        HpSlider1 = GameObject.Find("Canvas/HPBAR1").GetComponent<Slider>();
        HpSlider2 = GameObject.Find("Canvas/HPBAR2").GetComponent<Slider>();
    }

    void Update()
    {
        if (otherPlayer == null)
        {
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (var p in players)
            {
                if (!p.photonView.IsMine)
                {
                    otherPlayer = p;
                    break;
                }
            }
        }

        if (!photonView.IsMine)
            return;
        otherHealth = otherPlayer.health;

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

        HealthUI();

        if (health < 0f)
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

    public void HealthUI()
    {
        HpSlider1.value = health / 100f;
        HpSlider2.value = otherHealth / 100f;
    }
}
