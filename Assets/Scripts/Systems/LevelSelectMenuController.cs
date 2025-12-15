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

        return scenes[index];
    }

    protected override void OnSelect(int index)
    {
        if (scenes == null || index < 0 || index >= scenes.Length)
            return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(scenes[index]);
    }

    protected override bool CanCancel() => true;

    protected override void OnCancelPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
