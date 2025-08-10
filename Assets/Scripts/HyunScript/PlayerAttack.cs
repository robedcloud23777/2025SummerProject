using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

public class PlayerAttack : MonoBehaviourPun
{
    public Transform hitObject;
    
    
    [PunRPC]
    private void Attack()
    {
        
    }
    private IEnumerator AttackRoutine()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll( hitObject.position, 0.1f ,1 << LayerMask.NameToLayer("Player") );

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject != gameObject)
                {
                    print(hit.gameObject.name);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
    
}



