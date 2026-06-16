using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider peasantSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float percentPeasant = (float)Enemy_Spawner.numberOfEnemies / (float)Enemy_Spawner.maxNumberOfEnemies;
        peasantSlider.value = percentPeasant;
    }
}
