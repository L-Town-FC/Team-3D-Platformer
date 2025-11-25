using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCamera : MonoBehaviour
{
    PlayerInput input;
    [SerializeField]
    Transform playerCamera;
    Rigidbody rb;

    public Vector3 cameraPanAmount = Vector3.zero;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        TiltCamera();
    }

    void PanCamera()
    {
        cameraPanAmount = new Vector3(0f, input.cameraInput.x, 0f);
    }

    void TiltCamera()
    {

    }
}
