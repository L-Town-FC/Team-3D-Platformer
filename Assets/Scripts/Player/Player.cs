using UnityEngine;
using UnityEngine.VFX;
using System.Reflection;

[RequireComponent(typeof(ActorController))]
public class Player :MonoBehaviour, IDamageable
{
    [HideInInspector]
    public ActorController actor;
    public bool isPlayerGrounded => actor.isGrounded; //read only copy of ground check from actor class

    [HideInInspector]
    public PlayerInput input;
    public Collision playerCollision => actor.actorCollision; //read only copy of collisions from base class

    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set { } }
    public float health { get; set; }

    [HideInInspector]
    public int maxJumpCount = 2;
    [HideInInspector]
    public int currentJumpCount = 0;

    //need a better way of doing this
    //perhaps having all player audio clips in a different static script
    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioSource audioSource;

    public VisualEffect doubleJumpVFX;

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
        actor = GetComponent<ActorController>();

        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
        audioSource = GetComponentInChildren<AudioSource>();
        doubleJumpVFX = GetComponent<VisualEffect>();

        health = _maxHealth;

        //generates all possible states the player can be in
        GetAllPlayerStates();

        //TESTING PURPOSES ONLY
        PlayerPrefs.DeleteAll();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateActorInputVectors(Vector3.zero, transform.forward);

        currentPlayerState = pGroundState;
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerState.UpdateState(this);
    }

    void FixedUpdate()
    {
        actor.ApplyMovement();
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
        actor.CalculateMovement(_inputVector, _newForward);
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

        foreach (ContactPoint contact in actor.actorCollision.contacts)
        {

            //Having a dot product of 0 means two vectors are perpendicular
            //to have a normal that is perpendicular to the "up" vector means the wall is vertical (not sloped)
            //made comparison value slightly larger than 0 because floating point bullshit
            if (Vector3.Dot(Vector3.up, contact.normal) <= 0.000001f)
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

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
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
