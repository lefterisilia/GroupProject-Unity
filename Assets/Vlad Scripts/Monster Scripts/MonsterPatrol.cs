using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 1f;
    public float resumeDelay = 5f;

    private int currentPoint = 0;
    private float waitTimer = 0f;
    private float lostPlayerTimer = 0f;
    private bool isWaiting = false;
    private bool waitingToResume = false;

    private NavMeshAgent agent;
    private MonsterAI ai;
    private MonsterStunHandler stun;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<MonsterAI>();
        stun = GetComponent<MonsterStunHandler>();

        GoToNextPoint();
    }

    void Update()
    {
        //  If stunned, do nothing
        if (stun.IsStunned()) return;

        //  If chasing player, reset resume timer and stop patrol
        if (ai.IsChasingPlayer())
        {
            lostPlayerTimer = 0f;
            isWaiting = false;
            waitingToResume = true;
            return;
        }

        //  If recently chased, count delay before resuming patrol
        if (waitingToResume)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= resumeDelay)
            {
                waitingToResume = false;
                GoToNextPoint();
            }
            return;
        }

        //  Patrol logic (not chasing or stunned)
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
