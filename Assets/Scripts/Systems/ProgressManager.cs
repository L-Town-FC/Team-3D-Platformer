using UnityEngine;

public class ProgressManager : MonoBehaviour
{

#if UNITY_EDITOR
    [Header("Editor Testing")]
    [SerializeField] private bool resetProgressOnPlayInEditor = true;
    [SerializeField] private string editorTestSceneName = "SampleScene";
#endif

    public static ProgressManager Instance { get; private set; }

    private const string KEY_LEVEL2_UNLOCKED = "Progress_Level2Unlocked";

    // Example saved value per scene: "Key_01|Key_02|Key_03"
    private const string KEY_COLLECTED_KEYS_PREFIX = "Progress_CollectedKeys_";

    public bool Level2Unlocked { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject go = new GameObject("ProgressManager");
        go.AddComponent<ProgressManager>();
        // Awake runs immediately after AddComponent
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        if (resetProgressOnPlayInEditor)
        {
            PlayerPrefs.DeleteKey("Progress_Level2Unlocked");
            PlayerPrefs.DeleteKey("Progress_CollectedKeys_" + editorTestSceneName);
            PlayerPrefs.Save();

            Debug.Log($"ProgressManager: Editor reset on Play (scene={editorTestSceneName}).");
        }
#endif

        Load();

        Debug.Log($"ProgressManager: Awake. Level2Unlocked = {Level2Unlocked}");
    }


    private void Load()
    {
        Level2Unlocked = PlayerPrefs.GetInt(KEY_LEVEL2_UNLOCKED, 0) == 1;
    }

    private void SaveLevel2()
    {
        PlayerPrefs.SetInt(KEY_LEVEL2_UNLOCKED, Level2Unlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void UnlockLevel2()
    {
        if (Level2Unlocked)
            return;

        Level2Unlocked = true;
        SaveLevel2();

        Debug.Log("ProgressManager: Level 2 unlocked + saved.");
    }

    public void ResetProgress()
    {
        Level2Unlocked = false;
        SaveLevel2();

        Debug.Log("ProgressManager: Progress reset (Level2Unlocked=false).");
    }

    // -----------------------------
    // Key persistence (per scene)
    // -----------------------------

    public bool IsKeyCollected(string sceneName, string keyId)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || string.IsNullOrWhiteSpace(keyId))
            return false;

        string stored = PlayerPrefs.GetString(KEY_COLLECTED_KEYS_PREFIX + sceneName, "");
        if (string.IsNullOrEmpty(stored))
            return false;

        string[] parts = stored.Split('|');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == keyId)
                return true;
        }

        return false;
    }

    public void MarkKeyCollected(string sceneName, string keyId)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || string.IsNullOrWhiteSpace(keyId))
            return;

        if (IsKeyCollected(sceneName, keyId))
            return;

        string prefKey = KEY_COLLECTED_KEYS_PREFIX + sceneName;
        string stored = PlayerPrefs.GetString(prefKey, "");

        stored = string.IsNullOrEmpty(stored) ? keyId : (stored + "|" + keyId);

        PlayerPrefs.SetString(prefKey, stored);
        PlayerPrefs.Save();
    }

    public int GetCollectedKeyCount(string sceneName)
    {
        string stored = PlayerPrefs.GetString(KEY_COLLECTED_KEYS_PREFIX + sceneName, "");
        if (string.IsNullOrEmpty(stored))
            return 0;

        string[] parts = stored.Split('|');
        int count = 0;

        for (int i = 0; i < parts.Length; i++)
        {
            if (!string.IsNullOrEmpty(parts[i]))
                count++;
        }

        return count;
    }

    public void ResetCollectedKeysForScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        PlayerPrefs.DeleteKey(KEY_COLLECTED_KEYS_PREFIX + sceneName);
        PlayerPrefs.Save();
    }
}
