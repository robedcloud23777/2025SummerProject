using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun.Demo.PunBasics;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;

    [Header("NicknamePanel")]
    public GameObject NicknamePanel;
    public TMP_InputField NickNameInput;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public TMP_InputField RoomInput;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("UserPanel")]
    public GameObject UserPanel;
    public TMP_Text[] PlayerName;
    public Button StartButton;
    public TMP_Text ReadyText;
    private bool[] isReady;

    [Header("ETC")]
    public TMP_Text StatusText;
    public PhotonView PV;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    public void Play()
    {
        DisconnectPanel.SetActive(false);
        NicknamePanel.SetActive(true);
    }

    #region 방리스트 갱신
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<TMP_Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<TMP_Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region 서버연결
    void Awake() => Screen.SetResolution(960, 540, false);

    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        NicknamePanel.SetActive(false);
        LobbyPanel.SetActive(true);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }
    #endregion

    #region 방
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        LobbyPanel.SetActive(false);
        UserPanel.SetActive(true);

        foreach (KeyValuePair<int, Photon.Realtime.Player> entry in PhotonNetwork.CurrentRoom.Players)
        {
            int index = entry.Value.ActorNumber - 1;
            Photon.Realtime.Player player = entry.Value;

            PlayerName[index].text = player.NickName;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        foreach (KeyValuePair<int, Photon.Realtime.Player> entry in PhotonNetwork.CurrentRoom.Players)
        {
            int index = entry.Value.ActorNumber - 1;
            Photon.Realtime.Player player = entry.Value;

            PlayerName[index].text = player.NickName;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 방 정보 갱신, 플레이어가 나갔을 때 처리 (필요 시)
    }
    #endregion

    public void Ready()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("SetReadyState", RpcTarget.All, index - 1);
    }

    [PunRPC]
    void SetReadyState(int playerIndex)
    {
        isReady[playerIndex] = !isReady[playerIndex] ? true : false;
        if (isReady[0]) PlayerName[0].color = Color.green;
        else PlayerName[0].color = Color.yellow;
        if (isReady[1]) PlayerName[1].color = Color.green;
        else PlayerName[1].color = Color.yellow;
        if (PhotonNetwork.IsMasterClient && isReady[0] && isReady[1])
        {
            StartButton.interactable = true;
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main");
        }
    }
}