using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : ActorController, IDamegeable
{
    //TODO: Need to add literal corner case to OnWallCheck()

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
    bool isOnWall = false;

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
            inputVector += Vector3.up * jumpForce * (1f - Mathf.InverseLerp(jumpStartTime, jumpStartTime + maxJumpHoldTime, Time.time));
        }

        WallJump();

        base.Update();

        isOnWall = false;
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
        if (isGrounded)
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

    void WallJump()
    {
        //checks if the player is touching wall
        int numOfWallContactHits = 0;
        Vector3 wallJumpDirection = Vector3.zero;
        //currently only checks if atleast 2 points are returning that they are touching the wall
        //may need to modify criteria to stop unintended wall jumps
        foreach (ContactPoint contact in actorCollision.contacts)
        {
            if(contact.normal.y == 0)
            {
                numOfWallContactHits++;
                wallJumpDirection = -contact.normal.normalized;
            }
        }

        if(numOfWallContactHits > 1 && !isGrounded)
        {
            isOnWall = true;
        }
    }

}
