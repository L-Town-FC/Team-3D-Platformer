using UnityEngine;

public class UILevelSelectState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        UIMenu = ui.GetComponentInChildren<LevelSelectMenu>(true);
        ui.GetMenu(UIMenu);
    }

    public override void ExitState(UIStateManager ui)
    {
    }

    public override void UpdateState(UIStateManager ui)
    {
        if (ui.uiInputActions.Navigate.WasPressedThisFrame())
        {
            ui.NavigateMenu((int)ui.uiInputActions.Navigate.ReadValue<Vector2>().y);
        }
        Debug.Log(ui.currentMenuIndex);
    }
}
