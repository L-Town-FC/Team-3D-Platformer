using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class DeathMenuController : MenuControllerBase
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    protected override void OnEnable()
    {
        base.OnEnable();
        Player.playerDeath += Die;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Player.playerDeath -= Die;
    }

    public bool IsDead => isOpen;

    public void Die()
    {
        if (isOpen)
            return;

        Open();
    }

    protected override void OnOpened()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    protected override void OnClosed()
    {
        // Usually you never “close” death without respawning, but keep safe.
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnSelect(int index)
    {
        // 0 = Respawn, 1 = Main Menu
        if (index == 0)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    protected override bool CanCancel() => false;
}
