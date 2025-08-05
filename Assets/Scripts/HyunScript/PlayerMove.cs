using Photon.Pun;
using UnityEngine;

public class PlayerMove : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private void Update()
    {
        if(!photonView.IsMine)
            return;
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        Vector3 movement = new Vector3(horizontal, 0,0);
        
        rb.linearVelocity = movement * moveSpeed;
        
        
    }
}
