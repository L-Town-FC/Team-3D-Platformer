using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //TODO: Drag force doesnt work at all
    //Currently zeroing out vertical movement because it was making it annoying to test

    Rigidbody rb;
    PlayerInput input;

    private Vector3 appliedMovement = Vector3.zero;
    private Quaternion appliedRotation = Quaternion.identity;
    [SerializeField]
    float speed = 10f;
    float gravityForce = 1f;
    float groundCheckDst = 0.05f;
    bool isGrounded = false;

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

        Vector3 horizontalMovement = transform.TransformDirection(input.movementInput);
        Vector3 verticalMovement = Vector3.zero;

        if (!isGrounded)
        {
            verticalMovement = ApplyGravity();
        }

        if (input.isJump)
        {
            verticalMovement += Vector3.up * 10f;
        }


        verticalMovement = Vector3.zero;

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

    Vector3 ApplyGravity()
    {
        return Vector3.down * gravityForce;
    }

    Vector3 DragForce(Vector3 _startingMovement)
    {
        float dragAmount = 0.01f;
        Vector3 dragAdjustedMovement = _startingMovement - _startingMovement.normalized * dragAmount;
        if(Vector3.Dot(appliedMovement.normalized, dragAdjustedMovement.normalized) < 0f)
        {
            dragAdjustedMovement = _startingMovement;
        }

        return dragAdjustedMovement;
    }

    bool GroundCheck()
    {
        RaycastHit hitInfo;

        if(rb.SweepTest(-transform.up, out hitInfo,  gravityForce * groundCheckDst)){
            print(hitInfo.transform.name);
            return true;
        }
        return false;
    }
}
