using UnityEngine;

/// <summary>
/// Put this on the HeadStompTrigger child (trigger collider).
/// If the player contacts this trigger from above, kill the enemy and bounce the player.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyStompTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Stomp detection")]
    [SerializeField] private float aboveHeadPadding = 0.05f;
    [SerializeField] private float maxUpwardSpeedAllowed = 0.25f;

    [Header("Bounce")]
    [SerializeField] private float stompBounceVelocity = 18f;

    private BasicEnemy enemy;
    private Collider headTrigger;

    private void Awake()
    {
        enemy = GetComponentInParent<BasicEnemy>();
        headTrigger = GetComponent<Collider>();
        headTrigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only respond to player
        if (!other.CompareTag(playerTag))
            return;

        // Must have a Rigidbody somewhere on the player
        Rigidbody playerRb = other.attachedRigidbody;
        if (playerRb == null)
            return;

        // 1) Player must be above the head trigger
        float playerFeetY = other.bounds.min.y;
        float headBottomY = headTrigger.bounds.min.y;

        if (playerFeetY < headBottomY - aboveHeadPadding)
            return;

        // 2) Reject obvious upward motion
        if (playerRb.linearVelocity.y > maxUpwardSpeedAllowed)
            return;

        // 3) Kill enemy first (disables colliders immediately)
        if (enemy != null)
            enemy.DieByStomp();

        // 4) Bounce player USING Player.cs movement system
        //    (do NOT fight Rigidbody velocity directly)
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.ApplyStompBounce(stompBounceVelocity);
        }
    }
}
