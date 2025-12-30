using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenuController : MenuControllerBase
{
    [Header("Scene names must match exactly (and be in Build Profiles)")]
    [SerializeField] private string[] scenes = { "SampleScene", "Level2" };

    protected override void Start()
    {
        base.Start();
        Time.timeScale = 1f;
        Open(); // level select is the whole scene UI
    }

    protected override string GetLabelForIndex(int index)
    {
        if (scenes == null || index < 0 || index >= scenes.Length)
            return base.GetLabelForIndex(index);

        string label = scenes[index];

        if (label == "Level2")
        {
            bool unlocked = (ProgressManager.Instance != null) && ProgressManager.Instance.Level2Unlocked;
            if (!unlocked)
                return "Level2 (Locked)";
        }

        return label;
    }


    protected override void OnSelect(int index)
    {
        if (scenes == null || index < 0 || index >= scenes.Length)
            return;

        string sceneToLoad = scenes[index];

        // Gate Level2
        if (sceneToLoad == "Level2")
        {
            bool unlocked = (ProgressManager.Instance != null) && ProgressManager.Instance.Level2Unlocked;
            if (!unlocked)
            {
                // Optional: add a UI beep / message later; for now we just ignore.
                Debug.Log("Level 2 is locked. Reach it via the Level 1 portal first.");
                return;
            }
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
