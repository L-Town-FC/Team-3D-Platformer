using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerInput input;

    // Drag your active CinemachineCamera here (the one in your screenshot)
    [SerializeField] GameObject cinemachineCamera;
    private CinemachineInputAxisController inputAxisController;
    private CinemachineOrbitalFollow orbitFollow;

    [SerializeField] private Vector2 minAndMaxCameraTilt = new Vector2(-60f, 75f);
    [SerializeField] private Vector2 horizontalAndVerticalCameraSensitivity = new Vector2(1f, 1f);


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();
        orbitFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
    }

    private void Update()
    {
        
    }

    private void OnValidate()
    {
        //this will have to be moved out of OnValidate when we want these values to be modifiable by UI
        if(inputAxisController != null && orbitFollow != null)
        {
            //sets camera sensitivity
            inputAxisController.GetController("Look Orbit X").Input.Gain = horizontalAndVerticalCameraSensitivity.x;
            inputAxisController.GetController("Look Orbit Y").Input.Gain = horizontalAndVerticalCameraSensitivity.y;

            //set camera tilt constaints
            orbitFollow.VerticalAxis.Range = minAndMaxCameraTilt;
        }
    }
}
