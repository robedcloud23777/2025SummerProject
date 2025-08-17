using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerKickAttack : MonoBehaviourPunCallbacks
{
    public GameObject hitObject;
    public float cooldown = 0.5f; 

    private bool isCooldown = false;
    
    public PlayerMove playerMove;
    public Rigidbody2D rb;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }
    public void Attack()
    {
        if (isCooldown) return;
        photonView.RPC("AttackRPC_", RpcTarget.All);
        StartCoroutine(CooldownRoutine());
    }
    
    [PunRPC]
    private void AttackRPC_()
    {
        StartCoroutine(AttackRoutine());
    }

    protected IEnumerator AttackRoutine()
    {
        //애니메이션 넣음녀 됨
        float timer = 0f;
        rb.linearVelocity = Vector2.zero;
        while (timer < 0.5f)
        {
            playerMove.enabled = false;
            hitObject.SetActive(true);
            timer += Time.deltaTime;
            yield return null;
        }
        playerMove.enabled = true;
        hitObject.SetActive(false);
    }
    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }
}
