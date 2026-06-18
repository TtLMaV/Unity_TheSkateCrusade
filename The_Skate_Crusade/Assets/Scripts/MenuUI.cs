using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prevScoreText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        prevScoreText.text = "PREVIOUS SCORE: " + PlayerController.Score;
    }

    //
    public void StartGame(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    //
    public void QuitGame()
    {
        Application.Quit();
    }
}
