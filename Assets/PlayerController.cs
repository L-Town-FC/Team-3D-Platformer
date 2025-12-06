using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //TODO: Add the following to jumping
        //variable jump height dependednt on how long jump is held
        //only can only initiate jump when grounded
    //TODO: Make GroundCheck better so normal of contact point is returned
    //TODO: Create slope handling

    Rigidbody rb;
    PlayerInput input; //player input script

    private Vector3 appliedMovement = Vector3.zero; //holds the movement vector that is eventually applied to the player
    private Quaternion appliedRotation = Quaternion.identity; //holds the rotation quaternion that is eventually applied to the player
    [SerializeField]
    float speed = 4f;
    float gravityForce = 1f;
    float groundCheckDst = 0.05f; //small distance so player doesnt accidentally just brute force through ground when falling
    [SerializeField]
    float jumpForce = 7f;
    [SerializeField]
    Vector2 minAndMaxVerticalMovementSpeed = new Vector2(-8f, 10f);

    bool isGrounded = false;
    [SerializeField]
    LayerMask groundLayerMask;

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

        if (input.isJump)
        {
            verticalMovement += Vector3.up * jumpForce; 
        }

        //clamps movement vectors so player speed doesnt increase without bound
        horizontalMovement = ClampMovement(horizontalMovement, -speed, speed);
        verticalMovement = ClampMovement(verticalMovement, minAndMaxVerticalMovementSpeed.x, minAndMaxVerticalMovementSpeed.y);

        //applies drag force to object, slowing them down
        horizontalMovement = DragForce(horizontalMovement);
        verticalMovement = DragForce(verticalMovement);

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
        rb.Move(rb.position + appliedMovement * speed * Time.fixedDeltaTime, rb.rotation * appliedRotation);
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
        //when dealing with grounded vs ungrounded and moving on different materials

        float dragAmount = 0.35f; //arbitrary number just to test
        Vector3 dragAdjustedMovement = _startingMovement - _startingMovement.normalized * dragAmount;
        
        if(Vector3.Dot(_startingMovement.normalized, dragAdjustedMovement.normalized) < 0f)
        {
            dragAdjustedMovement = Vector3.zero;
        }

        return dragAdjustedMovement;
    }

    bool GroundCheck()
    {
        //Checks if capsule matching player is touching the ground
        //adds a tiny extra amount at bottom to stop precision issues

        //currently this will ground players who touch walls marked as ground
        //this can be fixed later by checking if the collision normal vector from
        //this check is NOT horizontal or angled downard
        if(Physics.CheckCapsule(transform.position - Vector3.up * (0.5f + groundCheckDst), transform.position + Vector3.up * 0.5f, 0.5f, groundLayerMask))
        {
            return true;
        }

        return false;
    }
}
