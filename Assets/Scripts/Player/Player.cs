using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : ActorController, IDamegeable
{
    //TODO: May need to add wall check to base actor class so vertical drag can be lowered while touching wall
    //(player should have to some friction when moving down wall)

    //TODO: Wall jump works correctly, but the forces are way off and I dont know why

    PlayerInput input;
    PlayerCamera playerCamera; // access to camera so it can be disabled during death or game pause

    bool isJumping = false;
    float jumpStartTime = 0f;

    [SerializeField] float jumpForce = 30f;
    [SerializeField] float maxJumpHoldTime = 0.2f;

    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set {} }
    public float health { get; set; }

    // --- Stomp bounce (triggered by EnemyStompTrigger via ApplyStompBounce) ---
    float stompBounceStartTime = -1f;
    float stompBounceDuration = 0.12f; // tweak for feel (0.08 - 0.15 is a good range)
    float stompBounceForce = 22f;   // set by ApplyStompBounce()

    bool isOnWall = false;
    bool isWallJumping = false;
    Vector3 wallJumpDir = Vector3.zero;

    protected override void Start()
    {
        health = _maxHealth;
        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
        base.Start();
    }

    protected override void Update()
    {
        // --------------------------------------------------------------------
        // 1) CAMERA-RELATIVE MOVEMENT (HORIZONTAL ONLY)
        //    Your old line was:
        //      base.inputVector = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        //
        //    That can introduce a NEGATIVE Y when the camera is tilted down,
        //    which fights your stomp bounce. So we project onto the XZ plane.
        // --------------------------------------------------------------------
        Vector3 camRelative = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        camRelative = Vector3.ProjectOnPlane(camRelative, Vector3.up).normalized; // <-- NEW
        base.inputVector = camRelative;

        // player always faces the direction they are moving in
        base.newTransformForward = base.inputVector;

        // checks conditions/sets variables and states for jumping
        JumpCheck();

        // --------------------------------------------------------------------
        // 2) NORMAL JUMP / JUMP-HOLD
        // --------------------------------------------------------------------
        if (input.isJump && isJumping)
        {
            // holding jump increases height of jump, diminishing until maxJumpHoldTime
            base.inputVector += Vector3.up * jumpForce *
                (1f - Mathf.InverseLerp(jumpStartTime, jumpStartTime + maxJumpHoldTime, Time.time));
        }

        // --------------------------------------------------------------------
        // 3) WALL JUMP / JUMP-HOLD
        // --------------------------------------------------------------------
        if (input.isJump && isWallJumping)
        {
            // holding jump increases height of jump, diminishing until maxJumpHoldTime
            base.inputVector += Vector3.up + wallJumpDir * jumpForce *
                (1f - Mathf.InverseLerp(jumpStartTime, jumpStartTime + maxJumpHoldTime, Time.time));
        }

        // --------------------------------------------------------------------
        // 4) STOMP BOUNCE (MINI JUMP WINDOW)
        //    Old code did: base.inputVector += Vector3.up * bounce...
        //    But if inputVector.y is negative (camera tilt) or small,
        //    adding might not win. So we OVERRIDE Y upward for the window.
        // --------------------------------------------------------------------
        // Stomp bounce (mini jump window)
        if (stompBounceStartTime >= 0f)
        {
            float t01 = Mathf.InverseLerp(
                stompBounceStartTime,
                stompBounceStartTime + stompBounceDuration,
                Time.time
            );

            float bounce = stompBounceForce * (1f - t01);

            // Because base.inputVector is a property, we must modify a local copy then reassign.
            Vector3 iv = base.inputVector;
            iv.y = Mathf.Max(iv.y, bounce);
            base.inputVector = iv;

            if (Time.time >= stompBounceStartTime + stompBounceDuration)
                stompBounceStartTime = -1f;
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

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

            if (input.isJump)
            {
                isJumping = true;
                jumpStartTime = Time.time;
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

        //stops player from wall jupming as soon as they touch the wall. They need to actively try to jump
        //after releasing jump and touching the wall
        if (input.isJump && !isJumping && isOnWall)
        {
            isWallJumping = true;
            jumpStartTime = Time.time;
        }

        return;
    }

    /// <summary>
    /// Called by EnemyStompTrigger when the player stomps an enemy.
    /// Starts a short bounce window using the same movement pipeline as your jump.
    /// </summary>
    public void ApplyStompBounce(float bounceVelocity)
    {
        // Force "air" state so grounded/jump logic doesn't cancel the bounce immediately
        isGrounded = false;

        // Cancel any ongoing jump-hold so bounce isn't weakened/overridden
        isJumping = false;

        // Use the passed value as the bounce force
        stompBounceForce = bounceVelocity;

        // Start (or restart) the bounce window now
        stompBounceStartTime = Time.time;
    }
}
