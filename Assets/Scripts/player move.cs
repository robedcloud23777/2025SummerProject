using Photon.Pun;
using UnityEngine;

public class playermove : MonoBehaviourPun
{
    public float speed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        float moveX = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveX, 0, 0);
        transform.Translate(move * speed * Time.deltaTime);
    }
}
