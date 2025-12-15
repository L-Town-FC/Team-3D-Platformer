using UnityEngine;

public class DeathPlaneTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private DeathMenuController deathManager;

    private void Awake()
    {
        if (deathManager == null)
            deathManager = FindFirstObjectByType<DeathMenuController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (deathManager != null)
            deathManager.Die();
    }
}
