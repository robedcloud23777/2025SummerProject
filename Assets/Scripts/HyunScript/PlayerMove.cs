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

    private PlayerAnimation playerAnim;

    [HideInInspector] public bool guarding;

    private void Start()
    {
        playerAnim = GetComponent<PlayerAnimation>();
    }


    private void Update()
    {
        if(!photonView.IsMine) return;
        CheckGround();
        //print(guarding);
        Move();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        
         
        rb.linearVelocity = new Vector2(horizontal *moveSpeed, rb.linearVelocity.y); ;

        
        
        float facingSign = Mathf.Sign(transform.localScale.x);
        guarding = horizontal != 0 && Mathf.Sign(horizontal) != facingSign;

        playerAnim.SetMove((int)horizontal, isGrounded, Mathf.CeilToInt(rb.linearVelocity.y));
    }

   
    
    public void Jump()
    {
        photonView.RPC("JumpRPC", RpcTarget.All);
    }

    [PunRPC]
    private void JumpRPC()
    {
        if (!isGrounded) return;
        playerAnim.TriggerJump();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    private void CheckGround()
    {
        // Ray 길이 (groundCheck 위치에서 아래로 쏘는 길이)
        float rayLength = 0.2f;

        // Raycast를 아래 방향으로 발사
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, rayLength, 1 << LayerMask.NameToLayer("Ground"));

        // 맞은 오브젝트가 있으면 땅에 닿아 있음
        isGrounded = hit.collider != null;

        // 디버그 용: Scene 뷰에 Ray 표시
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(groundCheck.position, Vector2.down * rayLength, rayColor);
    }
}
