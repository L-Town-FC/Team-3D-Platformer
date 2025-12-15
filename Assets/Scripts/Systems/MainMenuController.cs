using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string levelSelectScene = "LevelSelect";

    private PlayerControls controls;
    private InputAction submitAction;

    private void Awake()
    {
        controls = new PlayerControls();
        submitAction = controls.UI.Submit;
    }

    private void OnEnable()
    {
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Disable();
    }

    private void Update()
    {
        if (submitAction != null && submitAction.WasPressedThisFrame())
        {
            GoToLevelSelect();
        }
    }

    private void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectScene);
    }
}
