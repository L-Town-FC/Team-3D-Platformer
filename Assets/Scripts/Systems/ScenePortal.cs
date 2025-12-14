using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string playerTag = "Player";

    private Collider portalCollider;
    private bool isLoading;

    private void Awake()
    {
        portalCollider = GetComponent<Collider>();
        portalCollider.isTrigger = true;
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
            Time.timeScale = 1f; // safety if you paused
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
