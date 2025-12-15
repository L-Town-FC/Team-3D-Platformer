using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //TODO: movement up slope is slowed down a lot. Player should be able to move same speed on slope as flat surface
    //TODO: player bounces down steep slopes instead of sliding down them

    Rigidbody rb;
    PlayerInput input; //player input script
    [SerializeField]
    private Vector3 appliedMovement = Vector3.zero; //holds the movement vector that is eventually applied to the player
    private Quaternion appliedRotation = Quaternion.identity; //holds the rotation quaternion that is eventually applied to the player
    [SerializeField]
    float speed = 12f;
    [SerializeField]
    float gravityForce = 5f;
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
    float currentSlopeAngle = 0f;
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

        Debug.DrawRay(transform.position + Vector3.down, groundNormal * 5f, Color.red);

        //splits movement into horizontal and vertical parts to make dealing with
        //gravity and inputs easier
        Vector3 horizontalMovement = transform.TransformDirection(input.movementInput) + Vector3.ProjectOnPlane(appliedMovement, Vector3.up);
        Vector3 verticalMovement = Vector3.up * appliedMovement.y;

        //need to figure out how to properly cancel gravity while still letting players slide down slopes
        if (!isGrounded)
        {
            (horizontalMovement, verticalMovement) = ApplyGravity(horizontalMovement, verticalMovement);
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

        //sets ground variables to false every fixed update call because OnCollisionEnter will always determine if its true or not before next fixed update is called
        ResetGroundVariables();
    }

    Vector3 ClampMovement(Vector3 _movement, float lowerLimit, float upperLimit)
    {
        //clamps inputs speeds by set limits
        float finalSpeed = Mathf.Clamp(_movement.magnitude, lowerLimit, upperLimit);

        return _movement.normalized * finalSpeed;
    }

    (Vector3, Vector3) ApplyGravity(Vector3 _horizontalMovement, Vector3 _verticalMovement)
    {
        //if the player is on a steep slope, gravity should push them down it instead of being set to zero
        //if they arent on a steep slope, no need to force them downward by applying a diaganol force down slope

        if(currentSlopeAngle > maxSlopeAngle)
        {
            Vector3 slopeAdjustedVector = Vector3.ProjectOnPlane(_verticalMovement, groundNormal);
            _verticalMovement = slopeAdjustedVector.y * Vector3.up;
            _horizontalMovement += new Vector3(slopeAdjustedVector.x, 0f, slopeAdjustedVector.z);
        }
        else
        {
            _verticalMovement += Vector3.down * gravityForce;
        }


        return (_horizontalMovement, _verticalMovement);
    }

    Vector3 DragForce(Vector3 _startingMovement)
    {
        //will potentially have to move to handle separate script since this can get complicated
        //when dealing with moving on different materials

        //slows the player down by creating a movement vector oppsite the direction they are moving

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
        //checks if the players head is current hitting anything
        //removes upward movement if this is the case
        if(Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.5f, Vector3.up, out RaycastHit hitInfo, groundLayerMask))
        {
            _inputVector.y = Mathf.Clamp(_inputVector.y, -Mathf.Infinity, 0f);
        }

        return _inputVector;
    }

    Vector3 ApplySlopeMovement(Vector3 _inputVector)
    {
        //final slope check before player movement
        //converts the horizontal movement of the player partially into vertical movement to match the angle of the slope the player is on
        Vector3 horizontalMovementProjectedOnSlope = Vector3.ProjectOnPlane(new Vector3(_inputVector.x, 0f, _inputVector.z), groundNormal);

        //if the player is on a slope of less than the max allowed slope angle, nothing further needs to be done
        if(currentSlopeAngle < maxSlopeAngle)
        {
            return _inputVector.y * Vector3.up + horizontalMovementProjectedOnSlope;
        }

        /*
         * if the player is on a steeper slope, find the vector component that goes up the slope and remove if from the players movement.
         * this means the player can move down the slope or horizontally, but not up the slope
         */


        //projects the global "up" onto the sloped surface to get a vector that
        //has no horizontal component and points to top of slope
        Vector3 upProjectedSlopeVector = Vector3.ProjectOnPlane(Vector3.up, groundNormal); 

        //using the upProject vector, the component of the movement vector that is aligns with it can be calculated
        Vector3 projectedMovementUpSlope = Vector3.Project(horizontalMovementProjectedOnSlope, upProjectedSlopeVector); 

        //this component is then subtracted from the intial input vector, thus removing any movement "up slope"
        return _inputVector - projectedMovementUpSlope;
    }

    void ResetGroundVariables()
    {
        //these variables are used for player logic when the player is grounded or not
        //needs to be reset every fixed update or else player controller will think its grounded even when its not
        isGrounded = false;
        groundNormal = Vector3.zero;
        currentSlopeAngle = 0f;
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

            groundNormal = contact.normal;

            currentSlopeAngle = Vector3.Angle(groundNormal, Vector3.up);

            if (currentSlopeAngle > maxSlopeAngle)
            {
                continue;
            }

            isGrounded = true;
        }
    }
}
