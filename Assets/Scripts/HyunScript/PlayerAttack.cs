using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
 
    public SpriteRenderer indicator;   
    public BoxCollider2D hitbox;       

    public float range = 1.5f;         
    public float height = 1.0f;       
    public Vector2 offset = new Vector2(0.75f, 0f); 

    
    public int damage = 10;
    public float showTime = 0.2f;

    void Awake()
    {
        
        if (indicator) indicator.enabled = false;
        if (hitbox) hitbox.enabled = false;

        
        Physics2D.queriesHitTriggers = true;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            
            photonView.RPC(nameof(RPC_Attack), RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_Attack()
    {
        if (indicator == null || hitbox == null) return;

       
        float dir = Mathf.Sign(transform.localScale.x); 

        
        Transform t = indicator.transform; 
        t.position = transform.position + new Vector3(dir * offset.x, offset.y, 0f);

        indicator.transform.localScale = new Vector3(range, height, 1f);
        hitbox.size = new Vector2(range, height);
        hitbox.offset = Vector2.zero;

        indicator.enabled = true;
        hitbox.enabled = true;

        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), showTime);
    }

    void Hide()
    {
        if (indicator) indicator.enabled = false;
        if (hitbox) hitbox.enabled = false;
    }
}



