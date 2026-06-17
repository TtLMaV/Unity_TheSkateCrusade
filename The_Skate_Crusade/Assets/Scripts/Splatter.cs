using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Splatter : MonoBehaviour
{
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    private DecalProjector decal;
    [SerializeField] private float fadeTime;
    private float curfadeTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //
        decal = GetComponent<DecalProjector>();
        float newX = Random.Range(minSize, maxSize);
        float newY = Random.Range(minSize, maxSize);
        decal.size = new Vector3(newX, newY, 6f);

        //
        Quaternion newRotation = Quaternion.Euler(90, Random.Range(0, 360), 0f);
        transform.rotation = newRotation;

        //
        curfadeTime = fadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        //
        float alphafade = (curfadeTime / fadeTime);
        decal.fadeFactor = alphafade;

        //
        curfadeTime -= Time.deltaTime;
        if(curfadeTime <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
