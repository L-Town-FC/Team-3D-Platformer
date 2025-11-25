using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //TODO: Fix quaternion bullshit
    Rigidbody rb;
    PlayerInput input;

    private Vector3 appliedMovement;
    private Quaternion appliedRotation;
    private PlayerCamera playerCamera;

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
        appliedMovement = transform.InverseTransformDirection(input.movementInput);
        appliedRotation = Quaternion.Euler(playerCamera.cameraPanAmount);
    }

    private void FixedUpdate()
    {
        rb.Move(rb.position + appliedMovement, Quaternion.identity);
    }
}
