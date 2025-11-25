using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControls playerControls;
    private InputAction movementActions;
    private InputAction cameraActions;

    public Vector3 movementInput = Vector3.zero;
    public Vector2 cameraInput = Vector2.zero;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        movementActions = playerControls.Player.Move;
        movementActions.Enable();

        cameraActions = playerControls.Player.Look;
        cameraActions.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector3(movementActions.ReadValue<Vector2>().x, 0f, movementActions.ReadValue<Vector2>().y).normalized;
        cameraInput = new Vector2(cameraActions.ReadValue<Vector2>().x, cameraActions.ReadValue<Vector2>().y);
    }
}
