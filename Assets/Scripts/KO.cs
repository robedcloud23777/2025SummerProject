using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class KO : MonoBehaviourPun
{
    public GameObject panel;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0f; // 시작 시 투명
        panel.SetActive(true);
    }

    private void Update()
    {
        if(HealthManager.Instance.player1Health <= 0 || HealthManager.Instance.player2Health <= 0)
        {
            photonView.RPC("EndPanelActive", RpcTarget.All);
        }
    }

    [PunRPC]
    public void EndPanelActive()
    {
        panel.SetActive(true);
        canvasGroup.DOFade(1f, 1f).SetEase(Ease.Linear);
    }
}