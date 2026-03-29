using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MenuControllerBase
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Dependencies")]
    [SerializeField] private DeathMenuController deathManager;

    protected override void Start()
    {
        base.Start();
        Time.timeScale = 1f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (deathManager == null)
            deathManager = FindFirstObjectByType<DeathMenuController>();

        var ui = UIInput.Instance;
        if (ui != null)
            ui.Controls.UI.Pause.performed += OnPause;
    }

    protected override void OnDisable()
    {
        var ui = UIInput.Instance;
        if (ui != null)
            ui.Controls.UI.Pause.performed -= OnPause;

        Time.timeScale = 1f;
        base.OnDisable();
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (deathManager != null && deathManager.IsDead)
            return;

        if (isOpen) Close();
        else Open();
    }

    protected override void OnOpened()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
    }

    protected override void OnClosed()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnSelect(int index)
    {
        // 0 = Resume, 1 = Main Menu, 2 = Settings
        if (index == 0)
        {
            Close();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Optional: allow cancel to behave like resume
    protected override bool CanCancel() => true;

    protected override void OnCancelPressed()
    {
        if (deathManager != null && deathManager.IsDead)
            return;

        Close();
    }

    private void LateUpdate()
    {
        // If death happens while paused, force close so death UI owns the screen
        if (deathManager != null && deathManager.IsDead && isOpen)
            Close();
    }
}
