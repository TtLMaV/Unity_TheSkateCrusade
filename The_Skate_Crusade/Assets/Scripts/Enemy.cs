using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private Rigidbody rb;
    [SerializeField] private float newPositionCDMin;
    [SerializeField] private float newPositionCDMax;
    private float curNewPositionCD;
    [SerializeField] private float newPositionRange;
    [SerializeField] private float avoidPlayerDistance;

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


    private void RunFromPlayer()
    {
        //
        Vector3 awayFromPlayer = Vector3.Normalize(transform.position - player.transform.position) * 2f;
        NavMesh.SamplePosition(transform.position + awayFromPlayer, out NavMeshHit hit, 2f, 7);
        if (hit.hit)
            agent.SetDestination(hit.position);
    }

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


    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(agent.destination, 0.2f);
        }
    }
}
