using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public static int numberOfEnemies;
    public static int maxNumberOfEnemies;
    [SerializeField] public int maxEnemies;
    [SerializeField] private float minSpawnDelay = 5f;
    [SerializeField] private float maxSpawnDelay = 5f;
    private float curSpawnDelay;
    [SerializeField] private float playerCheckDistance = 5f;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private GameObject enemyToSpawn;

    // Push this value to all other instance
    private void OnValidate()
    {
        maxNumberOfEnemies = maxEnemies;
        Enemy_Spawner[] allSpawners = FindObjectsByType<Enemy_Spawner>();
        foreach (Enemy_Spawner spawner in allSpawners)
        {
            spawner.maxEnemies = maxNumberOfEnemies;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        numberOfEnemies = 0;
    }

    // Update is called once per frame
    void Update()
    {
        curSpawnDelay -= Time.deltaTime;

        if(curSpawnDelay <= 0)
        {
            // Check For Nearby Players
            bool nearbyPlayer = Physics.CheckSphere(transform.position, playerCheckDistance, playerLayerMask);

            // If ther is no nearby player allow enemy spawn
            if (!nearbyPlayer && numberOfEnemies < maxEnemies)
            {
                numberOfEnemies++;
                Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            }

            curSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, playerCheckDistance);
    }
}
