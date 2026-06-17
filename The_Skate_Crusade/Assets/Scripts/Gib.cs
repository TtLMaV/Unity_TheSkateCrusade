using UnityEngine;

public class Gib : MonoBehaviour
{
    //
    [SerializeField] private float destroyCD;
    [SerializeField] private float randomForceStrength;
    private Rigidbody rb;

    void Start()
    {
        Quaternion newRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        Quaternion newForceRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        Vector3 newForce = (newForceRotation * transform.up) * randomForceStrength;
        transform.rotation = newRotation;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = newForce;
    }

    // Update is called once per frame
    void Update()
    {
        destroyCD -= Time.deltaTime;
        if (destroyCD <= 0)
            Destroy(gameObject);
    }
}
