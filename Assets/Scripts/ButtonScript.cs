using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public int choiceValue;
    public GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        gameManager.SetChoice(choiceValue);
    }
}
