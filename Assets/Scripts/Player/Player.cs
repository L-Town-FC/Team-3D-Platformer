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
    PlayerCamera playerCamera;
    //protected override Vector3 inputVector { get; set; } not sure why this has to be disabled, I assumed this was required
    //protected override Vector3 newTransformForward { get; set; }
    float maxHealth = 100f;
    public float health { get; set; }

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
        base.inputVector = input.movementInput.normalized;
        base.newTransformForward = Camera.main.transform.forward;
        //sets variables for jumping
        JumpCheck();

        if (input.isJump && isJumping)
        {
            //gives player upward velocity when pressing jump
            //holding jump increases height of jump, but the effect decreases until the max jump hold time is reached
            inputVector += Vector3.up * jumpForce * (1f - Mathf.InverseLerp(jumpStartTime, jumpStartTime + maxJumpHoldTime, Time.time));
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

}
