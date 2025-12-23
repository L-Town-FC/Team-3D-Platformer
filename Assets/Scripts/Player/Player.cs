using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : ActorController, IDamegeable
{
    PlayerInput input;
    bool isJumping = false;
    float jumpStartTime = 0f;
    [SerializeField]
    float jumpForce = 30f;
    [SerializeField]
    float maxJumpHoldTime = 0.2f;
    PlayerCamera playerCamera; //access to camera so it can be disabled during death or game pause
    float maxHealth = 100f;
    public float health { get; set; }
    // Stomp bounce queued by EnemyStompTrigger (applied next Update so it plays nice with ActorController)
    float pendingStompBounce = 0f;
    float stompBounceStartTime = -1f;
    float stompBounceDuration = 0.12f; // tweak
    float stompBounceForce = 22f;      // tweak



    //protected override Vector3 newTransformForward { get; set; } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        health = maxHealth;
        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //movements inputs are made relative to the camera
        base.inputVector = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        
        //player always face the direction they are moving in
        base.newTransformForward = base.inputVector;
        //sets variables for jumping
        JumpCheck();

        if (input.isJump && isJumping)
        {
            //gives player upward velocity when pressing jump
            //holding jump increases height of jump, but the effect decreases until the max jump hold time is reached
            base.inputVector += Vector3.up * jumpForce * (1f - Mathf.InverseLerp(jumpStartTime, jumpStartTime + maxJumpHoldTime, Time.time));
        }

        // Stomp bounce: apply upward force for a short duration (like a mini jump)
        if (stompBounceStartTime >= 0f)
        {
            float t01 = Mathf.InverseLerp(stompBounceStartTime, stompBounceStartTime + stompBounceDuration, Time.time);

            // apply a decaying upward push (strong at start, fades to 0)
            base.inputVector += Vector3.up * stompBounceForce * (1f - t01);

            // stop after duration
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
        playerCamera.enabled = false; //disables players ability to move the camera when they are dead
        deathMenu.Die();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if(health == 0f)
        {
            Die();
        }
    }

    void JumpCheck()
    {
        //checks if the player can start a jump and what time the jump started
        if (base.isGrounded)
        {
            if (input.isJump)
            {
                isJumping = true;
                jumpStartTime = Time.time;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    public void ApplyStompBounce(float bounceVelocity)
    {
        Debug.Log($"[PLAYER] ApplyStompBounce force={bounceVelocity} time={Time.time}");

        // Force us to be "in air" so JumpCheck doesn't cancel things
        base.isGrounded = false;

        // Cancel normal jump hold behavior
        isJumping = false;

        // Use bounceVelocity as the "force" for this mini bounce
        stompBounceForce = bounceVelocity;

        // Start bounce window now
        stompBounceStartTime = Time.time;
    }



}
