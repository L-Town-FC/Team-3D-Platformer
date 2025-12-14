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

    [Header("Dependencies")]
    [SerializeField] private DeathManager deathManager; // drag your DeathManager here

    private bool isPaused;
    private int selectedIndex;

    private void Awake()
    {
        if (deathManager == null)
            deathManager = FindFirstObjectByType<DeathManager>();
    }

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

        Time.timeScale = 1f;
    }

    private void Update()
    {
        // If dead, pause menu should never be navigable
        if (deathManager != null && deathManager.IsDead)
            return;

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

    private void LateUpdate()
    {
        // If we became dead while paused, force pause off so Death UI owns timescale/UI
        if (deathManager != null && deathManager.IsDead && isPaused)
            SetPaused(false);
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        // Ignore pause toggles while dead
        if (deathManager != null && deathManager.IsDead)
            return;

        SetPaused(!isPaused);

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

        optionTexts[0].text = (selectedIndex == 0 ? cursor : "  ") + "Resume";
        optionTexts[1].text = (selectedIndex == 1 ? cursor : "  ") + "Main Menu";
    }
}
