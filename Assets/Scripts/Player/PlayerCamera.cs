using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerInput input;

    [Header("Auto-bind")]
    [SerializeField] private string cinemachineRigTag = "CinemachineRig";

    private GameObject cinemachineCamera;
    private CinemachineInputAxisController inputAxisController;
    private CinemachineOrbitalFollow orbitFollow;

    [SerializeField] private Vector2 minAndMaxCameraTilt = new Vector2(-60f, 75f);
    [SerializeField] private Vector2 horizontalAndVerticalCameraSensitivity = new Vector2(1f, 1f);
    [SerializeField] private bool invertVerticalControls = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cinemachineCamera = GameObject.FindGameObjectWithTag(cinemachineRigTag);

        if (cinemachineCamera == null)
        {
            Debug.LogError($"[{nameof(PlayerCamera)}] No object tagged '{cinemachineRigTag}' found. " +
                            $"Tag your CinemachineCamera GameObject or change cinemachineRigTag.");
            enabled = false;
            return;
        }

        inputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();
        orbitFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

        if (inputAxisController == null || orbitFollow == null)
        {
            Debug.LogError($"[{nameof(PlayerCamera)}] Cinemachine rig missing required components. " +
                            $"Need {nameof(CinemachineInputAxisController)} and {nameof(CinemachineOrbitalFollow)} on {cinemachineCamera.name}.");
            enabled = false;
            return;
        }

        ApplyTuning();
    }

    private void Update()
    {
        
    }

    private void OnValidate()
    {
        // Keep OnValidate for editor-time tuning, but don't rely on it for runtime correctness
        if (inputAxisController != null && orbitFollow != null)
        {
            ApplyTuning();
        }
    }

    private void ApplyTuning()
    {
        inputAxisController.GetController("Look Orbit X").Input.Gain = horizontalAndVerticalCameraSensitivity.x;
        inputAxisController.GetController("Look Orbit Y").Input.Gain = horizontalAndVerticalCameraSensitivity.y * ConvertBoolToFloat(invertVerticalControls);

        orbitFollow.VerticalAxis.Range = minAndMaxCameraTilt;
    }

    float ConvertBoolToFloat(bool _bool)
    {
        if (_bool)
        {
            return -1f;
        }

        return 1f;
    }
}
