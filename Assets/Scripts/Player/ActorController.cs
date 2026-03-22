using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ActorController : MonoBehaviour
{
    Rigidbody rb;
    private Vector3 appliedMovement = Vector3.zero; //holds the movement vector that is eventually applied to the player
    public Vector3 AppliedMovmement => appliedMovement; //read only copy of the appliedMovement for other scripts to read
    private Quaternion appliedRotation { get; set; } = Quaternion.identity; //holds the rotation quaternion that is eventually applied to the player

    public float speed = 10f;
    public float gravityForce = 3f;
    public Vector2 minAndMaxVerticalMovementSpeed = new Vector2(-30f, 30f);

    [HideInInspector]
    public bool isGrounded = false;
    Vector3 groundNormal = Vector3.zero; //used to calculate the slope the player is on
    float maxSlopeAngle = 45; //degrees
    float currentSlopeAngle = 0f;
    [HideInInspector]
    public Vector3 externalMovement = Vector3.zero;

    public Collision actorCollision = new Collision();
    [HideInInspector]
    public float airDrag = 0.15f;
    [HideInInspector]
    public float groundDrag = 0.5f;

    [HideInInspector]
    public float downwardGravityModifier = 0.6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //UPDATE METHOD
    public void CalculateMovement(Vector3 inputVector, Vector3 newTransformForward)
    {
        //stops residual velocity from collisions from affecting player movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //splits movement into horizontal and vertical parts to make dealing with
        //gravity and inputs easier
        Vector3 horizontalMovement = inputVector + Vector3.ProjectOnPlane(appliedMovement, Vector3.up);
        Vector3 verticalMovement = Vector3.up * (appliedMovement.y + inputVector.y);

        //applies gravity
        verticalMovement = ApplyGravity(verticalMovement);

        //clamps movement vectors so player speed doesnt increase without bound
        horizontalMovement = ClampMovement(horizontalMovement, speed);
        verticalMovement = ClampMovement(verticalMovement, minAndMaxVerticalMovementSpeed.y);

        //checks if movement is occuring on slope and adjusts it as needed
        (horizontalMovement, verticalMovement) = ProjectMovementOnSlope(horizontalMovement, verticalMovement);

        verticalMovement = CheckAbovePlayer(verticalMovement);

        //calculates the players movement for the next physics step using values from the player input script
        appliedMovement = horizontalMovement + verticalMovement;

        //applies drag force to object, slowing them down
        appliedMovement = DragForce(appliedMovement);

        if(newTransformForward.magnitude == 0f)
        {
            newTransformForward = transform.forward;
        }

        appliedRotation = CalculateQuaternionRotation(newTransformForward);
    }

    //FIXED UPDATE METHOD
    public void ApplyMovement()
    {
        //moves the player
        //ISSUE: if multiple scripts are trying to edit external movement only one of them will be used
        //possible fix is to have the external movement be added to not set by other scripts and then zeroed out by player script after movement is applied

        rb.Move(rb.position + appliedMovement * Time.fixedDeltaTime + externalMovement, rb.rotation * appliedRotation);

        //Checks to make sure character hasnt flipped updside down
        //rotates the player so their "forward" stays the same and their "up" matches world up
        //May cause rare movement instability
        if (transform.up != Vector3.up)
        {
            rb.MoveRotation(Quaternion.LookRotation(transform.forward, Vector3.up));
        }

        if (isGrounded)
        {
            externalMovement = Vector3.zero;
        }

        //sets ground variables to false every fixed update call because OnCollisionEnter will always determine if its true or not before next fixed update is called
        ResetGroundVariables();
    }

    Vector3 ClampMovement(Vector3 _movement, float upperLimit)
    {
        //soft clamps the speed
        //now when above the set speed, an additional drag factor is added so to get the player back to the max speed
        //this lets the actor temporarily exceed the max speed for abilities such as dashing or very powerful jumping
        float terminalDragFactor = 0.3f;
        float adjustedSpeed = _movement.magnitude;
        if (_movement.magnitude > upperLimit)
        {
            adjustedSpeed = Mathf.Clamp(_movement.magnitude - upperLimit, 0f, Mathf.Infinity) * terminalDragFactor;
            if (adjustedSpeed < upperLimit && _movement.magnitude > upperLimit)
            {
                adjustedSpeed = upperLimit;
            }
        }

        return _movement.normalized * adjustedSpeed;
    }

    Vector3 ApplyGravity(Vector3 _verticalMovement)
    {
        //zero out gravity when grounded to stop player from phasing into the ground
        if (isGrounded)
        {
            return Vector3.zero;
        }

        //lessen graivty when falling to make player floatier
        if (AppliedMovmement.y < 0f)
        {
            return _verticalMovement += Vector3.down * downwardGravityModifier * gravityForce;
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
            dragAmount = groundDrag;
        }
        else
        {
            dragAmount = airDrag;
        }

        //creates a vector in the opposite direction of the inputted movement to create a drag force
        Vector3 dragAdjustedMovement = _startingMovement - _startingMovement.normalized * dragAmount;

        //if the drag force is so much that is causes the player to switch directions, the movement amound is just set to 0
        if (Vector3.Dot(_startingMovement.normalized, dragAdjustedMovement.normalized) < 0f)
        {
            dragAdjustedMovement = Vector3.zero;
        }

        return dragAdjustedMovement;
    }

    Vector3 CheckAbovePlayer(Vector3 _inputVector)
    {
        //creates layermask only for player and ignore raycast. may need to be expanded as enemies that use this script is expanded
        LayerMask layerMask = LayerMask.GetMask("IgnoreRaycast", "Player");
        layerMask = ~layerMask; //inverts mask so everything but player and ignore raycast are checked

        //checks if the players head is current hitting anything
        //removes upward movement if this is the case
        Vector3 positionCheck = transform.position + Vector3.up * 0.5f; //position of the player's head
        float sphereRadius = 0.35f; //slightly smaller than player radius because player slightly phases into walls when pushing against them 
                                    //and this avoid triggering when nothing is above
        if(Physics.CheckSphere(positionCheck, sphereRadius, layerMask, QueryTriggerInteraction.Ignore))
        {
            //if the player is hitting their head, cancel any upward velocity
            _inputVector.y = Mathf.Clamp(_inputVector.y, -Mathf.Infinity, 0f);
        }

        return _inputVector;
    }

    (Vector3, Vector3) ProjectMovementOnSlope(Vector3 _horizontalMovement, Vector3 _verticalMovement)
    {
        //final slope check before player movement
        //converts the horizontal movement of the player partially into vertical movement to match the angle of the slope the player is on
        Vector3 movementProjectedOnSlope = Vector3.ProjectOnPlane(_horizontalMovement, groundNormal).normalized * _horizontalMovement.magnitude;

        Vector3 horizontalComponent = _horizontalMovement * Mathf.Cos(currentSlopeAngle * Mathf.Deg2Rad);
        Vector3 verticalComponent = Vector3.up * (_horizontalMovement * Mathf.Sin(currentSlopeAngle * Mathf.Deg2Rad)).magnitude * Mathf.Sign(movementProjectedOnSlope.y);

        //if the player is on a slope of less than the max allowed slope angle, nothing further needs to be done
        if (currentSlopeAngle < maxSlopeAngle)
        {
            return (_horizontalMovement, _verticalMovement + verticalComponent);
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

    Quaternion CalculateQuaternionRotation(Vector3 newForward)
    {
        //Quaternion.FromToRotation was occassionally flipping the player upside down/making the player rotation freak out
        //manually calculating the rotation matrix appears to have removed these issues
        Quaternion from = Quaternion.LookRotation(transform.forward, Vector3.up);
        Quaternion to = Quaternion.LookRotation(Vector3.ProjectOnPlane(newForward, Vector3.up), Vector3.up);

        Quaternion rot = to * Quaternion.Inverse(from);
        return rot;
    }

    void ResetGroundVariables()
    {
        //these variables are used for player logic when the player is grounded or not
        //needs to be reset every fixed update or else player controller will think its grounded even when its not
        isGrounded = false;
        groundNormal = Vector3.zero;
        currentSlopeAngle = 0f;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        /*
         * function triggers every fixedUpdate frame that a collision is occurring with the players collider
         * Vector3.down * (0.5f) sets the collision check point exactly at the place in the player capsule collider where it transitions from
         * a cylinder to a sphere. to avoid issue where player becomes grounded when just touching a wall, a small bufferDst was added to lower the point onto the sphere
         * this point can only be hit on a steep slope and not a flat wall
         */

        float bufferDst = 0.01f; //small distance to help false positive grounding
        actorCollision = collision;

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
        actorCollision = collision;

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
}
