using UnityEngine;

public class FlyingEnemy : ActorController, IDamageable
{
    float enemyAlertDst = 10f;
    public bool isAlertedToPlayer = false;
    string playerTag = "Player";
    public FlyingEnemyBaseState currentEnemyState;

    #region Circling Variables
    public float defaultHeightAbovePlayer = 3f;
    public float circlingRadius = 6f;
    public float circlingSpeed = 4f;
    #endregion

    #region Swooping Variables
    public float swoopSpeed = 8f;
    public Vector2 minAndMaxTimeBetweenSwoops = new Vector2(5f, 7f);
    #endregion

    #region Components
    [HideInInspector]
    public Transform player;
    [SerializeField]
    SphereCollider chaseCollider;
    MeshRenderer meshRenderer;
    #endregion

    #region All Flying Enemy States
    public feIdleState idleState = new feIdleState();
    public feCirclingState circlingState = new feCirclingState();
    public feSwoopState swoopState = new feSwoopState();
    #endregion

    #region IDamageable Variables
    public float health { get; set; }
    public float maxHealth { get; set; }
    private float _maxHealth = 100f;
    float contactDamage = 50f;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        chaseCollider.radius = enemyAlertDst;
        //air drag is set to ground drag since to a flying enemy, the air is the ground
        //makes movement easier to handle
        base.airDrag = base.groundDrag;
        ChangeSpeed(circlingSpeed, circlingSpeed);

        maxHealth = health = _maxHealth;

        currentEnemyState = idleState;
        currentEnemyState.EnterState(this);

        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        currentEnemyState.UpdateState(this);

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public void ChangeState(FlyingEnemyBaseState _newState)
    {
        currentEnemyState.ExitState(this);
        currentEnemyState = _newState;
        currentEnemyState.EnterState(this);
    }

    //method called by enemy state classes to update this base classes movement and direction
    public void UpdateActorInputVectors(Vector3 _inputVector, Vector3 _newForward)
    {
        base.inputVector = _inputVector;
        base.newTransformForward = _newForward;
    }

    public void ChangeSpeed(float _horizontalSpeed, float _verticalSpeed)
    {
        //assigns vertical and horizontal speed independent of one another
        speed = _horizontalSpeed;
        minAndMaxVerticalMovementSpeed = new Vector2(-_verticalSpeed, _verticalSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isAlertedToPlayer = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isAlertedToPlayer = false;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        //sets enemy color to red when damaged
        //since it dies it to hits there doesnt need to be additional logic here
        meshRenderer.material.color = Color.red;

        if (health <= 0f)
        {
            health = 0f;
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        //only want to try apply damage ONLY if they are contacting the player
        //this stops them from hurting other enemies
        if (!collision.collider.CompareTag(playerTag)) { return; }

        //apply damage to player through damage interface
        if (collision.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(contactDamage);
        }
    }
}
