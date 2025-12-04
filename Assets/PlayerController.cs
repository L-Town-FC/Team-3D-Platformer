using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
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
        isGrounded = GroundCheck();

        //calculates the players movement for the next physics step using values from the player input script
        appliedMovement = transform.TransformDirection(input.movementInput);

        if (!isGrounded)
        {
            ApplyGravity();
        }


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

    void ApplyGravity()
    {
        appliedMovement += Vector3.down * gravityForce;
    }

    bool GroundCheck()
    {
        RaycastHit hitInfo;

        if(rb.SweepTest(-transform.up, out hitInfo, groundCheckDst)){
            return true;
        }
        return false;
    }
}
