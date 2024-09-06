using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static string currentPlayerUsername;
    public static string apiUrl;
    public static string gameToken;
    public static string scoreId;

    public Image playerChoiceImage;
    public Image computerChoiceImage;
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
    public Color winColor = new Color(0.1f, 0.7f, 0.1f); // dark green
    public Color loseColor = new Color(0.6f, 0, 0); // maroon

    public AudioClip[] soundEffects;
    public AudioSource audioSource;

    private int playerChoice;
    private int computerChoice;

    private void Start()
    {
        resultText.text = null;
        adviceText.gameObject.SetActive(true);
        playerChoiceImage.sprite = defaultSprite;
        computerChoiceImage.sprite = defaultSprite;
    }

    public void SetChoice(int choice)
    {
        playerChoice = choice;
        switch (choice)
        {
            case 1:
                playerChoiceImage.sprite = rockSprite;
                break;
            case 2:
                playerChoiceImage.sprite = paperSprite;
                break;
            case 3:
                playerChoiceImage.sprite = scissorsSprite;
                break;
            default:
                break;
        }
        DetermineComputerChoice();
        DetermineWinner();
        StartCoroutine(StartNextRound());
        rockButton.interactable = false;
        paperButton.interactable = false;
        scissorsButton.interactable = false;
        adviceText.gameObject.SetActive(false);
    }

    IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(2f); // wait for 2 seconds before allowing player to choose again
        resultText.text = null;
        adviceText.gameObject.SetActive(true);
        playerChoiceImage.sprite = defaultSprite;
        computerChoiceImage.sprite = defaultSprite;
        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorsButton.interactable = true;
    }

    private void DetermineComputerChoice()
    {
        computerChoice = Random.Range(1, 4);
        switch (computerChoice)
        {
            case 1:
                computerChoiceImage.sprite = rockSprite;
                break;
            case 2:
                computerChoiceImage.sprite = paperSprite;
                break;
            case 3:
                computerChoiceImage.sprite = scissorsSprite;
                break;
            default:
                break;
        }
    }

    private void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffects.Length)
        {
            audioSource.clip = soundEffects[index];
            audioSource.Play();
        }
    }

    private void DetermineWinner()
    {
        if (playerChoice == computerChoice)
        {
            resultText.text = "Draw!";
            resultText.color = tieColor;
            PlaySoundEffect(1); // play draw sound effect
        }
        else if ((playerChoice == 1 && computerChoice == 3) || (playerChoice == 2 && computerChoice == 1) || (playerChoice == 3 && computerChoice == 2))
        {
            resultText.text = "You Win!";
            resultText.color = winColor;
            PlaySoundEffect(0); // play win sound effect

            StartCoroutine(UpdateScore());
        }
        else
        {
            resultText.text = "Computer Wins!";
            resultText.color = loseColor;
            PlaySoundEffect(2); // play lose sound effect
        }
    }

    IEnumerator UpdateScore()
    {
        string urlInsert = $"{apiUrl}/insertPlayerActivity?token={gameToken}&alias={currentPlayerUsername}&id={scoreId}&operator=add&value=1";

        using (UnityWebRequest www = UnityWebRequest.Post(urlInsert, ""))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Handle the response data here
                string responseData = www.downloadHandler.text;
                Debug.Log("Successfully Update Score: " + responseData);
            }

        }
    }
}
