using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float stopDistance = 0.75f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector3 toPlayer = player.position - transform.position;

        // stay level (no flying up/down)
        toPlayer.y = 0f;

        float dist = toPlayer.magnitude;
        if (dist <= stopDistance || dist < 0.001f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        Vector3 dir = toPlayer / dist;

        // Move using physics so collisions with ground behave naturally
        Vector3 desired = dir * moveSpeed;
        rb.linearVelocity = new Vector3(desired.x, rb.linearVelocity.y, desired.z);
    }

    public void SetPlayer(Transform t)
    {
        player = t;
    }
}
