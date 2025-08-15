using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks, IHitable
{
    
    private float health = 100f;
    public PlayerKickAttack kickAttack;
    public PlayerAttack punchAttack;
    private PlayerMove playerMove;
    private PlayerAnimation playerAnim;

    public float Health
    {
        get => health;

        set
        {
            health = value;
            // 체력 닳았을떄 이펙트 추가.
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
            punchAttack.Attack(); 
            playerAnim.TriggerAttack();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            kickAttack.Attack();
            playerAnim.TriggerKick();
        }
    }

    public void Hit(float damage)
    {
        if (playerMove.guarding == true) return;
        health -= damage;
        playerAnim.TriggerHit();
    }

}



