using UnityEngine;

/// <summary>
/// Put this on the HeadStompTrigger child (trigger collider).
/// If the player contacts this trigger from above, kill the enemy and optionally bounce the player.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyStompTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Stomp detection")]
    // How far below the head trigger bottom we still accept as "from above"
    [SerializeField] private float aboveHeadPadding = 0.05f;

    // Allow stomps even if the player's RB velocity reads as 0 (common on contact),
    // but reject clearly upward motion.
    [SerializeField] private float maxUpwardSpeedAllowed = 0.25f;

    [Header("Bounce")]
    [SerializeField] private float stompBounceVelocity = 8f;

    private BasicEnemy enemy;
    private Collider headTrigger;

    private void Awake()
    {
        enemy = GetComponentInParent<BasicEnemy>();
        headTrigger = GetComponent<Collider>();

        // Safety: ensure this collider is a trigger
        headTrigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only respond to player
        if (!other.CompareTag(playerTag))
            return;

        // Must have a Rigidbody somewhere on the colliding player object
        Rigidbody playerRb = other.attachedRigidbody;
        if (playerRb == null)
            return;

        // 1) Require the player to be "above" the head trigger (prevents side hits counting as stomps)
        float playerFeetY = other.bounds.min.y;
        float headBottomY = headTrigger.bounds.min.y;

        bool fromAbove = playerFeetY >= (headBottomY - aboveHeadPadding);
        if (!fromAbove)
            return;

        // 2) Reject obvious upward motion (but allow 0 / tiny upward)
        float velY = playerRb.linearVelocity.y;
        if (velY > maxUpwardSpeedAllowed)
            return;

        // 3) Kill the enemy (should despawn via Destroy)
        if (enemy != null)
            enemy.DieByStomp();

        // 4) Optional bounce
        Vector3 v = playerRb.linearVelocity;
        v.y = stompBounceVelocity;
        playerRb.linearVelocity = v;
    }
}
