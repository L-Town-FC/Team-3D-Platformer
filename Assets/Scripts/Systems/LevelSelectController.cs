using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [Header("Scene names must match exactly (and be in Build Profiles)")]
    [SerializeField] private string[] scenes = { "SampleScene", "Level2" };

    [Header("UI options (TMP Text) in the same order as scenes")]
    [SerializeField] private TMP_Text[] optionTexts;

    [SerializeField] private string cursor = "> ";
    private int selectedIndex = 0;

    private void Start()
    {
        Time.timeScale = 1f;
        RefreshUI();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex + 1) % scenes.Length;
            RefreshUI();
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex - 1 + scenes.Length) % scenes.Length;
            RefreshUI();
        }
        else if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            LoadSelected();
        }
        // Optional: back to main menu
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void RefreshUI()
    {
        if (optionTexts == null || optionTexts.Length == 0)
            return;

        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (optionTexts[i] == null)
                continue;

            string label = (i < scenes.Length) ? scenes[i] : optionTexts[i].text;
            optionTexts[i].text = (i == selectedIndex ? cursor : "  ") + label;
        }
    }

    private void LoadSelected()
    {
        if (selectedIndex < 0 || selectedIndex >= scenes.Length)
            return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(scenes[selectedIndex]);
    }
}
