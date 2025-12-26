using UnityEngine;
using UnityEngine.VFX;
public class BasicEnemy : ActorController, IDamegeable
{
    [SerializeField] private Transform player;
    [SerializeField] private float stopDistance = 0.75f;

    SphereCollider sphereCollider;
    [SerializeField] float chaseDistance = 10f; //max distance from the player that an enemy can continue chasing them
    Vector3 dir = Vector3.zero;
    EnemyPlayerInteractions enemyPlayerInteractions;
    bool isChasePlayer = false; //if the enemy is actively chasing the player
    string playerTag = "Player";

    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set { } }
    public float health { get; set; }

    //used for stun determination
    public bool isStunned = false;
    [SerializeField]
    float stunLength = 5f;
    float stunStartTime = 0f;
    VisualEffect stunEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        health = _maxHealth;
        stunEffect = GetComponent<VisualEffect>();
        enemyPlayerInteractions = GetComponent<EnemyPlayerInteractions>();
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
        //this is either from being out of range or that they are currently stunned
        if(isStunned || !isChasePlayer)
        {
            base.newTransformForward = transform.forward;
            base.inputVector = Vector3.zero;
        }
        else
        {
            base.newTransformForward = dir;
            base.inputVector = transform.forward;
        }

        StunCheck();

        float dist = dir.magnitude;
        if (dist <= stopDistance || dist < 0.001f)
        {
            return;
        }

        base.Update();
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
            enemyPlayerInteractions.enabled = true;
        }

        //should change the player when stunned but doesnt work currently
        if (!isStunned)
        {
            stunEffect.Reinit();
        }
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
