using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
        if (HealthManager.Instance.player1Health <= 0 || HealthManager.Instance.player2Health <= 0)
        {
            photonView.RPC("EndPanelActive", RpcTarget.All);
        }
    }

    [PunRPC]
    public void EndPanelActive()
    {
        panel.SetActive(true);

        // 페이드 완료 후 연결 끊고 씬 이동
        canvasGroup
            .DOFade(1f, 1f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 서버 연결 끊기
                PhotonNetwork.Disconnect();

                // 로컬 씬 이동
                SceneManager.LoadScene("Start 1");
            });
    }
}