using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenuController : MenuControllerBase
{
    [Header("Scene names must match exactly (and be in Build Profiles)")]
    [SerializeField] private string[] scenes = { "SampleScene", "Level2" };

    [Header("For Testing only")]
    [SerializeField] private bool lockLevels = false;

    protected override void Start()
    {
        base.Start();

        PlayerPrefs.SetInt("Level1", 0); //Player should always have level 1 unlocked

        Time.timeScale = 1f;
        Open(); // level select is the whole scene UI
    }

    protected override void Update()
    {
        base.Update();

        if (lockLevels)
        {
            PlayerPrefs.DeleteKey("Level2");
            PlayerPrefs.DeleteKey("Level3");
        }
    }

    protected override string GetLabelForIndex(int index)
    {
        if (scenes == null || index < 0 || index >= scenes.Length)
            return base.GetLabelForIndex(index);

        string label = scenes[index];

        if (!PlayerPrefs.HasKey(label))
        {
            return label + " (Locked)";
        }

        return label;
    }


    protected override void OnSelect(int index)
    {
        if (scenes == null || index < 0 || index >= scenes.Length)
            return;

        string sceneToLoad = scenes[index];

        // Gate Level2
            bool unlocked = (ProgressManager.Instance != null) && PlayerPrefs.HasKey(sceneToLoad);
            if (!unlocked)
            {
                // Optional: add a UI beep / message later; for now we just ignore.
                Debug.Log(sceneToLoad + " is locked. Reach it via the previous Level portal first.");
                return;
            }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }


    protected override bool CanCancel() => true;

    protected override void OnCancelPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
