using UnityEngine;
using Photon.Realtime;

public class PlayerAttack : MonoBehaviour
{
    private void attack(int damage)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.right, 0.5f);
        if (hit2D.collider != null)
        {
            hit2D.collider.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }
    
    
}
