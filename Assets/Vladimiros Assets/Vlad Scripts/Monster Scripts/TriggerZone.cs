using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public enum TriggerType { Close, Mid, Far }
    public TriggerType triggerType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            MonsterAI monster = GetComponentInParent<MonsterAI>();
            if (monster != null && playerController != null)
            {
                monster.OnPlayerTriggerEnter(triggerType, playerController);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            MonsterAI monster = GetComponentInParent<MonsterAI>();
            if (monster != null && playerController != null)
            {
                monster.OnPlayerTriggerStay(triggerType, playerController);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggerType == TriggerType.Mid)
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null && !pc.IsCrouching)
            {
                MonsterAI monster = GetComponentInParent<MonsterAI>();
                if (monster != null)
                {
                    monster.OnPlayerTriggerExit(triggerType, other.transform.position);
                }
            }
        }
    }
}
