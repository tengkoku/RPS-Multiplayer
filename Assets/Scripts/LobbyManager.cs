using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public TMP_Text[] playerName;

    public TMP_Text enterPlayerName;
    public TMP_Text roomName;
    //public Text mode;

    public GameObject roomSettingPanel;
    public GameObject waiting;

    public Button startButton;

    RoomOptions options;
    public int maxNumberOfPlayers = 2;
    public int minNumberOfPlayers = 1;

    void Start()
    {
        options = new RoomOptions
        {
            MaxPlayers = (byte)maxNumberOfPlayers,
            IsOpen = true,
            IsVisible = true
        };
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == maxNumberOfPlayers)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    public void JoinRoom()
    {
        if (enterPlayerName.text != "" && roomName.text != "")
        {
            roomSettingPanel.SetActive(false);
            waiting.SetActive(true);

            PhotonNetwork.NickName = enterPlayerName.text;
            PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);
        }
    }

    public void LoadScene()
    {
        PhotonNetwork.LoadLevel("Game_MP");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsMasterClient)
        {
            playerName[0].text = PhotonNetwork.NickName;
            photonView.RPC("Send_PlayersName", RpcTarget.OthersBuffered, 0, PhotonNetwork.NickName);
        }
        else
        {
            playerName[1].text = PhotonNetwork.NickName;
            photonView.RPC("Send_PlayersName", RpcTarget.OthersBuffered, 1, PhotonNetwork.NickName);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        throw new System.NotImplementedException();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        throw new System.NotImplementedException();
    }

    //to sync both players (send to each other)
    [PunRPC]
    void Send_PlayersName(int index, string name)
    {
        playerName[index].text = name;
    }
}
