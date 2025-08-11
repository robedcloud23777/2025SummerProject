using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks, IHitable
{
    
    public float health = 100f;
    public PlayerKickAttack kickAttack;
    public PlayerAttack punchAttack;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            punchAttack.Attack(); 
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            kickAttack.Attack();
            
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
    }

}



