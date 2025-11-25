using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;

    private Vector3 appliedMovement = Vector3.zero;
    private Quaternion appliedRotation = Quaternion.identity;
    private PlayerCamera playerCamera;
    [SerializeField]
    float speed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        playerCamera = GetComponent<PlayerCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        //calculates the players movement for the next physics step using values from the player input script
        appliedMovement = transform.TransformDirection(input.movementInput);
        appliedRotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up));
    }

    private void FixedUpdate()
    {
        //moves the player
        rb.Move(rb.position + appliedMovement * speed * Time.fixedDeltaTime, rb.rotation * appliedRotation);
    }
}
