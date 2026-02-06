using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(ActorController))]
public class BasicEnemy : MonoBehaviour, IDamageable
{
    ActorController actor;

    [SerializeField] private Transform player;
    [SerializeField] private float stopDistance = 0.75f;

    SphereCollider sphereCollider;
    [SerializeField] float chaseDistance = 10f; //max distance from the player that an enemy can continue chasing them
    Vector3 dir = Vector3.zero;
    bool isChasePlayer = false; //if the enemy is actively chasing the player
    string playerTag = "Player";

    float contactDamage = 50f;
    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set { } }
    public float health { get; set; }

    //used for stun determination
    public bool isStunned = false;
    [SerializeField]
    float stunLength = 5f;
    float stunStartTime = 0f;
    VisualEffect stunEffect;

    private void Awake()
    {
        actor = GetComponent<ActorController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = _maxHealth;
        stunEffect = GetComponent<VisualEffect>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        //player detection is done by checking if the player is within a sphere collider centered on the enemy
        //changing the size of the collider changes the max chase range
        sphereCollider.radius = chaseDistance;

        if (player == null)
            return;

        dir =  player.position - transform.position;

        //zeros out the enemy inputs if they arent actively chasing the player
        //this is either from being out of range or that they are currently stunned
        if(isStunned || !isChasePlayer)
        {
            actor.CalculateMovement(Vector3.zero, transform.forward);
        }
        else
        {
            actor.CalculateMovement(transform.forward, dir);

        }

        StunCheck();

        float dist = dir.magnitude;
        if (dist <= stopDistance || dist < 0.001f)
        {
            return;
        }

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        isStunned = true; //stun the enemy whenever they take damage
        stunStartTime = Time.time;
        stunEffect.Play();

        if (health == 0f)
            Die();
    }

    public void Die()
    {
        Destroy(transform.gameObject);
    }

    void StunCheck()
    {
        //checks if player should be stunned and sets effects accordingly
        if (Time.time > stunStartTime + stunLength)
        {
            isStunned = false;
        }

        //should change the player when stunned but doesnt work currently
        if (!isStunned)
        {
            stunEffect.Reinit();
        }
    }

    void FixedUpdate()
    {
        actor.ApplyMovement();
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

    void OnCollisionEnter(Collision collision)
    {
        //only want to try apply damage ONLY if they are contacting the player
        //this stops them from hurting other enemies
        if (!collision.collider.CompareTag(playerTag)) { return; }

        //apply damage to player through damage interface
        if(collision.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(contactDamage);
        }
    }
}
