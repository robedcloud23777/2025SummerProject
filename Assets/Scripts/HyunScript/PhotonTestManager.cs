using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonTestManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("AutoRoom", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 참가 완료: " + PhotonNetwork.CurrentRoom.Name);
    
        Vector3 spawnPos;
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPos = new Vector3(-5, 0, 0);
            GameObject tmp =PhotonNetwork.Instantiate("PlayerTest 1", spawnPos, Quaternion.identity, 0);
            
        }
        else
        {
            spawnPos = new Vector3(5, 0, 0);
            GameObject tmp =PhotonNetwork.Instantiate("PlayerTest", spawnPos, Quaternion.identity, 0);
            tmp.transform.localScale = new Vector3(-1, 1, 1);

        }
        

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("방 참가 실패: " + message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Photon 연결 끊김: " + cause);
    }
}
