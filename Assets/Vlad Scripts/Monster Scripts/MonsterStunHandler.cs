using UnityEngine;
using UnityEngine.AI;

public class MonsterStunHandler : MonoBehaviour
{
    public float stunDuration = 5f;

    private bool isStunned = false;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void TriggerStun()
    {
        if (!isStunned)
        {
            StartCoroutine(StunRoutine());
        }
    }

    private System.Collections.IEnumerator StunRoutine()
    {
        Debug.Log("[Monster] Stunned!");

        isStunned = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(stunDuration);

        Debug.Log("[Monster] Recovered from stun");

        isStunned = false;
        agent.isStopped = false;
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
