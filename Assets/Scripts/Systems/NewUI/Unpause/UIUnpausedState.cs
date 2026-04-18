using UnityEngine;

public class UIUnpausedState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }
    bool isDead = false;

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        Player.playerDeath += Die;
        
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
        if (isDead)
        {
            isDead = false;
            ui.ChangeState(ui.UIDeathState);
            return;
        }

        if (ui.uiInputActions.Pause.WasPressedThisFrame())
        {
            ui.ChangeState(ui.UIPauseState);
            return; 
        }
        
    }

    void Die()
    {
        isDead = true;
        Player.playerDeath -= Die;
    }
}
