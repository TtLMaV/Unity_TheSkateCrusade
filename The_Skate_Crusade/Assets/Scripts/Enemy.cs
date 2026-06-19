using UnityEngine;
using UnityEngine.AdaptivePerformance;
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
    [SerializeField] private Animator enemyAnimations;
    [SerializeField] private AudioSource screamSFX;
    private bool animateTop;
    private bool closeEnough;
    private SpawnController _spawnController;

    [Header("Death")]
    [SerializeField] private GameObject[] gibs;
    [SerializeField] private int numberOfDeathGibs;
    [SerializeField] private GameObject bloodParticlesPrefab;
    [SerializeField] private GameObject splatterPrefab;
    [SerializeField] private int numberOfDeathSplatter;
    [SerializeField] private GameObject deathSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //
        _spawnController = FindAnyObjectByType<SpawnController>();

        // Grab Agent Component And Player Reference
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        //
        screamSFX.pitch = Random.Range(0.8f, 1.4f);
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = _spawnController.revoltStarted ? 7 : 3;

        RandomPatrol();
        CreateAnimations();

        if (player == null)
            return;

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (!_spawnController.revoltStarted)
        {
            // Always Update Destination to Be the Players Position
            if (playerDistance < avoidPlayerDistance)
            {
                //
                RunFromPlayer();
                animateTop = true;
            }
            else
            {
                //
                RandomPatrol();
                animateTop = false;
            }
        }
        else
        {
            agent.SetDestination(player.transform.position);
            closeEnough = playerDistance < 5f;
            animateTop = closeEnough;
        }

        //
        if (rb != null)
            rb.linearVelocity = Vector3.zero;


    }

    //
    private void CreateAnimations()
    {
        // Do Animations
        enemyAnimations.SetBool("Walking", agent.desiredVelocity.magnitude > 0.5f);
        enemyAnimations.SetLayerWeight(1, animateTop ? 1f : 0f);
        enemyAnimations.SetBool("Attacking", closeEnough);
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
            Vector3 posOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Instantiate(splatterPrefab, transform.position + posOffset, Quaternion.identity);
        }
        Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
        Instantiate(deathSFX, transform.position, Quaternion.identity);

        //
        PlayerController.Score += _spawnController.revoltStarted ? 5 : 100;
        _spawnController.numberOfEnemies--;
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
