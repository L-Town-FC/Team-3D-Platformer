using UnityEngine;

/// <summary>
/// Attach to each key pickup object.
/// When the player touches it, we:
/// - mark it collected
/// - notify KeyManager
/// - destroy the key GameObject (disappears)
/// </summary>
[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; // Tag used to identify the player.
    private bool collected;                               // Guard against double-triggering.

    private void Reset()
    {
        // Reset() runs when the component is first added or reset in Inspector.
        // Ensure this collider behaves as a pickup trigger.
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        // Defensive: ensure the collider is a trigger even if someone unticked it in Inspector.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent double collection (can happen with multiple colliders / rapid events).
        if (collected)
            return;

        // Support child colliders:
        // If the player's colliders are on child objects, they may not carry the Player tag.
        bool isPlayer =
            other.CompareTag(playerTag) ||
            other.transform.root.CompareTag(playerTag);

        if (!isPlayer)
            return;

        collected = true;

        // Notify the manager (if missing, we log so it's obvious why nothing happens).
        if (KeyManager.Instance != null)
            KeyManager.Instance.RegisterKeyCollected();
        else
            Debug.LogWarning("KeyPickup: No KeyManager found in scene.");

        // Remove the key from the world.
        Destroy(gameObject);
    }
}
