using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : ActorController, IDamegeable
{
    //TODO: Cancel lateral movement except when jumping while on wall to stop players from abusing wall sliding
    //TODO: Have wall drag start high enough that player doesnt move, then decrease over time so they start accelerating downward
    
    //TODO: Implement double jump

    PlayerInput input;
    PlayerCamera playerCamera; // access to camera so it can be disabled during death or game pause

    [SerializeField]
    bool isJumping = false;
    [SerializeField]
    float jumpStartTime = 0f;
    int currentJumpCount = 0;
    int maxJumpCount = 2;

    [SerializeField] float jumpForce = 6f;
    [SerializeField] float maxJumpHoldTime = 0.2f;

    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set {} }
    public float health { get; set; }

    // --- Stomp bounce (triggered by EnemyStompTrigger via ApplyStompBounce) ---
    public float stompBounceStartTime = -1f;
    float stompBounceDuration = 0.12f; // tweak for feel (0.08 - 0.15 is a good range)
    float stompBounceForce = 6f;   // set by ApplyStompBounce()

    bool isOnWall = false;
    bool isWallJumping = false;
    Vector3 wallJumpDir = Vector3.zero;

    [SerializeField]
    float onWallAirDrag = 1.5f;
    float baseAirDrag;

    protected override void Start()
    {
        health = _maxHealth;
        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
        baseAirDrag = airDrag;
        base.Start();
    }

    protected override void Update()
    {
        // --------------------------------------------------------------------
        // 1) CAMERA-RELATIVE MOVEMENT (HORIZONTAL ONLY)
        // --------------------------------------------------------------------
        base.inputVector = CamRelativeInputVector();

        // checks conditions/sets variables and states for jumping
        JumpCheck();

        // --------------------------------------------------------------------
        // 2) NORMAL JUMP / JUMP-HOLD
        // --------------------------------------------------------------------
        if (input.isJump && isJumping)
        {
            base.inputVector += ApplyJump(Vector3.up, jumpForce, jumpStartTime, maxJumpHoldTime);
        }

        // --------------------------------------------------------------------
        // 3) WALL JUMP / JUMP-HOLD
        // --------------------------------------------------------------------
        if (input.isJump && isWallJumping)
        {
            base.inputVector += ApplyJump(Vector3.up + wallJumpDir, jumpForce, jumpStartTime, maxJumpHoldTime);
        }

        // --------------------------------------------------------------------
        // 4) STOMP BOUNCE (MINI JUMP WINDOW)
        // --------------------------------------------------------------------
        if (stompBounceStartTime >= 0f)
        {
            ApplyStompBounce();
        }

        AttackCheck();

        //player friction increased when "OnWall". This slows the players descent and allows for more precise wall jumps
        //can be improved by having the drag start higher and then shrink over time
        //player can also currently abuse this by sliding along wall so lateral movement will need to be cancelled
        if (isOnWall)
        {
            airDrag = onWallAirDrag;
        }
        else
        {
            airDrag = baseAirDrag;
        }

        //player always faces the direction they are moving in
        //this is done at the end because of the movement abilties that automatically change the players
        //input vector (wall jump)
        base.newTransformForward = base.inputVector;

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //Converts input vector from world vector to input vector based on the camera
    //makes movement feel better
    Vector3 CamRelativeInputVector()
    {
        //    Your old line was:
        //      base.inputVector = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        //
        //    That can introduce a NEGATIVE Y when the camera is tilted down,
        //    which fights your stomp bounce. So we project onto the XZ plane.
        Vector3 camRelative = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        camRelative = Vector3.ProjectOnPlane(camRelative, Vector3.up).normalized; // <-- NEW
        return camRelative;
    }

    #region IDamageable methods
    public void Die()
    {
        DeathMenuController deathMenu = FindFirstObjectByType<DeathMenuController>();
        playerCamera.enabled = false; // disables player's ability to move the camera when they are dead
        deathMenu.Die();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (health == 0f)
            Die();
    }
    #endregion

    #region Movement Methods
    void JumpCheck()
    {
        //if the player isnt trying to jump, dont trigger a jump
        if (!input.isJump)
        {
            isJumping = isWallJumping = false;
        }

        // checks if the player can start a jump and what time the jump started
        if (isGrounded)
        {
            //since you cant be considered "OnWall" when grounded, you can't wall jump
            isWallJumping = false;
            currentJumpCount = 0;

            //start jump when player tries to jump when grounded. Also sets jumpCount variable for 
            //double jump tracking and jumpStartTime for ability to hold jumps
            if (input.isJump)
            {
                isJumping = true;
                jumpStartTime = Time.time;
                currentJumpCount++;
            }

            return;
        }

        //OnWallCheck
        //Walls must be perfectly vertical to count as walls
        //if they are vertical then the normal vectors must have no y-component
        int wallContacts = 0;

        foreach (ContactPoint contact in actorCollision.contacts)
        {
            if(contact.normal.y == 0)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.green);
                //the direction exactly opposite the wall
                wallJumpDir = -contact.normal.normalized;
                wallContacts++;
            }
        }

        //if atleast two points on the player are touching a wall, the player is considered on the wall
        isOnWall = wallContacts > 1;

        //currently resetting jump counter letting player double jump after a wall jump
        if (isOnWall)
        {
            currentJumpCount = 0;
        }

        //stops player from wall jupming as soon as they touch the wall. They need to actively try to jump
        //after releasing jump and touching the wall
        if (input.isJump && !isJumping && isOnWall)
        {
            isWallJumping = true;
            jumpStartTime = Time.time;
            return;
        }

        //checks if player can double jump
        if (input.isJump && !isJumping && currentJumpCount < maxJumpCount)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            currentJumpCount++;
        }

        return;
    }

    //used for "Jump" movement to player whether its standard jump, wall jump, double jump, etc
    Vector3 ApplyJump(Vector3 _jumpDir, float _jumpForce, float _jumpStartTime, float _maxJumpHoldTime)
    {
        // holding jump increases height of jump, diminishing until maxJumpHoldTime
        return _jumpDir * _jumpForce *
                (1f - Mathf.InverseLerp(_jumpStartTime, _jumpStartTime + _maxJumpHoldTime, Time.time));
    }

    //Applies a stomp bounce velocity based on the time a stomp bounce was set
    void ApplyStompBounce()
    {
        //calculates how far through the stomp bounce time the player is
        float t01 = Mathf.InverseLerp(
            stompBounceStartTime,
            stompBounceStartTime + stompBounceDuration,
            Time.time
        );

        //applied bounce amount slowly fades over time to make movement less jarring
        float bounce = stompBounceForce * (1f - t01);

        // Because base.inputVector is a property, we must modify a local copy then reassign.
        Vector3 iv = base.inputVector;
        iv.y = Mathf.Max(iv.y, bounce);
        base.inputVector = iv;

        //resets bounce time so player doesnt have a small infinite bounce applied to them
        if (Time.time >= stompBounceStartTime + stompBounceDuration)
            stompBounceStartTime = -1f;
    }

    /// <summary>
    /// Called by EnemyStompTrigger when the player stomps an enemy.
    /// Starts a short bounce window using the same movement pipeline as your jump.
    /// </summary>
    public void SetStompBounce()
    {
        // Force "air" state so grounded/jump logic doesn't cancel the bounce immediately
        isGrounded = false;

        // Cancel any ongoing jump-hold so bounce isn't weakened/overridden
        isJumping = false;

        // Start (or restart) the bounce window now
        stompBounceStartTime = Time.time;
    }
    #endregion
    void AttackCheck()
    {
        //if the player is attacking, create a sphere in front of them and check the colliders that are hit
        //Trigger checking was disabled so colliders are only checked if the player can collide with them
        if (input.isAttacking)
        {
            LayerMask everythingExceptPlayerMask = ~LayerMask.GetMask("Player"); //creates a layer mask that affects everything except the player
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, 0.5f, everythingExceptPlayerMask, QueryTriggerInteraction.Ignore);
            foreach(Collider hit in hits)
            {
                if(hit.transform.TryGetComponent<IDamegeable>(out IDamegeable damegeable))
                {
                    damegeable.TakeDamage(damegeable.health);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (input == null || !input.isAttacking)
            return;

        float radius = 0.5f;
        Vector3 origin = transform.position + transform.forward;

        Gizmos.color = Color.red;

        // Start sphere
        Gizmos.DrawWireSphere(origin, radius);
    }

}
