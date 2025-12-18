using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ActorController : MonoBehaviour
{
    //TODO: movement up slope is slowed down a lot. Player should be able to move same speed on slope as flat surface
    //TODO: player bounces down steep slopes instead of sliding down them
    //TODO: Up slope climbing is broken

    Rigidbody rb;
    [SerializeField]
    private Vector3 appliedMovement = Vector3.zero; //holds the movement vector that is eventually applied to the player
    private Quaternion appliedRotation { get; set; } = Quaternion.identity; //holds the rotation quaternion that is eventually applied to the player
    protected virtual Vector3 newTransformForward { get; set; } = Vector3.zero;
    
    [SerializeField]
    protected float speed = 12f;
    [SerializeField]
    float gravityForce = 5f;
    [SerializeField]
    protected Vector2 minAndMaxVerticalMovementSpeed = new Vector2(-30f, 30f);
    
    [SerializeField]
    protected bool isGrounded = false;
    Vector3 groundNormal = Vector3.zero; //used to calculate the slope the player is on
    float maxSlopeAngle = 45; //degrees
    float currentSlopeAngle = 0f;
    [SerializeField]
    LayerMask groundLayerMask; //masks out player layer so groundcheck checks all colliders except the players
    public Vector3 externalMovement = Vector3.zero;

    protected virtual Vector3 inputVector { get; set; } = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    //all inputs and calculations to movement and rotation should be done here
    protected virtual void Update()
    {
        rb.linearVelocity = Vector3.zero; //stops residual velocity from collisions from affecting player movement

        //splits movement into horizontal and vertical parts to make dealing with
        //gravity and inputs easier
        Vector3 horizontalMovement = transform.TransformDirection(inputVector) + Vector3.ProjectOnPlane(appliedMovement, Vector3.up);
        Vector3 verticalMovement = Vector3.up * (appliedMovement.y + inputVector.y);

        //applies gravity
        verticalMovement = ApplyGravity(verticalMovement);

        //clamps movement vectors so player speed doesnt increase without bound
        horizontalMovement = ClampMovement(horizontalMovement, -speed, speed);
        verticalMovement = ClampMovement(verticalMovement, minAndMaxVerticalMovementSpeed.x, minAndMaxVerticalMovementSpeed.y);

        //checks if movement is occuring on slope and adjusts it as needed
        (horizontalMovement, verticalMovement) = ProjectMovementOnSlope(horizontalMovement, verticalMovement);

        verticalMovement = CheckAbovePlayer(verticalMovement);

        //calculates the players movement for the next physics step using values from the player input script
        appliedMovement = horizontalMovement + verticalMovement;

        //applies drag force to object, slowing them down
        appliedMovement = DragForce(appliedMovement);

        //to make camera movement smoother, the FPS camera rotates independently of the player
        //the player rigidbody then rotates to match the new camera forward position
        //may need to extend this to RB movement as well
        appliedRotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(newTransformForward, Vector3.up));
    }

    //forces and movements are actually exerted here
    protected virtual void FixedUpdate()
    {
        //moves the player
        //ISSUE: if multiple scripts are trying to edit external movement only one of them will be used
        //possible fix is to have the external movement be added to not set by other scripts and then zeroed out by player script after movement is applied

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

    Vector3 ApplyGravity(Vector3 _verticalMovement)
    {
        if (isGrounded)
        {
            return Vector3.zero;
        }

        return _verticalMovement += Vector3.down * gravityForce;
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

    (Vector3, Vector3) ProjectMovementOnSlope(Vector3 _horizontalMovement, Vector3 _verticalMovement)
    {
        //final slope check before player movement
        //converts the horizontal movement of the player partially into vertical movement to match the angle of the slope the player is on
        Vector3 movementProjectedOnSlope = Vector3.ProjectOnPlane(_horizontalMovement, groundNormal).normalized * _horizontalMovement.magnitude;

        Debug.DrawRay(transform.position, movementProjectedOnSlope, Color.red);

        Vector3 horizontalComponent = _horizontalMovement * Mathf.Cos(currentSlopeAngle * Mathf.Deg2Rad);
        Vector3 verticalComponent = Vector3.up * (_horizontalMovement * Mathf.Sin(currentSlopeAngle * Mathf.Deg2Rad)).magnitude * Mathf.Sign(movementProjectedOnSlope.y);

        //if the player is on a slope of less than the max allowed slope angle, nothing further needs to be done
        if (currentSlopeAngle < maxSlopeAngle)
        {
            return (horizontalComponent, _verticalMovement + verticalComponent);
        }

        /*
         * if the player is on a steeper slope, find the vector component that goes up the slope and remove if from the players movement.
         * this means the player can move down the slope or horizontally, but not up the slope
         */

        //projects the global "up" onto the sloped surface to get a vector that
        //has no horizontal component and points to top of slope
        Vector3 upProjectedSlopeVector = Vector3.ProjectOnPlane(Vector3.up, groundNormal);

        //using the upProject vector, the component of the movement vector that is aligns with it can be calculated
        Vector3 projectedMovementUpSlope = Vector3.Project(movementProjectedOnSlope, upProjectedSlopeVector);

        //this component is then subtracted from the intial input vector, thus removing any movement "up slope"
        return (horizontalComponent, _verticalMovement - verticalComponent);
    }

    Vector3 ApplySlopeMovement(Vector3 _inputVector)
    {
        //final slope check before player movement
        //converts the horizontal movement of the player partially into vertical movement to match the angle of the slope the player is on
        Vector3 horizontalMovementProjectedOnSlope = Vector3.ProjectOnPlane(new Vector3(_inputVector.x, 0f, _inputVector.z), groundNormal);

        Debug.DrawRay(transform.position + Vector3.down, horizontalMovementProjectedOnSlope * 5f, Color.red);

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

    private void OnCollisionEnter(Collision collision)
    {
        /*
         * function triggers every fixedUpdate frame that a collision is occurring with the players collider
         * Vector3.down * (0.5f) sets the collision check point exactly at the place in the player capsule collider where it transitions from
         * a cylinder to a sphere. to avoid issue where player becomes grounded when just touching a wall, a small bufferDst was added to lower the point onto the sphere
         * this point can only be hit on a steep slope and not a flat wall
         */

        float bufferDst = 0.01f; //small distance to help false positive grounding

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.point.y > transform.TransformPoint(Vector3.down * (0.5f + bufferDst)).y)
            {
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
