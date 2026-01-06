using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : ActorController, IDamageable
{
    PlayerInput input;
    PlayerCamera playerCamera; // access to camera so it can be disabled during death or game pause

    //Player State variables
    enum PlayerState {onGround, inAir, onWall, crouched}
    PlayerState currentPlayerState = PlayerState.inAir;

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

    bool isWallJumping = false;
    float defaultDownwardGravityModifier; //base modifier for gravity when player is moving downward
    float onWallGravityModifier = 0.05f; //downward gravity modifier for when player first touches a wall
    float gravityModifierMaxChangeTime = 2f; //how long until normal gravity takes over when on the wall
    float defaultAirDrag;
    float onWallDrag = 1f;
    Vector3 wallJumpDir = Vector3.zero;

    bool isCrouch = false;
    float crouchSpeed = 3f;
    float defaultSpeed;

    float playerStateChangeTime = 0f;

    protected override void Start()
    {
        health = _maxHealth;
        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
        defaultDownwardGravityModifier = downwardGravityModifier;
        defaultAirDrag = airDrag;
        defaultSpeed = speed;
        base.Start();
    }

    protected override void Update()
    {
        // --------------------------------------------------------------------
        // 1) CAMERA-RELATIVE MOVEMENT (HORIZONTAL ONLY)
        // --------------------------------------------------------------------
        base.inputVector = CamRelativeInputVector();

        //Sets player state so future methods are using the same state
        //this should only occur here
        SetPlayerState();

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

        CrouchCheck();

        AttackCheck();

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
        if (currentPlayerState == PlayerState.onGround)
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

        //stops player from wall jupming as soon as they touch the wall. They need to actively try to jump
        //after releasing jump and touching the wall
        //number of jumps is reset when touching a wall. this can be changed in the OnWallCheck method
        if (input.isJump && !isJumping && currentPlayerState == PlayerState.onWall)
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

    Vector3 ProjectOnWallMovement(Vector3 input)
    {
        Vector3 horInputVector = new Vector3(input.x, 0f, input.z); //remove the vertical component

        //wall jump normal is directly away from wall
        //since when on the wall we only care about vertical movement and
        //movement away from the wall, the horizontal vector is projected onto the wallJumpDir
        //so only movement away from the wall remains
        Vector3 projHorInputVector = Vector3.Project(horInputVector, wallJumpDir);

        //check if the new horizontal input vector is in the same direction as the wall normal
        //if not, the player shouldnt try to move towards the wall so cancel all horizontal input

        float dot = Vector3.Dot(projHorInputVector, wallJumpDir);
        if(dot < 0f)
        {
            projHorInputVector = Vector3.zero;
        }

        //the vertical component is then readded to the projected vector
        return new Vector3(projHorInputVector.x, input.y, projHorInputVector.z);
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
                if(hit.transform.TryGetComponent<IDamageable>(out IDamageable damegeable))
                {
                    damegeable.TakeDamage(damegeable.health);
                }
            }
        }
    }

    bool OnWallCheck()
    {
        //OnWallCheck
        //Walls must be perfectly vertical to count as walls
        //if they are vertical then the normal vectors must have no y-component
        int wallContacts = 0;

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
        return wallContacts > 1;
    }

    void CrouchCheck()
    {
        if(!isCrouch && input.isCrouching)
        {
            isCrouch = true;
            transform.localScale = new Vector3(1f, 0.5f, 1f);
            speed = crouchSpeed;
        }

        if (!input.isCrouching || currentPlayerState == PlayerState.onWall)
        {
            isCrouch = false;
            transform.localScale = Vector3.one;
            speed = defaultSpeed;
        }
    }

    void SetPlayerState()
    {
        //incredibly simple state machine that probably shouldnt even be called a state machine
        //need to look into moving this into a separate script to keep the player code clean
        //EnterState only fires when player is transitioning between different states
        //this lets playerStateChangeTime update correctly and not every frame


        if (isGrounded)
        {
            if(currentPlayerState != PlayerState.onGround)
            {
                currentPlayerState = EnterState(PlayerState.onGround);
            }

            downwardGravityModifier = defaultDownwardGravityModifier;
            airDrag = defaultAirDrag;
        }
        else if (OnWallCheck())
        {
            if (currentPlayerState != PlayerState.onWall)
            {
                currentPlayerState = EnterState(PlayerState.onWall);
            }
            base.inputVector = ProjectOnWallMovement(base.inputVector);

            //player gravity changes to very small when first on the wall so they appear to stick to wall
            //this "stick" allows player to make precision jumps
            //over time the player loses their "stick" and starts falling at normal gravity rate
            downwardGravityModifier = Mathf.Lerp(
                onWallGravityModifier, 
                defaultDownwardGravityModifier, 
                Mathf.InverseLerp(playerStateChangeTime, playerStateChangeTime + gravityModifierMaxChangeTime, Time.time)
                );

            //player air drag is increased on wall to stop them from sliding along wall when hitting wall at angle
            airDrag = onWallDrag;

            currentJumpCount = 0;
        }
        else
        {
            if (currentPlayerState != PlayerState.inAir)
            {
                currentPlayerState = EnterState(PlayerState.inAir);
            }
            downwardGravityModifier = defaultDownwardGravityModifier;
            airDrag = defaultAirDrag;
        }
    }

    PlayerState EnterState(PlayerState newPlayerState)
    {
        playerStateChangeTime = Time.time;
        return newPlayerState;
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
