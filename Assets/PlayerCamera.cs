using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCamera : MonoBehaviour
{
    //TODO: Set up script to set cinemachine values

    PlayerInput input;
    [SerializeField]
    Transform playerCamera;
    [SerializeField]
    Vector2 minAndMaxCameraTilt = new Vector2(-60f, 75f); //angle from horizontal that the player can look up or down
    [SerializeField]
    Vector2 horizontalAndVerticalCameraSensitivity = new Vector2(100f, 100f);

    public Vector3 cameraPanAmount = Vector3.zero;
    

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
    }

    void PanCamera()
    {
        //Get the players input and convert it into a form that the PlayerController script can use
        cameraPanAmount = new Vector3(0f, input.cameraInput.x, 0f).normalized * horizontalAndVerticalCameraSensitivity.x * Time.deltaTime;
    }
}
