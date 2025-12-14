using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input; //player input script
    [SerializeField]
    private Vector3 appliedMovement = Vector3.zero; //holds the movement vector that is eventually applied to the player
    private Quaternion appliedRotation = Quaternion.identity; //holds the rotation quaternion that is eventually applied to the player
    [SerializeField]
    float speed = 12f;
    [SerializeField]
    float gravityForce = 5f;
    [SerializeField, Range(0f, 0.5f)]
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
    Vector3 groundNormal = Vector3.zero; //used to calculate the slope the player is on
    float maxSlopeAngle = 45; //degrees
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

        //splits movement into horizontal and vertical parts to make dealing with
        //gravity and inputs easier
        Vector3 horizontalMovement = transform.TransformDirection(input.movementInput) + Vector3.ProjectOnPlane(appliedMovement, Vector3.up);
        Vector3 verticalMovement = Vector3.up * appliedMovement.y;

        //need to figure out how to properly cancel gravity while still letting players slide down slopes
        if (!isGrounded)
        {
            verticalMovement += ApplyGravity();
        }
        else
        {
            verticalMovement = Vector3.down * 0.5f;
        }

        //sets variables for jumping
        JumpCheck();

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

        appliedMovement = ApplySlopeMovement(appliedMovement);

        rb.Move(rb.position + appliedMovement * Time.fixedDeltaTime + externalMovement, rb.rotation * appliedRotation);

        if (isGrounded)
        {
            externalMovement = Vector3.zero;
        }

        //sets isGrounded to false every fixed update call because OnCollisionEnter will always determine if its true or not before fixed update is called
        isGrounded = false;
        groundNormal = Vector3.zero;
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

    Vector3 ApplySlopeMovement(Vector3 _inputVector)
    {
        //calculate slope angle

        //if the slope is too steep, cancel movement in that direction and possible add downward movement to player so they slide

        //take ground normal and use it to project horizontal components of input vector onto sloped plane
        
        //use sin and cosine to convert horizontal components of _input vector and turn them into partial horizontal and partial vertical

        //return the new vector

        return _inputVector;
    }

    private void OnCollisionStay(Collision collision)
    {
        /*
         * function triggers every fixedUpdate frame that a collision is occurring with the players collider
         * Vector3.down * (0.5f) sets the collision check point exactly at the place in the player capsule collider where it transitions from
         * a cylinder to a sphere. to avoid issue where player becomes grounded when just touching a wall, a small bufferDst was added to lower the point onto the sphere
         * this point can only be hit on a steep slope and not a flat wall
         */

        float bufferDst = 0.01f; //small distance to help false positive grounding
        
        foreach(ContactPoint contact in collision.contacts)
        {
            if(contact.point.y > transform.TransformPoint(Vector3.down * (0.5f + bufferDst)).y){
                continue;
            }

            isGrounded = true;
            groundNormal = contact.normal;
        }
    }
}
