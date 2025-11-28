using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    Transform playerCamera;
    [SerializeField]
    Vector2 minAndMaxCameraTilt = new Vector2(-60f, 75f); //angle from horizontal that the player can look up or down
    [SerializeField]
    Vector2 horizontalAndVerticalCameraSensitivity = new Vector2(1f, 1f); //need to test what are good values for this

    [SerializeField]
    private GameObject FPSCamera;
    private CinemachinePanTilt panTilt;
    private CinemachineInputAxisController cinemachineInput;

    private void Awake()
    {
        panTilt = FPSCamera.GetComponent<CinemachinePanTilt>();
        cinemachineInput = FPSCamera.GetComponent<CinemachineInputAxisController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetCinemachineTiltRange();

        //Will need to make method public to let player change it mid-game from menu
        SetCameraSensivity();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetCinemachineTiltRange()
    {
        //sets the max and min tilt directions of the players camera
        panTilt.TiltAxis.Range.x = minAndMaxCameraTilt.x;
        panTilt.TiltAxis.Range.y = minAndMaxCameraTilt.y;
    }

    void SetCameraSensivity()
    {
        //manually sets the gain value for the cinemachine camera to change camera sensitivity
        //probably not the best way. Could look into changing the input for the CM to use
        //the InputActions asset and then change that but not sure
        if(cinemachineInput != null)
        {
            cinemachineInput.GetController("Look X (Pan)").Input.Gain = horizontalAndVerticalCameraSensitivity.x;
            cinemachineInput.GetController("Look Y (Tilt)").Input.Gain = -horizontalAndVerticalCameraSensitivity.y;
        }
    }

    //for testing only. triggers only when inspector is edited
    private void OnValidate()
    {
        //for testing purposes only
        SetCameraSensivity();
    }
}
