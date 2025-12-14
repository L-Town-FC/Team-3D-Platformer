using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string levelSelectScene = "LevelSelect";

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            GoToLevelSelect();

        // Optional: also allow Enter
        if (Keyboard.current.enterKey.wasPressedThisFrame)
            GoToLevelSelect();
    }

    private void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectScene);
    }
}
