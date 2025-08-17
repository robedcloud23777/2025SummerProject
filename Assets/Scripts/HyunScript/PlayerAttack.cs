using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    public GameObject hitObject;
    public float cooldown = 0.3f; 

    private bool isCooldown = false;
    private Coroutine attackCoroutine;
    public Rigidbody2D rb;
    public PlayerMove playerMove;
    public void Attack()
    {
        if (isCooldown) return;
        photonView.RPC("AttackRPC", RpcTarget.All);
        StartCoroutine(CooldownRoutine());
    }

    [PunRPC]
    private void AttackRPC()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(AttackRoutine());
        print("rhdrur");
    }

    protected IEnumerator AttackRoutine()
    {
        float timer = 0f;
        rb.linearVelocity = new Vector2(0,rb.linearVelocity.y);
        while (timer < 0.3f)
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