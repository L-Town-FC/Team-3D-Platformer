using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //TODO: Create slope handling

    Rigidbody rb;
    PlayerInput input; //player input script
    [SerializeField]
    private Vector3 appliedMovement = Vector3.zero; //holds the movement vector that is eventually applied to the player
    private Quaternion appliedRotation = Quaternion.identity; //holds the rotation quaternion that is eventually applied to the player
    [SerializeField]
    float speed = 12f;
    [SerializeField]
    float gravityForce = 5f;
    float groundCheckDst = 0.05f; //small distance so player doesnt accidentally just brute force through ground when falling
    [SerializeField]
    float jumpForce = 30f;
    [SerializeField]
    Vector2 minAndMaxVerticalMovementSpeed = new Vector2(-30f, 30f);

    //Variables used for variable jump height
    bool isJumping = false;
    float jumpStartTime = 0f;
    [SerializeField]
    float maxJumpHoldTime = 0.2f;
    
    [SerializeField]
    bool isGrounded = false;
    [SerializeField]
    LayerMask groundLayerMask; //masks out player layer so groundcheck checks all colliders except the players
    public Vector3 externalMovement = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    //all inputs and calculations to movement and rotation should be done here
    void Update()
    {
        rb.linearVelocity = Vector3.zero; //stops residual velocity from collisions from affecting player movement

        isGrounded = GroundCheck();

        //splits movement into horizontal and vertical parts to make dealing with
        //gravity and inputs easier
        //does NOT handle slopes right now
        Vector3 horizontalMovement = transform.TransformDirection(input.movementInput) + Vector3.ProjectOnPlane(appliedMovement, Vector3.up);
        Vector3 verticalMovement = Vector3.up * appliedMovement.y;

        //need to figure out how to properly cancel gravity while still letting players slide down slopes
        if (!isGrounded)
        {
            verticalMovement += ApplyGravity();
        }
        else
        {
            verticalMovement = Vector3.zero;
        }

        if (input.isJump && isJumping)
        {
            //gives player upward velocity when pressing jump
            //holding jump increases height of jump, but the effect decreases until the max jump hold time is reached
            verticalMovement += Vector3.up * jumpForce * (1f - Mathf.InverseLerp(jumpStartTime, jumpStartTime + maxJumpHoldTime, Time.time)); 
        }

        //clamps movement vectors so player speed doesnt increase without bound
        horizontalMovement = ClampMovement(horizontalMovement, -speed, speed);
        verticalMovement = ClampMovement(verticalMovement, minAndMaxVerticalMovementSpeed.x, minAndMaxVerticalMovementSpeed.y);

        //applies drag force to object, slowing them down
        horizontalMovement = DragForce(horizontalMovement);
        verticalMovement = DragForce(verticalMovement);

        verticalMovement = CheckAbovePlayer(verticalMovement);

        //calculates the players movement for the next physics step using values from the player input script
        appliedMovement = horizontalMovement + verticalMovement;

        //to make camera movement smoother, the FPS camera rotates independently of the player
        //the player rigidbody then rotates to match the new camera forward position
        //may need to extend this to RB movement as well
        appliedRotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up));
    }

    //forces and movements are actually exerted here
    private void FixedUpdate()
    {
        //moves the player
        //ISSUE: if multiple scripts are trying to edit external movement only one of them will be used
        //possible fix is to have the external movement be added to not set by other scripts and then zeroed out by player script after movement is applied
        rb.Move(rb.position + appliedMovement * Time.fixedDeltaTime + externalMovement, rb.rotation * appliedRotation);
    }

    Vector3 ClampMovement(Vector3 _movement, float lowerLimit, float upperLimit)
    {
        float finalSpeed = Mathf.Clamp(_movement.magnitude, lowerLimit, upperLimit);

        return _movement.normalized * finalSpeed;
    }

    Vector3 ApplyGravity()
    {
        return Vector3.down * gravityForce;
    }

    Vector3 DragForce(Vector3 _startingMovement)
    {
        //will potentially have to move to handle separate script since this can get complicated
        //when dealing with moving on different materials

        float dragAmount; //player experiences more drag when grounded than when in the air
        if (isGrounded)
        {
            dragAmount = 0.5f;
        }
        else
        {
            dragAmount = 0.15f;
        }

        //creates a vector in the opposite direction of the inputted movement to create a drag force
        Vector3 dragAdjustedMovement = _startingMovement - _startingMovement.normalized * dragAmount;
        
        //if the drag force is so much that is causes the player to switch directions, the movement amound is just set to 0
        if(Vector3.Dot(_startingMovement.normalized, dragAdjustedMovement.normalized) < 0f)
        {
            dragAdjustedMovement = Vector3.zero;
        }

        return dragAdjustedMovement;
    }
    bool GroundCheck()
    {
        /*
         * Checks if capsule matching player is touching the ground
         * adds a tiny extra amount at bottom to stop precision issues
         * the CheckSphere Radius is slightly smaller and lower down than the actual bottom of the player
         *      this stops the player from being able to move into an object, phasing into it, and setting the Ground Check to true when
         *      the player is just clipped into a wall. This also helps with slope detection
         *      this setup may lead to situations where the player is standing on the very edge of a platform but is technically not grounded
         */

        //slopt detection still needs work tho

        RaycastHit[] contacts = Physics.SphereCastAll(transform.position + Vector3.down * 0.55f, 0.45f, Vector3.down, groundCheckDst, groundLayerMask);
        if(contacts.Length == 0)
        {
            return false;
        }

        float maxSlope = 45f;
        bool initialGroundCheck = false;

        foreach(RaycastHit hit in contacts)
        {
            //the closer the dot product is to 1 the more vertical the normal is or how level the ground is
            float dotProduct = Vector3.Dot(hit.normal.normalized, Vector3.up);

            //compates the dot product to the pre-determined max slope angle
            //if slope is too steep, the player will not be considered grounded
            if (dotProduct > Mathf.Cos(maxSlope * Mathf.Deg2Rad))
            {
                initialGroundCheck = true;
            }
        }

        //lets the player jump when grounded and sets the variables to allow for variable jump height
        if (initialGroundCheck)
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

        return initialGroundCheck;
    }

    Vector3 CheckAbovePlayer(Vector3 _inputVector)
    {
        //do rb sweeptest all to get collision point
        //if no collision point, return original vector
        //if there is a collision point, recalculate vertical movement vector to stop player at that point and disable "is jumping"

        if(Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.5f, Vector3.up, out RaycastHit hitInfo, groundLayerMask))
        {
            _inputVector.y = Mathf.Clamp(_inputVector.y, -Mathf.Infinity, 0f);
        }

        return _inputVector;
    }

}
