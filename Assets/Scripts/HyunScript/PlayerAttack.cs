using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    public GameObject hitObject;
    public float cooldown = 0.3f; 

    private bool isCooldown = false;
    private Coroutine attackCoroutine;

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
        hitObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitObject.SetActive(false);
        attackCoroutine = null; 
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }
}