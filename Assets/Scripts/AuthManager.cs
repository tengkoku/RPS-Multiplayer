using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class AuthManager : MonoBehaviour
{
    private string apiUrl = "tenenet.net/api";
    private string gameToken = "4b45271adc23d3b37b68975b3bce8095";
    private string score_id = "game_score";
    private string board_id = "ldboard_rps"; 

    public TMP_Text usernameInput;
    public TMP_Text leaderboardText;

    public GameObject roomSettingPanel;
    public GameObject leaderboard;

    void Start()
    {
        StartCoroutine(MakeApiRequest());
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void createPlayer()
    {
        StartCoroutine(CreatePlayer());
    }

    private void SubmitName(string s)
    {
        Debug.Log(s);
    }

    public void loginPlayer()
    {
        StartCoroutine(LoginPlayer());
    }

    public void getLeaderboard()
    {
        StartCoroutine(GetLeaderboard());
    }

    IEnumerator MakeApiRequest()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("Authorization", "Bearer " + gameToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Successfully Connect to Server Web");

            GameManager.apiUrl = apiUrl;
            GameManager.gameToken = gameToken;
            GameManager.scoreId = score_id;
        }
    }

    IEnumerator CreatePlayer()
    {
        string username = usernameInput.text;
        string fname = usernameInput.text;
        string lname = usernameInput.text;
        float time = Time.deltaTime;
        string id = time.ToString();
        string urlCreate = $"{apiUrl}/createPlayer?token={gameToken}&alias={username}&id={id}&fname={fname}&lname={lname}";

        using (UnityWebRequest www = UnityWebRequest.Post(urlCreate, ""))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Player created successfully!");

                GameManager.currentPlayerUsername = username;

                LoadScene();
            }
        }
    }

    IEnumerator LoginPlayer()
    {
        string username = usernameInput.text;
        //string url = $"{apiUrl}?token={gameToken}&alias={username}";
        string urlGet = $"{apiUrl}/getPlayer?token={gameToken}&alias={username}";

        using (UnityWebRequest www = UnityWebRequest.Post(urlGet, ""))
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
                Debug.Log("Successfully Get Player: " + responseData);

                GameManager.currentPlayerUsername = username;
                BadgeScript.responseData = responseData;

                LoadScene();
            }

        }
    }

    IEnumerator GetLeaderboard()
    {
        string url = $"{apiUrl}/getLeaderboard?token={gameToken}&id={board_id}";

        using (UnityWebRequest www = UnityWebRequest.Post(url, ""))
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
                Debug.Log("Successfully Get Leaderboard: " + responseData);

                // Parse JSON response
                JSONNode jsonNode = JSON.Parse(responseData);
                JSONArray dataArray = jsonNode["message"]["data"].AsArray;

                // Prepare leaderboard text
                string leaderboardText = "Leaderboard:\n";
                foreach (JSONNode data in dataArray)
                {
                    string alias = data["alias"];
                    string score = data["score"];
                    string rank = data["rank"];

                    leaderboardText += $"{rank}. {alias} - Score: {score}\n";
                }

                // Display leaderboard text on UI element
                this.leaderboardText.text = leaderboardText;

                roomSettingPanel.SetActive(false);
                leaderboard.SetActive(true);
            }

        }
    }
}