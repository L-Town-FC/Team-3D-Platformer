using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TMP_Text[] optionTexts; // [0]=Respawn, [1]=Main Menu
    [SerializeField] private string cursor = "> ";

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isDead;
    private int selectedIndex;

    public bool IsDead => isDead;

    private void Start()
    {
        SetDead(false);
        selectedIndex = 0;
        RefreshUI();
    }

    private void Update()
    {
        if (!isDead)
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

    public void Die()
    {
        if (isDead)
            return;

        selectedIndex = 0;
        SetDead(true);
        RefreshUI();
    }

    private void ActivateSelection()
    {
        if (selectedIndex == 0)
            Respawn();
        else
            ReturnToMainMenu();
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        var sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetDead(bool dead)
    {
        isDead = dead;

        if (deathPanel != null)
            deathPanel.SetActive(dead);

        Time.timeScale = dead ? 0f : 1f;

        Cursor.visible = dead;
        Cursor.lockState = dead ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void RefreshUI()
    {
        if (optionTexts == null || optionTexts.Length < 2)
            return;

        optionTexts[0].text = (selectedIndex == 0 ? cursor : "  ") + "Respawn";
        optionTexts[1].text = (selectedIndex == 1 ? cursor : "  ") + "Main Menu";
    }
}
