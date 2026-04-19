using UnityEngine;

public class UIPauseState : UIBaseState
{
    //TODO: Create Pause menu object
    //TODO: Modify Pause menu
    //TODO: Create Death menu
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        Time.timeScale = 0f;

        //grab unique menu script so the transform can also be grabbed
        UIMenu = ui.GetComponentInChildren<PauseMenu>(true);
        UIMenu.enabled = true;

        //grabs menu transform associated with this script, enables it, while disabling all other menus
        ui.GetMenu(UIMenu);

        //highlights top field in menu
        ui.HighlightActiveField(ui, ui.currentlyHighlightedField);
    }

    public override void ExitState(UIStateManager ui)
    {
        Time.timeScale = 1f;
    }

    public override void UpdateState(UIStateManager ui)
    {
        //throw new System.NotImplementedException();
        if (ui.uiInputActions.Cancel.WasPressedThisFrame())
        {
            ui.ChangeState(ui.UIUnpausedState);
            return;
        }

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
            return;
        }
    }

    void SelectMenuOption(UIStateManager ui)
    {
        //Resume 
        switch (ui.currentlyHighlightedFieldIndex)
        {
            case 0: //Resume
                Time.timeScale = 1f;
                ui.ChangeState(ui.UIUnpausedState);
                break;
            case 1: //Main Menu
                ui.ChangeState(ui.UIMainMenuState);
                break;
            case 2: //Level Select
                ui.ChangeState(ui.UILevelSelectState);
                break;
            case 3: // Settings
                ui.ChangeState(ui.UISettingsState);
                break;
            case 4: //Exit game
                Debug.Log("Exit Game");
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                break;
        }
    }
}
