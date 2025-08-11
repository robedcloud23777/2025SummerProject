using Photon.Pun;
using UnityEngine;

public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Rigidbody2D rb;
    
    public Transform groundCheck;
    private bool isGrounded;
    private IPunObservable _punObservableImplementation;
    
    private Vector2 direction;
    private Vector2 opponentDirection;

    public Animator playerAnim;

    private void Start()
    {
        //playerAnim = GetComponent<PlayerAnimation>();
    }


    private void Update()
    {
        if(!photonView.IsMine) return;
        //playerAnim.SetMove(opponentDirection.x, false);
        Move();
        if(Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        
         
        direction = new Vector2(horizontal,0);
        rb.linearVelocity = new Vector2(horizontal *moveSpeed, rb.linearVelocity.y); ;

        playerAnim.SetFloat("MoveX", horizontal);
        playerAnim.SetBool("IsMoving",horizontal!=0);
        playerAnim.SetBool("IsGround", isGrounded);
       
    }

    public void Jump()
    {
        photonView.RPC("JumpRPC", RpcTarget.All);
    }

    [PunRPC]
    private void JumpRPC()
    {
        
        if (!Physics2D.OverlapCircle(groundCheck.position, 0.1f, 1 << LayerMask.NameToLayer("Ground")))
            return;
        
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        playerAnim.SetTrigger("Jump");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
        }
        else
        {
            opponentDirection = (Vector2)stream.ReceiveNext();
        }
    }
}
