using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Reflection;

public class GameManagerMulti : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    public Image[] playersChoiceImage;

    public Text resultText;
    public Text adviceText;
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;

    public Sprite rockSprite;
    public Sprite paperSprite;
    public Sprite scissorsSprite;
    public Sprite defaultSprite;

    public Color tieColor = Color.black;

    public AudioClip soundEffects;
    public AudioSource audioSource;

    private int playerChoice;
    private int player2Choice;
    //private int[] playersChoice;

    private void Start()
    {
        resultText.text = null;
        adviceText.gameObject.SetActive(true);
        playersChoiceImage[0].sprite = defaultSprite;
        playersChoiceImage[1].sprite = defaultSprite;

        Invoke("DoneChoice", 5f);
    }

    public void SetChoice(int choice)
    {
        playerChoice = choice;
        switch (choice)
        {
            case 1:
                playersChoiceImage[0].sprite = rockSprite;
                break;
            case 2:
                playersChoiceImage[0].sprite = paperSprite;
                break;
            case 3:
                playersChoiceImage[0].sprite = scissorsSprite;
                break;
            default:
                break;
        }

        photonView.RPC("ReceivePlayerChoice", RpcTarget.OthersBuffered, playerChoice);
    }

    public void SetChoice2(int choice)
    {
        player2Choice = choice;
        switch (choice)
        {
            case 1:
                playersChoiceImage[1].sprite = rockSprite;
                break;
            case 2:
                playersChoiceImage[1].sprite = paperSprite;
                break;
            case 3:
                playersChoiceImage[1].sprite = scissorsSprite;
                break;
            default:
                break;
        }

        photonView.RPC("ReceivePlayer2Choice", RpcTarget.OthersBuffered, player2Choice);
    }

    IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(2f); // wait for 2 seconds before allowing player to choose again
        resultText.text = null;
        adviceText.gameObject.SetActive(true);
        playersChoiceImage[0].sprite = defaultSprite;
        playersChoiceImage[1].sprite = defaultSprite;
        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorsButton.interactable = true;

        Invoke("DoneChoice", 5f);
    }

    private void DoneChoice()
    {
        DetermineWinner();
        StartCoroutine(StartNextRound());
        rockButton.interactable = false;
        paperButton.interactable = false;
        scissorsButton.interactable = false;
        adviceText.gameObject.SetActive(false);
    }

    private void DetermineWinner()
    {
        photonView.RPC("Send_PlayersChoice", RpcTarget.OthersBuffered, 0, playerChoice);
        photonView.RPC("Send_PlayersChoice", RpcTarget.OthersBuffered, 1, player2Choice);

        if (playerChoice == player2Choice)
        {
            resultText.text = "Draw!";
            photonView.RPC("Send_Result", RpcTarget.OthersBuffered, "Draw!");

        }
        else if ((playerChoice == 1 && player2Choice == 3) ||
                (playerChoice == 2 && player2Choice == 1) ||
                (playerChoice == 3 && player2Choice == 2) ||
                player2Choice == 0 )
        {
            resultText.text = "Player 1 Wins!";
            photonView.RPC("Send_Result", RpcTarget.OthersBuffered, "Player 1 Wins!");
        }
        else
        {
            resultText.text = "Player 2 Wins!";
            photonView.RPC("Send_Result", RpcTarget.OthersBuffered, "Player 2 Wins!");
        }

        resultText.color = tieColor;
        audioSource.clip = soundEffects;
        audioSource.Play();
    }

    [PunRPC]
    void Send_PlayersChoice(int index, int rps)
    {
        switch (rps)
        {
            case 1:
                playersChoiceImage[index].sprite = rockSprite;
                break;
            case 2:
                playersChoiceImage[index].sprite = paperSprite;
                break;
            case 3:
                playersChoiceImage[index].sprite = scissorsSprite;
                break;
            default:
                break;
        }
    }

    [PunRPC]
    void ReceivePlayerChoice(int choice)
    {
        // Update playerChoice with the received choice from the other player
        playerChoice = choice;
    }

    [PunRPC]
    void ReceivePlayer2Choice(int choice)
    {
        // Update player2Choice with the received choice from the other player
        player2Choice = choice;
    }

    [PunRPC]
    void Send_Result(string result)
    {
        resultText.text = result;
    }
}