using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    public SpriteRenderer indicator; 
    public LayerMask targetLayer;    
    public float range = 1f;           
    public int damage = 10;            

    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Z))
            Attack();
    }

    void Attack()
    {
        Debug.Log("데미지 시작");
        indicator.transform.localScale = new Vector3(range, 1, 1);
        indicator.enabled = true;
        Invoke(nameof(HideIndicator), 0.2f);

        
        Vector2 center = (Vector2)transform.position + Vector2.right * (range / 2f);
        Vector2 size = new Vector2(range, 1f);
        var hits = Physics2D.OverlapBoxAll(center, size, 0, targetLayer);

        
        foreach (var col in hits)
        {
            var pv = col.GetComponent<PhotonView>();
            if (pv != null)
                pv.RPC(nameof(RPC_TakeDamage), pv.Owner, damage);
        }
    }

    void HideIndicator() => indicator.enabled = false;

    [PunRPC]
    void RPC_TakeDamage(int dmg)
    {
        Debug.Log($"[RPC] Received {dmg} on {gameObject.name}");
        GetComponent<IDamageable>()?.TakeDamage(dmg);
    }

}

