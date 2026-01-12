using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControls playerControls;
    private InputActionMap playerActionMap;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction crouchAction;

    public Vector3 movementInput = Vector3.zero;
    public Vector2 cameraInput = Vector2.zero;
    public bool isJump;
    public bool wasJumpPressedThisFrame;
    public bool isAttacking;
    public bool isCrouching;
    public bool lastLookWasGamepad { get; private set; }


    [SerializeField] private float mouseLookMult = 1.0f;
    [SerializeField] private float gamepadLookMult = 120.0f; // typical “degrees/sec-ish” feel

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerActionMap = playerControls.Player;
        moveAction = playerActionMap.FindAction("Move");
        lookAction = playerActionMap.FindAction("Look");
        jumpAction = playerActionMap.FindAction("Jump");
        attackAction = playerActionMap.FindAction("Attack");
        crouchAction = playerActionMap.FindAction("Crouch");

        playerActionMap.Enable();
    }

    private void OnDisable()
    {
        playerActionMap.Disable();
    }

    void Update()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector2 look = lookAction.ReadValue<Vector2>();

        // Detect which device last drove the Look action
        lastLookWasGamepad = lookAction
            .activeControl?.device is Gamepad;

        // Mouse delta = per-frame delta
        // Stick = [-1..1], needs dt scaling
        if (lastLookWasGamepad)
        {
            look *= 120f * Time.deltaTime;   // tune this value
        }

        movementInput = new Vector3(move.x, 0f, move.y).normalized;
        cameraInput = look;

        isJump = jumpAction.IsPressed();
        wasJumpPressedThisFrame = jumpAction.WasPerformedThisFrame();
        isAttacking = attackAction.IsPressed();
        isCrouching = crouchAction.IsPressed();
    }

}
