using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    public float player1Health = 100f;
    public float player2Health = 100f;
    [SerializeField] private Slider HpSlider1;
    [SerializeField] private Slider HpSlider2;

    private PhotonView pv; // PhotonView 참조

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>(); // 할당
    }

    public void SetHealth(float newHealth)
    {
        if (PhotonNetwork.IsMasterClient)
            player1Health = newHealth;
        else
            player2Health = newHealth;

        HealthUI();
    }


    public void HealthUI()
    {
         HpSlider1.value = player1Health / 100f;
         HpSlider2.value = player2Health / 100f;
    }

    public void SetSliders(Slider s1, Slider s2)
    {
        HpSlider1 = s1;
        HpSlider2 = s2;
    }
}
