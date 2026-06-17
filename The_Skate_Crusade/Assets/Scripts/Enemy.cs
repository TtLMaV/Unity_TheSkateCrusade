using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grab Agent Component And Player Reference
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Always Update Destination to Be the Players Position
        Vector3 awayFromPlayer = Vector3.Normalize(transform.position - player.transform.position) * 2f;
        NavMesh.SamplePosition(transform.position + awayFromPlayer, out NavMeshHit hit, 2f, 7);
        if(hit.hit)
            agent.SetDestination(hit.position);
    }
}
