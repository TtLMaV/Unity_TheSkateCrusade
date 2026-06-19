using UnityEngine;

public class CamRotate : MonoBehaviour
{
    //
    [SerializeField] private float turnSpeed;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + (turnSpeed * Time.deltaTime), 0);
    }
}
