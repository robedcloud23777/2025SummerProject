using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public string playerPrefabName = "player";

    void Start()
    {
        
        Vector3 spawnPos = new Vector3(Random.Range(-5, 5), 0, 0);
        PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
    }
}
