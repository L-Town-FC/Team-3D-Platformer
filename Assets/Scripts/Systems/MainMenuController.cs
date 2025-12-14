using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SampleScene";

    private void Update()
    {
        // Keyboard Enter
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            StartGame();

        // Optional: also allow Space
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            StartGame();
    }

    public void StartGame()
    {
        Time.timeScale = 1f; // safety if you ever return from pause
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
