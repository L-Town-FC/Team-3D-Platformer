using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ScenePortal : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string targetSceneName;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    [Header("Activation")]
    [SerializeField] private bool startActive = false;

    private Collider portalCollider;
    private bool isLoading;

    private Renderer[] cachedRenderers;

    private void Awake()
    {
        portalCollider = GetComponent<Collider>();
        portalCollider.isTrigger = true;

        // Cache renderers so we can "appear/disappear" the portal visually.
        // Includes child renderers too.
        cachedRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        SetActive(startActive);
    }

    public void SetActive(bool active)
    {
        // Disable the trigger so the portal can't be used.
        if (portalCollider != null)
            portalCollider.enabled = active;

        // Hide/show portal visuals.
        if (cachedRenderers != null)
        {
            for (int i = 0; i < cachedRenderers.Length; i++)
            {
                if (cachedRenderers[i] != null)
                    cachedRenderers[i].enabled = active;
            }
        }

        // If we ever deactivate it again, allow future loads once reactivated.
        if (!active)
            isLoading = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isLoading)
            return;

        if (!other.CompareTag(playerTag))
            return;

        // "Fully inside" check: player's bounds must be fully contained in portal bounds.
        Bounds portalBounds = portalCollider.bounds;
        Bounds playerBounds = other.bounds;

        if (portalBounds.Contains(playerBounds.min) && portalBounds.Contains(playerBounds.max))
        {
            isLoading = true;

            if (ProgressManager.Instance != null)
                //ProgressManager.Instance.UnlockLevel2();
                PlayerPrefs.SetInt(targetSceneName, 0); //marks next level unlocked

            Time.timeScale = 1f; // safety if you paused
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
