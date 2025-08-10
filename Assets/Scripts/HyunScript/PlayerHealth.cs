using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun, IDamageable
{
    public float hp = 100f;

    [PunRPC] public void RPC_TakeDamage(int dmg) => TakeDamage(dmg);

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log($"[HP] {name} -{damage} ¡æ {hp}");
    }
}







