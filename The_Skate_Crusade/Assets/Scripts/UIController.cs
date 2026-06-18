using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Variables
    [SerializeField] private Slider peasantSlider;
    [SerializeField] private TextMeshProUGUI health_Text;
    [SerializeField] private Image health_Bar;
    [SerializeField] private TextMeshProUGUI score_Text;

    // Update is called once per frame
    void Update()
    {
        // Revolt Bar
        float percentPeasant = (float)Enemy_Spawner.numberOfEnemies / (float)Enemy_Spawner.maxNumberOfEnemies;
        peasantSlider.value = Enemy_Spawner.revoltStarted ? 1f : percentPeasant;

        // Health Bar
        float playerhealth = Mathf.Ceil(PlayerController.Health);
        health_Text.text = playerhealth.ToString();
        health_Bar.fillAmount = playerhealth / 100f;

        //
        score_Text.text = "SCORE: " + PlayerController.Score;
    }
}
