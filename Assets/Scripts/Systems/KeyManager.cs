using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [Header("Keys")]
    [SerializeField] private int requiredKeys = 3;

    [Header("Portal")]
    [SerializeField] private ScenePortal portal;

    public int RequiredKeys => requiredKeys;
    public int CollectedKeys { get; private set; }

    public event Action<int, int> OnKeyCountChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (portal != null)
            portal.SetActive(false);

        // ✅ Initialize from persisted progress for this scene
        string sceneName = SceneManager.GetActiveScene().name;

        if (ProgressManager.Instance != null)
            CollectedKeys = Mathf.Clamp(ProgressManager.Instance.GetCollectedKeyCount(sceneName), 0, requiredKeys);
        else
            CollectedKeys = 0;

        // If already complete, ensure portal is active (or play reveal if you want)
        if (CollectedKeys >= requiredKeys)
        {
            // If you want the cutscene to play again when returning, keep your cutscene logic.
            // Usually you DON'T; you just want the portal available.
            if (portal != null)
                portal.SetActive(true);
        }

        RaiseChanged();
    }

    public void RegisterKeyCollected()
    {
        CollectedKeys = Mathf.Clamp(CollectedKeys + 1, 0, requiredKeys);
        RaiseChanged();

        if (CollectedKeys >= requiredKeys)
        {
            PortalRevealCutscene cutscene = FindFirstObjectByType<PortalRevealCutscene>();
            if (cutscene != null)
                cutscene.Play();
            else if (portal != null)
                portal.SetActive(true);
        }
    }

    public void ResetKeys()
    {
        CollectedKeys = 0;

        if (portal != null)
            portal.SetActive(false);

        // Optional: also wipe persisted keys for this scene
        if (ProgressManager.Instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            ProgressManager.Instance.ResetCollectedKeysForScene(sceneName);
        }

        RaiseChanged();
    }

    private void RaiseChanged()
    {
        OnKeyCountChanged?.Invoke(CollectedKeys, requiredKeys);
    }
}
