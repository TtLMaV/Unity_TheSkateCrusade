using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Variables
    private SpawnController _spawnController;
    private PlayerController player;
    [SerializeField] private Slider peasantSlider;
    [SerializeField] private TextMeshProUGUI health_Text;
    [SerializeField] private Image health_Bar;
    [SerializeField] private Image coolPoints;
    [SerializeField] private TextMeshProUGUI score_Text;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject menuPanel;

    public void Start()
    {
        _spawnController = FindAnyObjectByType<SpawnController>();
        player = FindAnyObjectByType<PlayerController>();
    }

    public void UpdateMenu()
    {
        //
        gamePanel.SetActive(!PlayerController.inMenu);
        menuPanel.SetActive(PlayerController.inMenu);
    }

    public void QuitToMenu()
    {
        PlayerController.Score = 0;
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Revolt Bar
        float percentPeasant = (float)_spawnController.numberOfEnemies / (float)_spawnController.maxNumberOfEnemies;
        peasantSlider.value = _spawnController.revoltStarted ? 1f : percentPeasant;

        //
        if (_spawnController.revoltStarted)
        {
            RectTransform RevoltBarTransform = peasantSlider.GetComponent<RectTransform>();
            float NewRotation = Random.Range(-2f, 2f);
            RevoltBarTransform.localRotation = Quaternion.Euler(0f, 0f, NewRotation);
        }

        // Health Bar
        float playerhealth = Mathf.Ceil(PlayerController.Health);
        health_Text.text = playerhealth.ToString();
        health_Bar.fillAmount = playerhealth / 100f;

        //
        coolPoints.fillAmount = ((float)player.coolPoints) / (10f);

        //
        score_Text.text = "SCORE: " + PlayerController.Score;
    }
}
