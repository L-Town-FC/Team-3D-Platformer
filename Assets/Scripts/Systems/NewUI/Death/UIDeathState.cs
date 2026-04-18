using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDeathState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        Time.timeScale = 0f;

        stateEnterTime = Time.time;
        UIMenu = ui.GetComponentInChildren<DeathMenu>(true);

        //grabs menu transform associated with this script, enables it, while disabling all other menus
        ui.GetMenu(UIMenu);

        //highlights top field in menu
        ui.HighlightActiveField(ui, ui.currentlyHighlightedField);
    }

    public override void ExitState(UIStateManager ui)
    {

    }

    public override void UpdateState(UIStateManager ui)
    {
        //reads input and navigates up or down through child hierarchy
        //underlines the text of the currently selected option
        if (ui.uiInputActions.Navigate.WasPressedThisFrame())
        {
            ui.NavigateMenu((int)ui.uiInputActions.Navigate.ReadValue<Vector2>().y);
            ui.HighlightActiveField(ui, ui.currentlyHighlightedField);
            return;
        }

        if (ui.uiInputActions.Submit.WasPressedThisFrame())
        {
            SelectMenuOption(ui);
        }
    }

    void SelectMenuOption(UIStateManager ui)
    {
        //Resume 
        switch (ui.currentlyHighlightedFieldIndex)
        {
            case 0: //Respawn
                //Reloads scene
                ui.ChangeState(ui.UIPauseState);
                SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex));
                break;
            case 1: //Main Menu
                ui.ChangeState(ui.UIMainMenuState);
                break;
            case 2: //Exit game
                Debug.Log("Exit Game");
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                break;
        }
    }
}
