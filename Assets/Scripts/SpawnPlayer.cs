using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public Text[] playerName;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerName[0].text = PhotonNetwork.NickName;
            photonView.RPC("Set_OtherPlayerName", RpcTarget.OthersBuffered, 0, PhotonNetwork.NickName);
        }
        else
        {
            playerName[1].text = PhotonNetwork.NickName;
            photonView.RPC("Set_OtherPlayerName", RpcTarget.OthersBuffered, 1, PhotonNetwork.NickName);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    void Set_OtherPlayerName(int index, string name)
    {
        playerName[index].text = name;
    }
}
