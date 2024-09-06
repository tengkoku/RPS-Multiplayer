using UnityEngine;
using SimpleJSON;

public class BadgeScript : MonoBehaviour
{
    public static string responseData;

    public GameObject badge;
    public GameObject badge2;

    private void Start()
    {
        GetBadge();
    }
    public void GetBadge()
    {
        Debug.Log("Successfully Get Badge: " + responseData);

        // Parse JSON response
        JSONNode jsonNode = JSON.Parse(responseData);
        JSONNode message = jsonNode["message"];

        JSONArray dataArray = message["score"].AsArray;

        string metricName = "badges";
        //string metricValue = "badges1";

        foreach (JSONNode data in dataArray)
        {
            if (data["metric_name"] == metricName )
            {
                if (data["value"] == "badges1")
                {
                    badge.SetActive(true);
                }
                else if (data["value"] == "badges2")
                {
                    badge2.SetActive(true);
                }
            }
        }
    }
}
