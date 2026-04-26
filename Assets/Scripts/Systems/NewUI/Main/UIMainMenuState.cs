using UnityEngine;

public class UIMainMenuState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        UIMenu = ui.GetComponentInChildren<MainMenu>(true);
        ui.GetMenu(UIMenu);
    }

    public override void ExitState(UIStateManager ui)
    {
    }

    public override void UpdateState(UIStateManager ui)
    {
        if (ui.uiInputActions.Submit.WasPressedThisFrame())
        {
            ui.ChangeState(ui.UILevelSelectState);
            return;
        }

        if (ui.uiInputActions.Reset.WasPerformedThisFrame())
        {
            Debug.Log("Resetting");
            PlayerPrefs.DeleteAll();
        }
    }
}
