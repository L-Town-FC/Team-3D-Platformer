using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerInput input;

    // Drag your active CinemachineCamera here (the one in your screenshot)
    [SerializeField] private CinemachineCamera cinemachineCamera;

    // Drag the CameraTarget transform here (the same one shown in Tracking Target)
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private Vector2 minAndMaxCameraTilt = new Vector2(-60f, 75f);
    [SerializeField] private Vector2 horizontalAndVerticalCameraSensitivity = new Vector2(1f, 1f);

    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (cameraTarget != null)
        {
            Vector3 e = cameraTarget.rotation.eulerAngles;
            yaw = e.y;
            pitch = e.x;
        }
    }

    private void Update()
    {
        if (input == null || cameraTarget == null) { return; }

        Vector2 look = input.cameraInput;

        yaw += look.x * horizontalAndVerticalCameraSensitivity.x;
        pitch -= look.y * horizontalAndVerticalCameraSensitivity.y;

        // clamp pitch (tilt)
        pitch = Mathf.Clamp(pitch, minAndMaxCameraTilt.x, minAndMaxCameraTilt.y);

        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
