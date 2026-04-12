using UnityEngine;

public class UIUnpausedState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;

        //grab unique menu script so the transform can also be grabbed
        UIMenu = ui.GetComponentInChildren<Unpaused>(true);
        UIMenu.enabled = true;

        //grabs menu transform associated with this script, enables it, while disabling all other menus
        ui.GetMenu(UIMenu);

        //highlights top field in menu
        ui.HighlightActiveField(ui, ui.currentlyHighlightedField);

    }

    public override void ExitState(UIStateManager ui)
    {
        Time.timeScale = 0f;
    }

    public override void UpdateState(UIStateManager ui)
    {
        if (ui.uiInputActions.Pause.WasPressedThisFrame())
        {
            ui.ChangeState(ui.UIPauseState);
            return; 
        }

        //DeathMenu Condition
    }
}
