using UnityEngine;

public class DeathPlaneTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private DeathManager deathManager;

    private void Awake()
    {
        if (deathManager == null)
            deathManager = FindFirstObjectByType<DeathManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (deathManager != null)
            deathManager.Die();
    }
}
