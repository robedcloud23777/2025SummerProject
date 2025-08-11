using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    
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

}



