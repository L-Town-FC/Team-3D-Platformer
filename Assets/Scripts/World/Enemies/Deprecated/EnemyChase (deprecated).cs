using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float stopDistance = 0.75f;

    private Rigidbody rb;
    bool isGrounded;
    Vector3 gravity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //zeroes out linar velocity in case some unaccounted velocity is added and messes up movement
        rb.linearVelocity = Vector3.zero;
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
            return;
        }

        Vector3 dir = toPlayer.normalized;

        gravity += Vector3.down;
        if (isGrounded)
        {
            gravity = Vector3.zero;
        }
;
        //rb.linearVelocity = new Vector3(desired.x, rb.linearVelocity.y, desired.z);
        rb.MovePosition(rb.position + (dir * moveSpeed + gravity) * Time.fixedDeltaTime);

        isGrounded = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        //if its touching anything it must be grounded
        //true in almost all cases and I dont feel like putting in the extra effort to cover edge cases rightnow
        isGrounded = true;
    }

    public void SetPlayer(Transform t)
    {
        player = t;
    }
}
