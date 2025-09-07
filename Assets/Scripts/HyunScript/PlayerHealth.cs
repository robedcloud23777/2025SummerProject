using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun, IDamageable
{
    public float health = 100f;
    public float otherHealth = 100f;
    private Slider HpSlider1;
    private Slider HpSlider2;
    private PlayerAnimation playerAnim;

    [PunRPC] public void RPC_TakeDamage(int dmg) => TakeDamage(dmg);

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    void Start()
    {
        playerAnim = GetComponent<PlayerAnimation>();
        HpSlider1 = GameObject.Find("Canvas/HPBAR1").GetComponent<Slider>();
        HpSlider2 = GameObject.Find("Canvas/HPBAR2").GetComponent<Slider>();
    }

    void Update()
    {
        HealthUI();

        if (health < 0f)
        {
            //playerAnim.TriggerDie();
        }
    }

    public void HealthUI()
    {
        HpSlider1.value = health;
        HpSlider2.value = otherHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            otherHealth = (float)stream.ReceiveNext();
        }
    }
}