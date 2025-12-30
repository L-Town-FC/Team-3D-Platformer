using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Persistence")]
    [SerializeField] private string keyId; // MUST be unique within the scene (e.g., "Key_01", "Key_Rooftop", etc.)

    private bool collected;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;

        // Nice default so you don't forget to set it, but you SHOULD override in Inspector.
        if (string.IsNullOrEmpty(keyId))
            keyId = gameObject.name;
    }

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        // If progress manager exists and says this key was collected, delete it immediately.
        if (ProgressManager.Instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (!string.IsNullOrEmpty(keyId) && ProgressManager.Instance.IsKeyCollected(sceneName, keyId))
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        bool isPlayer =
            other.CompareTag(playerTag) ||
            other.transform.root.CompareTag(playerTag);

        if (!isPlayer)
            return;

        collected = true;

        // Persist THIS key as collected
        if (ProgressManager.Instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (!string.IsNullOrEmpty(keyId))
                ProgressManager.Instance.MarkKeyCollected(sceneName, keyId);
            else
                Debug.LogWarning($"KeyPickup: keyId is empty on {name}. Set a unique keyId in Inspector.");
        }
        else
        {
            Debug.LogWarning("KeyPickup: No ProgressManager found (key won't persist).");
        }

        // Tell KeyManager to update HUD / portal logic
        if (KeyManager.Instance != null)
            KeyManager.Instance.RegisterKeyCollected();
        else
            Debug.LogWarning("KeyPickup: No KeyManager found in scene.");

        Destroy(gameObject);
    }
}
