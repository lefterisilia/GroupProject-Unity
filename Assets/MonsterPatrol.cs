using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 1f;
    public float resumeDelay = 5f; // Time to wait after losing player

    private int currentPoint = 0;
    private float waitTimer = 0f;
    private float lostTimer = 0f;
    private NavMeshAgent agent;
    private MonsterAI ai;

    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<MonsterAI>();
        GoToNextPoint();
    }

    void Update()
    {
        // Skip if chasing player
        if (ai.IsChasingPlayer())
        {
            lostTimer = 0f;
            return;
        }

        // Timer after losing player
        if (ai.WasRecentlyChasing())
        {
            lostTimer += Time.deltaTime;
            if (lostTimer < resumeDelay) return;
        }

        // Patrol logic
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
            }

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                GoToNextPoint();
                isWaiting = false;
            }
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPoint].position);
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }
}
