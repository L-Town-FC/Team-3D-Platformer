using UnityEngine;

public class BasicEnemy : ActorController
{
    [SerializeField] private Transform player;
    [SerializeField] private float stopDistance = 0.75f;

    SphereCollider sphereCollider;
    float chaseDistance = 10f; //max distance from the player that an enemy can continue chasing them
    Vector3 dir = Vector3.zero;

    bool isChasePlayer = false; //if the enemy is actively chasing the player
    string playerTag = "Player";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //player detection is done by checking if the player is within a sphere collider centered on the enemy
        //changing the size of the collider changes the max chase range
        sphereCollider.radius = chaseDistance;

        if (player == null)
            return;

        dir =  player.position - transform.position;

        //zeros out the enemy inputs if they arent actively chasing the player
        if (isChasePlayer)
        {
            base.newTransformForward = dir;
            base.inputVector = transform.forward;
        }
        else
        {
            base.newTransformForward = transform.forward;
            base.inputVector = Vector3.zero;
        }

        float dist = dir.magnitude;
        if (dist <= stopDistance || dist < 0.001f)
        {
            return;
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        //when player enters enemy range, let the enemy chase them
        if (other.CompareTag(playerTag))
        {
            isChasePlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //when the player exits enemy range, stop them from chasing
        if (other.CompareTag(playerTag))
        {
            isChasePlayer = false;
        }
    }
}
