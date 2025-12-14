using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text[] optionTexts; // [0]=Resume, [1]=Main Menu
    [SerializeField] private string cursor = "> ";

    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction; // your Player/Pause action

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private int selectedIndex;

    private void Start()
    {
        SetPaused(false);
        selectedIndex = 0;
        RefreshUI();
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnPause;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable();
        }

        // Safety: never leave timescale at 0 if object disables
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!isPaused)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex + 1) % 2;
            RefreshUI();
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex - 1 + 2) % 2;
            RefreshUI();
        }
        else if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ActivateSelection();
        }
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        SetPaused(!isPaused);

        // When pausing, default the selection to Resume (0)
        if (isPaused)
        {
            selectedIndex = 0;
            RefreshUI();
        }
    }

    private void ActivateSelection()
    {
        if (selectedIndex == 0)
        {
            Resume();
            return;
        }

        ReturnToMainMenu();
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pausePanel != null)
            pausePanel.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;

        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void RefreshUI()
    {
        if (optionTexts == null || optionTexts.Length < 2)
            return;

        // Enforce labels in case you want it fully standardized
        optionTexts[0].text = (selectedIndex == 0 ? cursor : "  ") + "Resume";
        optionTexts[1].text = (selectedIndex == 1 ? cursor : "  ") + "Main Menu";
    }
}
