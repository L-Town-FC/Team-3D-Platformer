using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControls playerControls; //Basic player control action map
    private InputActionMap playerActionMap;

    private Vector2 movementVector;
    private Vector2 cameraVector;

    //vectors holding players inputs for horizontal and camera movement
    public Vector3 movementInput = Vector3.zero;
    public Vector2 cameraInput = Vector2.zero;

    public bool isJump;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerActionMap = playerControls.Player;
        playerActionMap.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementVector = playerActionMap.FindAction("Move").ReadValue<Vector2>();
        cameraVector = playerActionMap.FindAction("Look").ReadValue<Vector2>();

        //Gets the mouse and keyboard inputs for basic movement to be used for the player controller
        movementInput = new Vector3(movementVector.x, 0f, movementVector.y).normalized;
        cameraInput = cameraVector;

        isJump = playerActionMap.FindAction("Jump").IsPressed();
    }
}
