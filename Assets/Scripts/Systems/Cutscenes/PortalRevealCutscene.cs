using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PortalRevealCutscene : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private ScenePortal portal;                 // Portal to activate at the end
    [SerializeField] private float portalEnableDelay = 0.25f;    // Small dramatic beat before enabling

    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera portalCam; // The cutscene virtual camera
    [SerializeField] private int cutscenePriority = 50;          // Higher than gameplay cam
    [SerializeField] private float holdTime = 1.25f;             // How long to hold on the portal after blend
    [SerializeField] private float blendSafety = 0.75f;          // Extra wait to allow blend time

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour[] disableDuringCutscene; // Player controller scripts to disable
    [SerializeField] private GameObject playerRoot;                 // Optional (for safety / reference)

    private int originalPriority;
    private bool isPlaying;

    public void Play()
    {
        if (isPlaying)
            return;

        if (portalCam == null)
        {
            Debug.LogWarning("PortalRevealCutscene: portalCam is not assigned.");
            if (portal != null)
                portal.SetActive(true);
            return;
        }

        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        isPlaying = true;

        // 1) Disable player control scripts (simple + reliable)
        for (int i = 0; i < disableDuringCutscene.Length; i++)
        {
            if (disableDuringCutscene[i] != null)
                disableDuringCutscene[i].enabled = false;
        }

        // 2) Blend to portal camera (raise priority)
        originalPriority = portalCam.Priority;
        portalCam.Priority = cutscenePriority;

        // Wait for the camera blend to “arrive”.
        // (Cinemachine blends are driven by the Brain; easiest is to wait a short safety window.)
        yield return new WaitForSeconds(blendSafety);

        // 3) Hold on portal for a beat
        yield return new WaitForSeconds(holdTime);

        // 4) Activate portal (collider + visuals) AFTER the camera is there
        if (portal != null)
        {
            yield return new WaitForSeconds(portalEnableDelay);
            portal.SetActive(true);
        }

        // 5) Return to gameplay camera (restore priority)
        portalCam.Priority = originalPriority;

        // Allow blend back
        yield return new WaitForSeconds(blendSafety);

        // 6) Re-enable player control
        for (int i = 0; i < disableDuringCutscene.Length; i++)
        {
            if (disableDuringCutscene[i] != null)
                disableDuringCutscene[i].enabled = true;
        }

        isPlaying = false;
    }
}
