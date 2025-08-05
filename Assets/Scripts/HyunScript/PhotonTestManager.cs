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
        PhotonNetwork.Instantiate("PlayerTest", new Vector3(0,0,0), Quaternion.identity, 0);
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
