using UnityEngine;
using TMPro;

public class UILevelSelectState : UIBaseState
{
    //each ui state is associated with a unique menu script
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        
        //grab unique menu script so the transform can also be grabbed
        UIMenu = ui.GetComponentInChildren<LevelSelectMenu>(true);
        UIMenu.enabled = true;

        //grabs menu transform associated with this script, enables it, while disabling all other menus
        ui.GetMenu(UIMenu);

        //highlights top field in menu
        ui.HighlightActiveField(ui, ui.currentlyHighlightedField);
        
        Debug.Log("Active Menu: " + ui.activeMenu.name);
        Debug.Log("menuOptions: " + ui.menuOptions.name);
        Debug.Log("highlighted: " + ui.currentlyHighlightedField.name);

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

        //if submit button was pressed, check if the level is unlocked or not before loading level
        if (ui.uiInputActions.Submit.WasPressedThisFrame())
        {
            if (UIMenu.Submit(ui.currentlyHighlightedField.name))
            {
                ui.ChangeState(ui.UIPauseState);
            }

            return;
        }
    }
}
