using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    private SpawnController _spawnController;
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
        _spawnController = GameObject.FindAnyObjectByType<SpawnController>();
        _spawnController.maxNumberOfEnemies = maxEnemies;
        Enemy_Spawner[] allSpawners = FindObjectsByType<Enemy_Spawner>();
        foreach (Enemy_Spawner spawner in allSpawners)
        {
            spawner.maxEnemies = _spawnController.maxNumberOfEnemies;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnValidate();
        curSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        _spawnController.numberOfEnemies = 0;
        _spawnController.revoltStarted = false;
    }

    private void Awake()
    {
        Start();
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
            if (!nearbyPlayer && _spawnController.numberOfEnemies < maxEnemies)
            {
                _spawnController.numberOfEnemies++;
                Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            }

            curSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        }

        // Start Revlot When Enemies Spawned
        if(_spawnController.numberOfEnemies == _spawnController.maxNumberOfEnemies)
            _spawnController.revoltStarted = true;

        //
        if(_spawnController.revoltStarted)
        {
            minSpawnDelay = 0f;
            maxSpawnDelay = 0.1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, playerCheckDistance);
    }
}
