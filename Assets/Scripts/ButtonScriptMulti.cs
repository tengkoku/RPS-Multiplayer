using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ButtonScriptMulti : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public int choiceValue;
    public GameManagerMulti gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManagerMulti>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            gameManager.SetChoice(choiceValue);
        }
        else
        {
            gameManager.SetChoice2(choiceValue);
        }

    }
}
