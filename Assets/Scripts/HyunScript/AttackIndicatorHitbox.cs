using UnityEngine;
using Photon.Pun;

public class AttackIndicatorHitbox : MonoBehaviourPun
{
    public int damage = 10;
    public LayerMask targetLayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Hitbox] enter: {other.name} (layer={LayerMask.LayerToName(other.gameObject.layer)})");
        if (other.transform.root == transform.root) return;

        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0) return;

        var pv = other.GetComponentInParent<PhotonView>();
        if (pv != null)
            pv.RPC(nameof(PlayerHealth.RPC_TakeDamage), RpcTarget.All, damage);
        else
            other.GetComponentInParent<IDamageable>()?.TakeDamage(damage);
    }
}

