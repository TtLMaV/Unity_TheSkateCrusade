using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private Rigidbody rb;
    [Header("AI")]
    [SerializeField] private float newPositionCDMin;
    [SerializeField] private float newPositionCDMax;
    private float curNewPositionCD;
    [SerializeField] private float newPositionRange;
    [SerializeField] private float avoidPlayerDistance;

    [Header("Death")]
    [SerializeField] private GameObject[] gibs;
    [SerializeField] private int numberOfDeathGibs;
    [SerializeField] private GameObject bloodParticlesPrefab;
    [SerializeField] private GameObject splatterPrefab;
    [SerializeField] private int numberOfDeathSplatter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grab Agent Component And Player Reference
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Always Update Destination to Be the Players Position
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (playerDistance < avoidPlayerDistance)
        {
            //
            RunFromPlayer();
        }
        else
        {
            //
            RandomPatrol();
        }

        //
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    //
    private void RunFromPlayer()
    {
        //
        Vector3 awayFromPlayer = Vector3.Normalize(transform.position - player.transform.position) * 2f;
        NavMesh.SamplePosition(transform.position + awayFromPlayer, out NavMeshHit hit, 2f, 7);
        if (hit.hit)
            agent.SetDestination(hit.position);
    }
    
    //
    private void RandomPatrol()
    {
        if (curNewPositionCD <= 0)
        {
            //
            float newRandomX = transform.position.x + Random.Range(-newPositionRange, newPositionRange);
            float newRandomZ = transform.position.z + Random.Range(-newPositionRange, newPositionRange);
            Vector3 tryNewPosition = new Vector3(newRandomX, transform.position.y, newRandomZ);
            NavMesh.SamplePosition(tryNewPosition, out NavMeshHit hit, 2f, 7);

            //
            if (hit.hit)
                agent.SetDestination(hit.position);

            //
            curNewPositionCD = Random.Range(newPositionCDMin, newPositionCDMax);
        }
        else
        {
            curNewPositionCD -= Time.deltaTime;
        }
    }

    //
    public void Death()
    {
        //
        for(int i = 0; i < numberOfDeathGibs; i++)
        {
            int newGibID = Random.Range(0, gibs.Length);
            Vector3 posOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            Instantiate(gibs[newGibID], transform.position + posOffset, Quaternion.identity);
        }
        for (int i = 0; i < numberOfDeathSplatter; i++)
        {
            Vector3 posOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            Instantiate(splatterPrefab, transform.position + posOffset, Quaternion.identity);
        }
        Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);

        //
        Enemy_Spawner.numberOfEnemies--;
        Destroy(gameObject);
    }

    //
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(agent.destination, 0.2f);
        }
    }
}
