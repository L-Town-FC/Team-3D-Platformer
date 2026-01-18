using UnityEngine;

public class Player : ActorController, IDamageable
{
    public bool isPlayerGrounded => isGrounded; //read only copy of ground check from base class

    [HideInInspector]
    public PlayerInput input;
    public Collision playerCollision => actorCollision; //read only copy of collisions from base class

    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set { } }
    public float health { get; set; }

    public int maxJumpCount = 2;
    public int currentJumpCount = 0;

    //need a better way of doing this
    //perhaps having all player audio clips in a different static script
    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioSource audioSource;

    #region All Possible Player States
    public PlayerBaseState currentPlayerState;
    public PlayerGroundState pGroundState;
    public PlayerIdleAirState pIdleAirState;
    public PlayerJumpState pJumpState;
    public PlayerWallState pWallState;
    public PlayerCrouchState pCrouchState;
    public PlayerSuperJumpState pSuperJumpState;
    public PlayerBounceState pBounceState;
    #endregion

    //Event that triggers when the player dies
    public delegate void PlayerDeath();
    public static PlayerDeath playerDeath;

    [HideInInspector]
    public float stompDamage = 50f;

    PlayerCamera playerCamera; // access to camera so it can be disabled during death or game pause

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
        audioSource = GetComponentInChildren<AudioSource>();

        health = _maxHealth;

        //generates all possible states the player can be in
        GetAllPlayerStates();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.inputVector = Vector3.zero;
        base.newTransformForward = transform.forward;

        currentPlayerState = pGroundState;
     
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        currentPlayerState.UpdateState(this);

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void ChangeState(PlayerBaseState _newState)
    {
        currentPlayerState.ExitState(this);
        currentPlayerState = _newState;
        currentPlayerState.EnterState(this);
    }

    //method called by player state classes to update this base classes movement and direction
    public void UpdateActorInputVectors(Vector3 _inputVector, Vector3 _newForward)
    {
        base.inputVector = _inputVector;
        base.newTransformForward = _newForward;
    }

    public Vector3 CamRelativeInputVector()
    {
        Vector3 camRelative = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        camRelative = Vector3.ProjectOnPlane(camRelative, Vector3.up).normalized;
        return camRelative;
    }

    public (bool, Vector3) OnWallCheck()
    {
        //Walls must be perfectly vertical to count as walls
        //if they are vertical then the normal vectors must have no y-component
        int wallContacts = 0;
        Vector3 wallJumpDir = Vector3.up;

        foreach (ContactPoint contact in actorCollision.contacts)
        {
            if (contact.normal.y == 0)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.green);
                //the direction exactly opposite the wall
                wallJumpDir = -contact.normal.normalized;
                wallContacts++;
            }
        }

        //if atleast two points on the player are touching a wall, the player is considered on the wall
        return (wallContacts > 1, wallJumpDir);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (health == 0f)
            Die();
    }

    public void Die()
    {
        playerCamera.enabled = false; // disables player's ability to move the camera when they are dead
        if (playerDeath != null)
        {
            playerDeath.Invoke();
        }
        Destroy(this.gameObject);
    }

    void GetAllPlayerStates()
    {
        pGroundState = new PlayerGroundState();
        pIdleAirState = new PlayerIdleAirState();
        pJumpState = new PlayerJumpState();
        pWallState = new PlayerWallState();
        pCrouchState = new PlayerCrouchState();
        pSuperJumpState = new PlayerSuperJumpState();
        pBounceState = new PlayerBounceState();
    }
}
