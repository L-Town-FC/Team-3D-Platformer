using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathPlaneTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private DeathMenuController deathMenu;

    private void Awake()
    {
        // Find in the active scene at runtime (works in every level)
        deathMenu = FindFirstObjectByType<DeathMenuController>();
        if (deathMenu == null)
            Debug.LogWarning("DeathPlaneTrigger: No DeathMenuController found in scene.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if(other.transform.TryGetComponent<Player>(out Player player))
        {
            player.Die();
        }
    }
}
