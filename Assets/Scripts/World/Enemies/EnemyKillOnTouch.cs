using UnityEngine;

/// <summary>
/// Kills the player when they collide with the enemy,
/// EXCEPT when the player is stomping the enemy's head (falling onto it).
///
/// This script assumes:
/// - Enemy has a normal (non-trigger) body collider (e.g., CapsuleCollider)
/// - Enemy has a separate head trigger (EnemyStompTrigger) that calls BasicEnemy.DieByStomp()
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyKillOnTouch : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Stomp Rules")]
    // If the player's collider bottom is at/above the enemy's top (minus padding), we treat it as a stomp-style contact
    // and do NOT kill the player.
    [SerializeField] private float stompHeightPadding = 0.15f;

    // If the player is moving downward at or below this, we treat it as a stomp-style contact.
    [SerializeField] private float stompDownwardSpeedThreshold = 0.0f;

    private DeathMenuController deathMenu;
    private BasicEnemy enemy;

    private void Awake()
    {
        deathMenu = FindFirstObjectByType<DeathMenuController>();
        if (deathMenu == null)
            Debug.LogWarning("EnemyKillOnTouch: No DeathMenuController found in scene.");

        // Enemy script on the same object (or parent). If it's on the same GameObject, GetComponent is enough.
        enemy = GetComponent<BasicEnemy>();
        if (enemy == null)
            enemy = GetComponentInParent<BasicEnemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[KILL COLLISION] With: {collision.collider.name} tag={collision.collider.tag} rb={(collision.collider.attachedRigidbody != null)} velY={(collision.collider.attachedRigidbody != null ? collision.collider.attachedRigidbody.linearVelocity.y : 999f)} enemy={name}");

        // Only react to player collisions
        if (!collision.collider.CompareTag(playerTag))
            return;

        // If the enemy has already been stomp-killed (or is in the process), do nothing
        if (enemy != null && enemy.IsDying)
            return;

        // If this collision is effectively a stomp, do NOT kill the player.
        // (The head trigger should handle killing the enemy.)
        if (IsStompContact(collision))
            return;

        // Otherwise, this is a side/bottom hit -> kill/damage player
        if (deathMenu != null)
        {
            if (collision.transform.TryGetComponent<IDamegeable>(out IDamegeable damageable))
            {
                // Kill the player via the damage interface
                damageable.TakeDamage(damageable.health);
            }

            // If you prefer the menu-based death, you can use this instead:
            // deathMenu.Die();
        }
    }

    /// <summary>
    /// Returns true if the contact looks like a stomp (player above enemy and moving downward).
    /// We intentionally keep this simple so it works with typical platformer colliders.
    /// </summary>
    private bool IsStompContact(Collision collision)
    {
        Rigidbody playerRb = collision.collider.attachedRigidbody;
        if (playerRb == null)
            return false;

        // Must be moving downward (or at least not moving upward)
        bool falling = playerRb.linearVelocity.y <= stompDownwardSpeedThreshold;
        if (!falling)
            return false;

        // Player must be above the enemy (rough heuristic using bounds)
        // - player's "feet" approximated by the bottom of their collider bounds
        // - enemy's "top" approximated by the top of THIS collider bounds (the enemy body collider)
        float playerFeetY = collision.collider.bounds.min.y;
        float enemyTopY = GetComponent<Collider>().bounds.max.y;

        bool playerAbove = playerFeetY >= (enemyTopY - stompHeightPadding);

        return playerAbove;
    }
}
