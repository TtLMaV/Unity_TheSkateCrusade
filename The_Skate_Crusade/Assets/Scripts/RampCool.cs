using UnityEngine;

public class RampCool : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerController pScript = other.GetComponent<PlayerController>();
            pScript.AddCoolPoint();
        }
    }
}
